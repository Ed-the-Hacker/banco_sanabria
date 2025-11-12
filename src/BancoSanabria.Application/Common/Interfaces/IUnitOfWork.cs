using System;
using System.Threading.Tasks;

namespace BancoSanabria.Application.Common.Interfaces;

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

