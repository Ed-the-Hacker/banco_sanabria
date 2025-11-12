using System.Collections.Generic;
using System.Threading.Tasks;
using BancoSanabria.Domain.Entities;

namespace BancoSanabria.Application.Common.Interfaces;

public interface IClienteRepository : IRepository<Cliente>
{
    Task<Cliente?> GetByIdentificacionAsync(string identificacion);
    Task<Cliente?> GetWithCuentasAsync(int clienteId);
}

