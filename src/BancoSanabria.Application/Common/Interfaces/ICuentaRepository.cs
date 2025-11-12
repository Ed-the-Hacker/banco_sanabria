using System.Collections.Generic;
using System.Threading.Tasks;
using BancoSanabria.Domain.Entities;

namespace BancoSanabria.Application.Common.Interfaces;

public interface ICuentaRepository : IRepository<Cuenta>
{
    Task<Cuenta?> GetByNumeroCuentaAsync(string numeroCuenta);
    Task<IEnumerable<Cuenta>> GetCuentasByClienteIdAsync(int clienteId);
    Task<Cuenta?> GetWithMovimientosAsync(int cuentaId);
}

