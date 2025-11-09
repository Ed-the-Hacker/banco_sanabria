using BancoSanabria.Domain.Entities;

namespace BancoSanabria.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio específico para Cliente con operaciones adicionales
    /// </summary>
    public interface IClienteRepository : IRepository<Cliente>
    {
        Task<Cliente?> GetByIdentificacionAsync(string identificacion);
        Task<Cliente?> GetWithCuentasAsync(int clienteId);
    }
}

