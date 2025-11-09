using BancoSanabria.Domain.Entities;
using BancoSanabria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoSanabria.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Cliente
    /// </summary>
    public class ClienteRepository : Repository<Cliente>, IClienteRepository
    {
        public ClienteRepository(BancoDbContext context) : base(context)
        {
        }

        public async Task<Cliente?> GetByIdentificacionAsync(string identificacion)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Identificacion == identificacion);
        }

        public async Task<Cliente?> GetWithCuentasAsync(int clienteId)
        {
            return await _dbSet
                .Include(c => c.Cuentas)
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId);
        }

        public override async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Cuentas)
                .FirstOrDefaultAsync(c => c.ClienteId == id);
        }
    }
}

