using System;
using System.Threading.Tasks;
using BancoSanabria.Application.DTOs;
using BancoSanabria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoSanabria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        /// <summary>
        /// Genera un reporte de estado de cuenta para un cliente en un rango de fechas
        /// </summary>
        /// <param name="fechaInicio">Fecha de inicio del reporte</param>
        /// <param name="fechaFin">Fecha de fin del reporte</param>
        /// <param name="clienteId">ID del cliente</param>
        /// <returns>Reporte en formato JSON con PDF en Base64</returns>
        [HttpGet]
        public async Task<ActionResult<ReporteDto>> GenerarReporte(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin,
            [FromQuery] int clienteId)
        {
            var request = new ReporteRequestDto
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                ClienteId = clienteId
            };

            var reporte = await _reporteService.GenerarReporteAsync(request);
            return Ok(reporte);
        }

        /// <summary>
        /// Genera un reporte (alternativa con POST para body)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ReporteDto>> GenerarReportePost([FromBody] ReporteRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reporte = await _reporteService.GenerarReporteAsync(request);
            return Ok(reporte);
        }
    }
}

