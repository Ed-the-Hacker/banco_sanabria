using System;
using System.Linq.Expressions;
using BancoSanabria.Application.Common.Interfaces;
using BancoSanabria.Domain.Entities;
using BancoSanabria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BancoSanabria.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de Movimiento
    /// </summary>
    public class MovimientoRepository : Repository<Movimiento>, IMovimientoRepository
    {
        public MovimientoRepository(BancoDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Movimiento>> GetMovimientosByCuentaIdAsync(int cuentaId)
        {
            return await _dbSet
                .Include(m => m.Cuenta)
                .Where(m => m.CuentaId == cuentaId)
                .OrderBy(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movimiento>> GetMovimientosByFechaRangoAsync(
            int cuentaId, 
            DateTime fechaInicio, 
            DateTime fechaFin)
        {
            var fechaInicioDia = fechaInicio.Date;
            var fechaFinDia = fechaFin.Date.AddDays(1).AddTicks(-1);

            return await _dbSet
                .Include(m => m.Cuenta)
                .Where(m => m.CuentaId == cuentaId 
                    && m.Fecha >= fechaInicioDia
                    && m.Fecha <= fechaFinDia)
                .OrderBy(m => m.Fecha)
                .ToListAsync();
        }

        public async Task<decimal> GetSumaDebitosDelDiaAsync(int cuentaId, DateTime fecha)
        {
            var fechaInicio = fecha.Date;
            var fechaFin = fecha.Date.AddDays(1).AddTicks(-1);

            return await _dbSet
                .Where(m => m.CuentaId == cuentaId 
                    && m.Fecha >= fechaInicio 
                    && m.Fecha <= fechaFin
                    && m.TipoMovimiento == "Debito")
                .SumAsync(m => Math.Abs(m.Valor)); // Valor absoluto del débito
        }

        public async Task<Movimiento?> GetUltimoMovimientoAsync(int cuentaId)
        {
            return await _dbSet
                .Where(m => m.CuentaId == cuentaId)
                .OrderByDescending(m => m.Fecha)
                .ThenByDescending(m => m.MovimientoId)
                .FirstOrDefaultAsync();
        }
    }
}

