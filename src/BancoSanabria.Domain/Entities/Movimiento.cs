namespace BancoSanabria.Domain.Entities
{
    /// <summary>
    /// Entidad Movimiento que representa una transacción bancaria
    /// </summary>
    public class Movimiento
    {
        public int MovimientoId { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal Saldo { get; set; } // Saldo después del movimiento

        // Relación con Cuenta
        public int CuentaId { get; set; }
        public Cuenta Cuenta { get; set; } = null!;
    }
}

