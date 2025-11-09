using BancoSanabria.Application.DTOs;

namespace BancoSanabria.Application.Services
{
    public interface IReporteService
    {
        Task<ReporteDto> GenerarReporteAsync(ReporteRequestDto request);
    }
}

