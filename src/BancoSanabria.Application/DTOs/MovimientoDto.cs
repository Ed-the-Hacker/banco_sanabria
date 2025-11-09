using System.ComponentModel.DataAnnotations;

namespace BancoSanabria.Application.DTOs
{
    public class MovimientoDto
    {
        public int MovimientoId { get; set; }
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal Saldo { get; set; }
        public int CuentaId { get; set; }
        public string? NumeroCuenta { get; set; }
    }

    public class MovimientoCreateDto
    {
        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "El tipo de movimiento es requerido")]
        public string TipoMovimiento { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El valor es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El valor debe ser mayor a 0")]
        public decimal Valor { get; set; }
        
        [Required(ErrorMessage = "El ID de cuenta es requerido")]
        public int CuentaId { get; set; }
    }
}

