using System.ComponentModel.DataAnnotations;

namespace BancoSanabria.Application.DTOs
{
    public class ClienteDto
    {
        public int ClienteId { get; set; }
        
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El género es requerido")]
        public string Genero { get; set; } = string.Empty;
        
        [Range(18, 120, ErrorMessage = "La edad debe estar entre 18 y 120 años")]
        public int Edad { get; set; }
        
        [Required(ErrorMessage = "La identificación es requerida")]
        [MaxLength(20)]
        public string Identificacion { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(4, ErrorMessage = "La contraseña debe tener al menos 4 caracteres")]
        public string Contrasena { get; set; } = string.Empty;
        
        public bool Estado { get; set; }
    }

    public class ClienteCreateDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "El género es requerido")]
        public string Genero { get; set; } = string.Empty;
        
        [Range(18, 120, ErrorMessage = "La edad debe estar entre 18 y 120 años")]
        public int Edad { get; set; }
        
        [Required(ErrorMessage = "La identificación es requerida")]
        [MaxLength(20)]
        public string Identificacion { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(4, ErrorMessage = "La contraseña debe tener al menos 4 caracteres")]
        public string Contrasena { get; set; } = string.Empty;
        
        public bool Estado { get; set; } = true;
    }

    public class ClienteUpdateDto
    {
        [MaxLength(100)]
        public string? Nombre { get; set; }
        
        public string? Genero { get; set; }
        
        [Range(18, 120)]
        public int? Edad { get; set; }
        
        [MaxLength(200)]
        public string? Direccion { get; set; }
        
        [MaxLength(20)]
        public string? Telefono { get; set; }
        
        [MinLength(4)]
        public string? Contrasena { get; set; }
        
        public bool? Estado { get; set; }
    }
}

