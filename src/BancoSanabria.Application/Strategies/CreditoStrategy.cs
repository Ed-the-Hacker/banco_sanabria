using BancoSanabria.Domain.Entities;
using BancoSanabria.Infrastructure.UnitOfWork;

namespace BancoSanabria.Application.Strategies
{
    /// <summary>
    /// Strategy para movimientos de tipo Crédito (depósitos)
    /// </summary>
    public class CreditoStrategy : IMovimientoStrategy
    {
        private readonly IUnitOfWork _unitOfWork;

        public string TipoMovimiento => "Credito";

        public CreditoStrategy(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> CalcularNuevoSaldoAsync(Cuenta cuenta, decimal valor)
        {
            // Obtener el último saldo registrado
            var ultimoMovimiento = await _unitOfWork.Movimientos
                .GetUltimoMovimientoAsync(cuenta.CuentaId);

            var saldoActual = ultimoMovimiento?.Saldo ?? cuenta.SaldoInicial;

            // Para créditos, el valor es positivo y se suma al saldo
            return saldoActual + Math.Abs(valor);
        }

        public Task ValidarMovimientoAsync(Cuenta cuenta, decimal valor, DateTime fecha)
        {
            // Los créditos no tienen validaciones especiales
            return Task.CompletedTask;
        }
    }
}

