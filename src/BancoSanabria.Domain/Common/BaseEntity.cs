namespace BancoSanabria.Domain.Common
{
    /// <summary>
    /// Clase base para auditoría de entidades
    /// </summary>
    public abstract class BaseEntity
    {
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}

