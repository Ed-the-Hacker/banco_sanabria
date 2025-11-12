using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BancoSanabria.Domain.Entities;

namespace BancoSanabria.Application.Common.Interfaces;

public interface IMovimientoRepository : IRepository<Movimiento>
{
    Task<IEnumerable<Movimiento>> GetMovimientosByCuentaIdAsync(int cuentaId);
    Task<IEnumerable<Movimiento>> GetMovimientosByFechaRangoAsync(int cuentaId, DateTime fechaInicio, DateTime fechaFin);
    Task<decimal> GetSumaDebitosDelDiaAsync(int cuentaId, DateTime fecha);
    Task<Movimiento?> GetUltimoMovimientoAsync(int cuentaId);
}

