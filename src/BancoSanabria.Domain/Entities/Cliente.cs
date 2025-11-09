namespace BancoSanabria.Domain.Entities
{
    /// <summary>
    /// Entidad Cliente que hereda de Persona y representa un cliente bancario
    /// </summary>
    public class Cliente : Persona
    {
        public int ClienteId { get; set; }
        public string Contrasena { get; set; } = string.Empty;
        public bool Estado { get; set; }

        // Relación: Un cliente puede tener múltiples cuentas
        public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
    }
}

