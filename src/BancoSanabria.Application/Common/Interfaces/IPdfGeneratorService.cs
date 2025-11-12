using System.Threading.Tasks;
using BancoSanabria.Application.DTOs;

namespace BancoSanabria.Application.Common.Interfaces;

public interface IPdfGeneratorService
{
    Task<string> GenerarPdfReporteAsync(ReporteDto reporte, ReporteRequestDto request);
}

