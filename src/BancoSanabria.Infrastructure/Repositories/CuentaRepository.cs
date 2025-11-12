using System.Linq.Expressions;
using BancoSanabria.Application.Common.Interfaces;
using BancoSanabria.Domain.Entities;
using BancoSanabria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoSanabria.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Cuenta
    /// </summary>
    public class CuentaRepository : Repository<Cuenta>, ICuentaRepository
    {
        public CuentaRepository(BancoDbContext context) : base(context)
        {
        }

        public async Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta)
        {
            return await _dbSet
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);
        }

        public async Task<IEnumerable<Cuenta>> GetCuentasByClienteIdAsync(int clienteId)
        {
            return await _dbSet
                .Include(c => c.Cliente)
                .Where(c => c.ClienteId == clienteId)
                .ToListAsync();
        }

        public async Task<Cuenta?> GetWithMovimientosAsync(int cuentaId)
        {
            return await _dbSet
                .Include(c => c.Movimientos.OrderBy(m => m.Fecha))
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.CuentaId == cuentaId);
        }

        public override async Task<Cuenta?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.CuentaId == id);
        }

        public override async Task<IEnumerable<Cuenta>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Cliente)
                .ToListAsync();
        }

        public override async Task<IEnumerable<Cuenta>> FindAsync(Expression<Func<Cuenta, bool>> predicate)
        {
            return await _dbSet
                .Include(c => c.Cliente)
                .Where(predicate)
                .ToListAsync();
        }
    }
}

