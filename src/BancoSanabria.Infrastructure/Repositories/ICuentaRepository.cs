using BancoSanabria.Domain.Entities;

namespace BancoSanabria.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio específico para Cuenta
    /// </summary>
    public interface ICuentaRepository : IRepository<Cuenta>
    {
        Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta);
        Task<IEnumerable<Cuenta>> GetCuentasByClienteIdAsync(int clienteId);
        Task<Cuenta?> GetWithMovimientosAsync(int cuentaId);
    }
}

