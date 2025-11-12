using BancoSanabria.Application.Common.Interfaces;
using BancoSanabria.Application.DTOs;
using BancoSanabria.Application.Strategies;
using BancoSanabria.Domain.Entities;
using BancoSanabria.Domain.Exceptions;

namespace BancoSanabria.Application.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MovimientoStrategyFactory _strategyFactory;

        public MovimientoService(
            IUnitOfWork unitOfWork,
            MovimientoStrategyFactory strategyFactory)
        {
            _unitOfWork = unitOfWork;
            _strategyFactory = strategyFactory;
        }

        public async Task<IEnumerable<MovimientoDto>> GetAllAsync()
        {
            var movimientos = await _unitOfWork.Movimientos.GetAllAsync();
            
            return movimientos.Select(MapToDto).ToList();
        }

        public async Task<MovimientoDto?> GetByIdAsync(int id)
        {
            var movimiento = await _unitOfWork.Movimientos.GetByIdAsync(id);
            
            return movimiento == null ? null : MapToDto(movimiento);
        }

        public async Task<IEnumerable<MovimientoDto>> GetByCuentaIdAsync(int cuentaId)
        {
            var movimientos = await _unitOfWork.Movimientos
                .GetMovimientosByCuentaIdAsync(cuentaId);
            
            return movimientos.Select(MapToDto).ToList();
        }

        public async Task<MovimientoDto> CreateAsync(MovimientoCreateDto movimientoDto)
        {
            // Validar que la cuenta existe
            var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(movimientoDto.CuentaId);
            if (cuenta == null)
                throw new BusinessException($"Cuenta con ID {movimientoDto.CuentaId} no encontrada");

            if (!cuenta.Estado)
                throw new BusinessException("La cuenta está inactiva");

            // Aplicando Patrón Strategy para gestionar tipos de movimiento
            var strategy = _strategyFactory.GetStrategy(movimientoDto.TipoMovimiento);

            // Validar el movimiento usando la estrategia
            await strategy.ValidarMovimientoAsync(cuenta, movimientoDto.Valor, movimientoDto.Fecha);

            // Calcular el nuevo saldo usando la estrategia
            var nuevoSaldo = await strategy.CalcularNuevoSaldoAsync(cuenta, movimientoDto.Valor);

            // Crear el movimiento
            var movimiento = new Movimiento
            {
                Fecha = movimientoDto.Fecha,
                TipoMovimiento = movimientoDto.TipoMovimiento,
                // El valor se guarda con signo según el tipo
                Valor = movimientoDto.TipoMovimiento.Equals("Debito", StringComparison.OrdinalIgnoreCase) 
                    ? -Math.Abs(movimientoDto.Valor) 
                    : Math.Abs(movimientoDto.Valor),
                Saldo = nuevoSaldo,
                CuentaId = movimientoDto.CuentaId
            };

            // Usar transacción para garantizar consistencia
            await _unitOfWork.BeginTransactionAsync();
            
            try
            {
                await _unitOfWork.Movimientos.AddAsync(movimiento);
                
                // Actualizar el saldo actual de la cuenta
                cuenta.SaldoInicial = nuevoSaldo;
                await _unitOfWork.Cuentas.UpdateAsync(cuenta);
                
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            return MapToDto(movimiento);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movimiento = await _unitOfWork.Movimientos.GetByIdAsync(id);
            
            if (movimiento == null)
                return false;

            // Validar que sea el último movimiento de la cuenta
            var ultimoMovimiento = await _unitOfWork.Movimientos
                .GetUltimoMovimientoAsync(movimiento.CuentaId);

            if (ultimoMovimiento?.MovimientoId != id)
                throw new BusinessException("Solo se puede eliminar el último movimiento registrado");

            await _unitOfWork.Movimientos.DeleteAsync(movimiento);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static MovimientoDto MapToDto(Movimiento movimiento)
        {
            return new MovimientoDto
            {
                MovimientoId = movimiento.MovimientoId,
                Fecha = movimiento.Fecha,
                TipoMovimiento = movimiento.TipoMovimiento,
                Valor = movimiento.Valor,
                Saldo = movimiento.Saldo,
                CuentaId = movimiento.CuentaId,
                NumeroCuenta = movimiento.Cuenta?.NumeroCuenta
            };
        }
    }
}

