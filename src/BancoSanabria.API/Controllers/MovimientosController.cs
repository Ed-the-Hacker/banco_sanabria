using BancoSanabria.Application.DTOs;
using BancoSanabria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoSanabria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientosController : ControllerBase
    {
        private readonly IMovimientoService _movimientoService;

        public MovimientosController(IMovimientoService movimientoService)
        {
            _movimientoService = movimientoService;
        }

        /// <summary>
        /// Obtiene todos los movimientos
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientoDto>>> GetAll()
        {
            var movimientos = await _movimientoService.GetAllAsync();
            return Ok(movimientos);
        }

        /// <summary>
        /// Obtiene un movimiento por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientoDto>> GetById(int id)
        {
            var movimiento = await _movimientoService.GetByIdAsync(id);
            
            if (movimiento == null)
                return NotFound(new { mensaje = $"Movimiento con ID {id} no encontrado" });

            return Ok(movimiento);
        }

        /// <summary>
        /// Obtiene los movimientos de una cuenta
        /// </summary>
        [HttpGet("cuenta/{cuentaId}")]
        public async Task<ActionResult<IEnumerable<MovimientoDto>>> GetByCuentaId(int cuentaId)
        {
            var movimientos = await _movimientoService.GetByCuentaIdAsync(cuentaId);
            return Ok(movimientos);
        }

        /// <summary>
        /// Registra un nuevo movimiento (transacción)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MovimientoDto>> Create([FromBody] MovimientoCreateDto movimientoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var movimiento = await _movimientoService.CreateAsync(movimientoDto);
            return CreatedAtAction(nameof(GetById), new { id = movimiento.MovimientoId }, movimiento);
        }

        /// <summary>
        /// Elimina un movimiento (solo el último)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var resultado = await _movimientoService.DeleteAsync(id);
            
            if (!resultado)
                return NotFound(new { mensaje = $"Movimiento con ID {id} no encontrado" });

            return NoContent();
        }
    }
}

