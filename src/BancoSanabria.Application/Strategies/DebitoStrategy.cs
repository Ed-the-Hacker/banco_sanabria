using BancoSanabria.Domain.Entities;
using BancoSanabria.Domain.Exceptions;
using BancoSanabria.Infrastructure.UnitOfWork;

namespace BancoSanabria.Application.Strategies
{
    /// <summary>
    /// Strategy para movimientos de tipo Débito (retiros)
    /// </summary>
    public class DebitoStrategy : IMovimientoStrategy
    {
        private readonly IUnitOfWork _unitOfWork;
        private const decimal LimiteDiario = 1000m;

        public string TipoMovimiento => "Debito";

        public DebitoStrategy(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> CalcularNuevoSaldoAsync(Cuenta cuenta, decimal valor)
        {
            // Obtener el último saldo registrado
            var ultimoMovimiento = await _unitOfWork.Movimientos
                .GetUltimoMovimientoAsync(cuenta.CuentaId);

            var saldoActual = ultimoMovimiento?.Saldo ?? cuenta.SaldoInicial;

            // Para débitos, el valor es negativo y se resta del saldo
            return saldoActual - Math.Abs(valor);
        }

        public async Task ValidarMovimientoAsync(Cuenta cuenta, decimal valor, DateTime fecha)
        {
            var valorAbsoluto = Math.Abs(valor);

            // Obtener el saldo actual
            var ultimoMovimiento = await _unitOfWork.Movimientos
                .GetUltimoMovimientoAsync(cuenta.CuentaId);
            
            var saldoActual = ultimoMovimiento?.Saldo ?? cuenta.SaldoInicial;

            // Validación 1: Verificar que el saldo no sea 0
            if (saldoActual == 0)
            {
                throw new BusinessException("Saldo no disponible");
            }

            // Validación 2: Verificar que haya suficiente saldo para el débito
            if (saldoActual < valorAbsoluto)
            {
                throw new BusinessException("Saldo no disponible");
            }

            // Validación 3: Verificar el límite diario de retiros (1000)
            var sumaDebitosHoy = await _unitOfWork.Movimientos
                .GetSumaDebitosDelDiaAsync(cuenta.CuentaId, fecha);

            if (sumaDebitosHoy + valorAbsoluto > LimiteDiario)
            {
                throw new BusinessException("Cupo diario Excedido");
            }
        }
    }
}

