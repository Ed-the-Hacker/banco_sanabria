using BancoSanabria.Domain.Exceptions;

namespace BancoSanabria.Application.Strategies
{
    /// <summary>
    /// Factory para obtener la estrategia correcta según el tipo de movimiento
    /// </summary>
    public class MovimientoStrategyFactory
    {
        private readonly IEnumerable<IMovimientoStrategy> _strategies;

        public MovimientoStrategyFactory(IEnumerable<IMovimientoStrategy> strategies)
        {
            _strategies = strategies;
        }

        public IMovimientoStrategy GetStrategy(string tipoMovimiento)
        {
            var strategy = _strategies.FirstOrDefault(s => 
                s.TipoMovimiento.Equals(tipoMovimiento, StringComparison.OrdinalIgnoreCase));

            if (strategy == null)
            {
                throw new BusinessException($"Tipo de movimiento '{tipoMovimiento}' no válido. Use 'Credito' o 'Debito'");
            }

            return strategy;
        }
    }
}

