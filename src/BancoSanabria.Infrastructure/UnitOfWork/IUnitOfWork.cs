using BancoSanabria.Infrastructure.Repositories;

namespace BancoSanabria.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Interfaz del patrón Unit of Work para gestionar transacciones
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IClienteRepository Clientes { get; }
        ICuentaRepository Cuentas { get; }
        IMovimientoRepository Movimientos { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}

