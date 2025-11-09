using BancoSanabria.Application.DTOs;

namespace BancoSanabria.Application.Services
{
    public interface IPdfGeneratorService
    {
        Task<string> GenerarPdfReporteAsync(ReporteDto reporte, ReporteRequestDto request);
    }
}

