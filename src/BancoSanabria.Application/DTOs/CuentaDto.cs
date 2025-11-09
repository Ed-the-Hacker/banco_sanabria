using System.ComponentModel.DataAnnotations;

namespace BancoSanabria.Application.DTOs
{
    public class CuentaDto
    {
        public int CuentaId { get; set; }
        
        [Required(ErrorMessage = "El número de cuenta es requerido")]
        [MaxLength(20)]
        public string NumeroCuenta { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El tipo de cuenta es requerido")]
        public string TipoCuenta { get; set; } = string.Empty;
        
        public decimal SaldoInicial { get; set; }
        
        public bool Estado { get; set; }
        
        public int ClienteId { get; set; }
        
        public string? NombreCliente { get; set; }
    }

    public class CuentaCreateDto
    {
        [Required(ErrorMessage = "El número de cuenta es requerido")]
        [MaxLength(20)]
        public string NumeroCuenta { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El tipo de cuenta es requerido")]
        public string TipoCuenta { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue, ErrorMessage = "El saldo inicial debe ser mayor o igual a 0")]
        public decimal SaldoInicial { get; set; }
        
        public bool Estado { get; set; } = true;
        
        [Required(ErrorMessage = "El ID del cliente es requerido")]
        public int ClienteId { get; set; }
    }

    public class CuentaUpdateDto
    {
        public string? TipoCuenta { get; set; }
        
        public decimal? SaldoInicial { get; set; }
        
        public bool? Estado { get; set; }
    }
}

