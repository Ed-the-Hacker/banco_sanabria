namespace BancoSanabria.Domain.Entities
{
    /// <summary>
    /// Entidad Cuenta que representa una cuenta bancaria
    /// </summary>
    public class Cuenta
    {
        public int CuentaId { get; set; }
        public string NumeroCuenta { get; set; } = string.Empty;
        public string TipoCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public bool Estado { get; set; }

        // Relación con Cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        // Relación: Una cuenta puede tener múltiples movimientos
        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    }
}

