using BancoSanabria.Application.Common.Interfaces;
using BancoSanabria.Application.DTOs;
using BancoSanabria.Domain.Exceptions;

namespace BancoSanabria.Application.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPdfGeneratorService _pdfGenerator;

        public ReporteService(IUnitOfWork unitOfWork, IPdfGeneratorService pdfGenerator)
        {
            _unitOfWork = unitOfWork;
            _pdfGenerator = pdfGenerator;
        }

        public async Task<ReporteDto> GenerarReporteAsync(ReporteRequestDto request)
        {
            // Validar que el cliente existe
            var cliente = await _unitOfWork.Clientes.GetWithCuentasAsync(request.ClienteId);
            if (cliente == null)
                throw new BusinessException($"Cliente con ID {request.ClienteId} no encontrado");

            var reporte = new ReporteDto
            {
                Cliente = new ClienteReporteDto
                {
                    Nombre = cliente.Nombre,
                    Identificacion = cliente.Identificacion,
                    Direccion = cliente.Direccion,
                    Telefono = cliente.Telefono
                },
                Cuentas = new List<CuentaReporteDto>()
            };

            // Procesar cada cuenta del cliente usando LINQ
            foreach (var cuenta in cliente.Cuentas)
            {
                var movimientos = await _unitOfWork.Movimientos
                    .GetMovimientosByFechaRangoAsync(cuenta.CuentaId, request.FechaInicio, request.FechaFin);

                var movimientosList = movimientos.ToList();

                // Calcular saldo inicial (antes del rango de fechas)
                var movimientoAnterior = await _unitOfWork.Movimientos
                    .GetUltimoMovimientoAsync(cuenta.CuentaId);
                
                decimal saldoInicial = cuenta.SaldoInicial;
                if (movimientosList.Any())
                {
                    // Si hay movimientos en el rango, el saldo inicial es el del primer movimiento menos su valor
                    var primerMovimiento = movimientosList.First();
                    saldoInicial = primerMovimiento.Saldo - primerMovimiento.Valor;
                }
                else if (movimientoAnterior != null)
                {
                    // Si no hay movimientos en el rango pero hay anteriores, usar el último
                    saldoInicial = movimientoAnterior.Saldo;
                }

                var cuentaReporte = new CuentaReporteDto
                {
                    NumeroCuenta = cuenta.NumeroCuenta,
                    TipoCuenta = cuenta.TipoCuenta,
                    SaldoInicial = saldoInicial,
                    Estado = cuenta.Estado,
                    Movimientos = movimientosList.Select(m => new MovimientoReporteDto
                    {
                        Fecha = m.Fecha,
                        TipoMovimiento = m.TipoMovimiento,
                        Valor = m.Valor,
                        Saldo = m.Saldo
                    }).ToList()
                };

                // Calcular saldo disponible (último movimiento del rango o saldo inicial)
                cuentaReporte.SaldoDisponible = movimientosList.Any() 
                    ? movimientosList.Last().Saldo 
                    : saldoInicial;

                // Calcular totales usando LINQ
                cuentaReporte.TotalCreditos = movimientosList
                    .Where(m => m.TipoMovimiento.Equals("Credito", StringComparison.OrdinalIgnoreCase))
                    .Sum(m => Math.Abs(m.Valor));

                cuentaReporte.TotalDebitos = movimientosList
                    .Where(m => m.TipoMovimiento.Equals("Debito", StringComparison.OrdinalIgnoreCase))
                    .Sum(m => Math.Abs(m.Valor));

                reporte.Cuentas.Add(cuentaReporte);
            }

            // Generar PDF en Base64
            reporte.PdfBase64 = await _pdfGenerator.GenerarPdfReporteAsync(reporte, request);

            return reporte;
        }
    }
}

