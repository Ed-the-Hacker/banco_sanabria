using BancoSanabria.Application.DTOs;
using BancoSanabria.Domain.Entities;
using BancoSanabria.Domain.Exceptions;
using BancoSanabria.Infrastructure.UnitOfWork;

namespace BancoSanabria.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClienteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ClienteDto>> GetAllAsync()
        {
            var clientes = await _unitOfWork.Clientes.GetAllAsync();
            
            // Uso de LINQ (programación funcional)
            return clientes.Select(c => new ClienteDto
            {
                ClienteId = c.ClienteId,
                Nombre = c.Nombre,
                Genero = c.Genero,
                Edad = c.Edad,
                Identificacion = c.Identificacion,
                Direccion = c.Direccion,
                Telefono = c.Telefono,
                Contrasena = c.Contrasena,
                Estado = c.Estado
            }).ToList();
        }

        public async Task<ClienteDto?> GetByIdAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
            
            if (cliente == null)
                return null;

            return new ClienteDto
            {
                ClienteId = cliente.ClienteId,
                Nombre = cliente.Nombre,
                Genero = cliente.Genero,
                Edad = cliente.Edad,
                Identificacion = cliente.Identificacion,
                Direccion = cliente.Direccion,
                Telefono = cliente.Telefono,
                Contrasena = cliente.Contrasena,
                Estado = cliente.Estado
            };
        }

        public async Task<ClienteDto> CreateAsync(ClienteCreateDto clienteDto)
        {
            // Validar que no exista un cliente con la misma identificación
            var existente = await _unitOfWork.Clientes
                .GetByIdentificacionAsync(clienteDto.Identificacion);
            
            if (existente != null)
                throw new BusinessException($"Ya existe un cliente con la identificación {clienteDto.Identificacion}");

            var cliente = new Cliente
            {
                Nombre = clienteDto.Nombre,
                Genero = clienteDto.Genero,
                Edad = clienteDto.Edad,
                Identificacion = clienteDto.Identificacion,
                Direccion = clienteDto.Direccion,
                Telefono = clienteDto.Telefono,
                Contrasena = clienteDto.Contrasena, // TODO: Encriptar contraseña
                Estado = clienteDto.Estado
            };

            await _unitOfWork.Clientes.AddAsync(cliente);
            await _unitOfWork.SaveChangesAsync();

            return new ClienteDto
            {
                ClienteId = cliente.ClienteId,
                Nombre = cliente.Nombre,
                Genero = cliente.Genero,
                Edad = cliente.Edad,
                Identificacion = cliente.Identificacion,
                Direccion = cliente.Direccion,
                Telefono = cliente.Telefono,
                Contrasena = cliente.Contrasena,
                Estado = cliente.Estado
            };
        }

        public async Task<ClienteDto> UpdateAsync(int id, ClienteUpdateDto clienteDto)
        {
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(id);
            
            if (cliente == null)
                throw new BusinessException($"Cliente con ID {id} no encontrado");

            // Actualizar solo los campos que no son nulos (PATCH)
            if (!string.IsNullOrEmpty(clienteDto.Nombre))
                cliente.Nombre = clienteDto.Nombre;
            
            if (!string.IsNullOrEmpty(clienteDto.Genero))
                cliente.Genero = clienteDto.Genero;
            
            if (clienteDto.Edad.HasValue)
                cliente.Edad = clienteDto.Edad.Value;
            
            if (!string.IsNullOrEmpty(clienteDto.Direccion))
                cliente.Direccion = clienteDto.Direccion;
            
            if (!string.IsNullOrEmpty(clienteDto.Telefono))
                cliente.Telefono = clienteDto.Telefono;
            
            if (!string.IsNullOrEmpty(clienteDto.Contrasena))
                cliente.Contrasena = clienteDto.Contrasena; // TODO: Encriptar contraseña
            
            if (clienteDto.Estado.HasValue)
                cliente.Estado = clienteDto.Estado.Value;

            await _unitOfWork.Clientes.UpdateAsync(cliente);
            await _unitOfWork.SaveChangesAsync();

            return new ClienteDto
            {
                ClienteId = cliente.ClienteId,
                Nombre = cliente.Nombre,
                Genero = cliente.Genero,
                Edad = cliente.Edad,
                Identificacion = cliente.Identificacion,
                Direccion = cliente.Direccion,
                Telefono = cliente.Telefono,
                Contrasena = cliente.Contrasena,
                Estado = cliente.Estado
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _unitOfWork.Clientes.GetWithCuentasAsync(id);
            
            if (cliente == null)
                return false;

            // Validar que el cliente no tenga cuentas activas
            if (cliente.Cuentas.Any(c => c.Estado))
                throw new BusinessException("No se puede eliminar un cliente con cuentas activas");

            await _unitOfWork.Clientes.DeleteAsync(cliente);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}

