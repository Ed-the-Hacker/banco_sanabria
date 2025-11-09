using BancoSanabria.Domain.Entities;

namespace BancoSanabria.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio específico para Movimiento
    /// </summary>
    public interface IMovimientoRepository : IRepository<Movimiento>
    {
        Task<IEnumerable<Movimiento>> GetMovimientosByCuentaIdAsync(int cuentaId);
        Task<IEnumerable<Movimiento>> GetMovimientosByFechaRangoAsync(int cuentaId, DateTime fechaInicio, DateTime fechaFin);
        Task<decimal> GetSumaDebitosDelDiaAsync(int cuentaId, DateTime fecha);
        Task<Movimiento?> GetUltimoMovimientoAsync(int cuentaId);
    }
}

