using BancoSanabria.Domain.Entities;

namespace BancoSanabria.Application.Strategies
{
    /// <summary>
    /// Aplicando Patrón Strategy para gestionar tipos de movimiento
    /// </summary>
    public interface IMovimientoStrategy
    {
        string TipoMovimiento { get; }
        Task<decimal> CalcularNuevoSaldoAsync(Cuenta cuenta, decimal valor);
        Task ValidarMovimientoAsync(Cuenta cuenta, decimal valor, DateTime fecha);
    }
}

