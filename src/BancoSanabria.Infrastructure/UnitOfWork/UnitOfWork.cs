using BancoSanabria.Infrastructure.Data;
using BancoSanabria.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace BancoSanabria.Infrastructure.UnitOfWork
{
    /// <summary>
    /// Implementación del patrón Unit of Work
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BancoDbContext _context;
        private IDbContextTransaction? _transaction;
        
        public IClienteRepository Clientes { get; }
        public ICuentaRepository Cuentas { get; }
        public IMovimientoRepository Movimientos { get; }

        public UnitOfWork(
            BancoDbContext context,
            IClienteRepository clienteRepository,
            ICuentaRepository cuentaRepository,
            IMovimientoRepository movimientoRepository)
        {
            _context = context;
            Clientes = clienteRepository;
            Cuentas = cuentaRepository;
            Movimientos = movimientoRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

