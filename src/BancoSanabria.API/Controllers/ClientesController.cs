using BancoSanabria.Application.DTOs;
using BancoSanabria.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoSanabria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        /// <summary>
        /// Obtiene todos los clientes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDto>>> GetAll()
        {
            var clientes = await _clienteService.GetAllAsync();
            return Ok(clientes);
        }

        /// <summary>
        /// Obtiene un cliente por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetById(int id)
        {
            var cliente = await _clienteService.GetByIdAsync(id);
            
            if (cliente == null)
                return NotFound(new { mensaje = $"Cliente con ID {id} no encontrado" });

            return Ok(cliente);
        }

        /// <summary>
        /// Crea un nuevo cliente
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> Create([FromBody] ClienteCreateDto clienteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = await _clienteService.CreateAsync(clienteDto);
            return CreatedAtAction(nameof(GetById), new { id = cliente.ClienteId }, cliente);
        }

        /// <summary>
        /// Actualiza un cliente existente (PUT completo)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ClienteDto>> Update(int id, [FromBody] ClienteUpdateDto clienteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = await _clienteService.UpdateAsync(id, clienteDto);
            return Ok(cliente);
        }

        /// <summary>
        /// Actualiza parcialmente un cliente (PATCH)
        /// </summary>
        [HttpPatch("{id}")]
        public async Task<ActionResult<ClienteDto>> Patch(int id, [FromBody] ClienteUpdateDto clienteDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = await _clienteService.UpdateAsync(id, clienteDto);
            return Ok(cliente);
        }

        /// <summary>
        /// Elimina un cliente
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var resultado = await _clienteService.DeleteAsync(id);
            
            if (!resultado)
                return NotFound(new { mensaje = $"Cliente con ID {id} no encontrado" });

            return NoContent();
        }
    }
}

