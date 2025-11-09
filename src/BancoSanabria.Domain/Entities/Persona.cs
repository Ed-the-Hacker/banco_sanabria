namespace BancoSanabria.Domain.Entities
{
    /// <summary>
    /// Clase base que representa una Persona en el sistema
    /// </summary>
    public abstract class Persona
    {
        public string Nombre { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public int Edad { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }
}

