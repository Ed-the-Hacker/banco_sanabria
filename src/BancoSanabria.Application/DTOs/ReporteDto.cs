namespace BancoSanabria.Application.DTOs
{
    public class ReporteDto
    {
        public ClienteReporteDto Cliente { get; set; } = new();
        public List<CuentaReporteDto> Cuentas { get; set; } = new();
        public string PdfBase64 { get; set; } = string.Empty;
    }

    public class ClienteReporteDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Identificacion { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
    }

    public class CuentaReporteDto
    {
        public string NumeroCuenta { get; set; } = string.Empty;
        public string TipoCuenta { get; set; } = string.Empty;
        public decimal SaldoInicial { get; set; }
        public decimal SaldoDisponible { get; set; }
        public bool Estado { get; set; }
        public List<MovimientoReporteDto> Movimientos { get; set; } = new();
        public decimal TotalCreditos { get; set; }
        public decimal TotalDebitos { get; set; }
    }

    public class MovimientoReporteDto
    {
        public DateTime Fecha { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal Saldo { get; set; }
    }

    public class ReporteRequestDto
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int ClienteId { get; set; }
    }
}

