using BancoSanabria.Application.DTOs;
using BancoSanabria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoSanabria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuentasController : ControllerBase
    {
        private readonly ICuentaService _cuentaService;

        public CuentasController(ICuentaService cuentaService)
        {
            _cuentaService = cuentaService;
        }

        /// <summary>
        /// Obtiene todas las cuentas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CuentaDto>>> GetAll()
        {
            var cuentas = await _cuentaService.GetAllAsync();
            return Ok(cuentas);
        }

        /// <summary>
        /// Obtiene una cuenta por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CuentaDto>> GetById(int id)
        {
            var cuenta = await _cuentaService.GetByIdAsync(id);
            
            if (cuenta == null)
                return NotFound(new { mensaje = $"Cuenta con ID {id} no encontrada" });

            return Ok(cuenta);
        }

        /// <summary>
        /// Obtiene las cuentas de un cliente
        /// </summary>
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<CuentaDto>>> GetByClienteId(int clienteId)
        {
            var cuentas = await _cuentaService.GetByClienteIdAsync(clienteId);
            return Ok(cuentas);
        }

        /// <summary>
        /// Crea una nueva cuenta
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CuentaDto>> Create([FromBody] CuentaCreateDto cuentaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cuenta = await _cuentaService.CreateAsync(cuentaDto);
            return CreatedAtAction(nameof(GetById), new { id = cuenta.CuentaId }, cuenta);
        }

        /// <summary>
        /// Actualiza una cuenta existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CuentaDto>> Update(int id, [FromBody] CuentaUpdateDto cuentaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cuenta = await _cuentaService.UpdateAsync(id, cuentaDto);
            return Ok(cuenta);
        }

        /// <summary>
        /// Actualiza parcialmente una cuenta
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<CuentaDto>> Patch(int id, [FromBody] CuentaUpdateDto cuentaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cuenta = await _cuentaService.UpdateAsync(id, cuentaDto);
            return Ok(cuenta);
        }

        /// <summary>
        /// Elimina una cuenta
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var resultado = await _cuentaService.DeleteAsync(id);
            
            if (!resultado)
                return NotFound(new { mensaje = $"Cuenta con ID {id} no encontrada" });

            return NoContent();
        }
    }
}

