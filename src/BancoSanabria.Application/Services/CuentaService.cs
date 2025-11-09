using BancoSanabria.Application.DTOs;
using BancoSanabria.Domain.Entities;
using BancoSanabria.Domain.Exceptions;
using BancoSanabria.Infrastructure.UnitOfWork;

namespace BancoSanabria.Application.Services
{
    public class CuentaService : ICuentaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CuentaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CuentaDto>> GetAllAsync()
        {
            var cuentas = await _unitOfWork.Cuentas.GetAllAsync();
            
            return cuentas.Select(MapToDto).ToList();
        }

        public async Task<CuentaDto?> GetByIdAsync(int id)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(id);
            
            return cuenta == null ? null : MapToDto(cuenta);
        }

        public async Task<IEnumerable<CuentaDto>> GetByClienteIdAsync(int clienteId)
        {
            var cuentas = await _unitOfWork.Cuentas.GetCuentasByClienteIdAsync(clienteId);
            
            return cuentas.Select(MapToDto).ToList();
        }

        public async Task<CuentaDto> CreateAsync(CuentaCreateDto cuentaDto)
        {
            // Validar que el cliente existe
            var cliente = await _unitOfWork.Clientes.GetByIdAsync(cuentaDto.ClienteId);
            if (cliente == null)
                throw new BusinessException($"Cliente con ID {cuentaDto.ClienteId} no encontrado");

            // Validar que no exista una cuenta con el mismo número
            var existente = await _unitOfWork.Cuentas
                .GetByNumeroCuentaAsync(cuentaDto.NumeroCuenta);
            
            if (existente != null)
                throw new BusinessException($"Ya existe una cuenta con el número {cuentaDto.NumeroCuenta}");

            var cuenta = new Cuenta
            {
                NumeroCuenta = cuentaDto.NumeroCuenta,
                TipoCuenta = cuentaDto.TipoCuenta,
                SaldoInicial = cuentaDto.SaldoInicial,
                Estado = cuentaDto.Estado,
                ClienteId = cuentaDto.ClienteId
            };

            await _unitOfWork.Cuentas.AddAsync(cuenta);
            await _unitOfWork.SaveChangesAsync();

            // Recargar con cliente
            cuenta = await _unitOfWork.Cuentas.GetByIdAsync(cuenta.CuentaId);
            
            return MapToDto(cuenta!);
        }

        public async Task<CuentaDto> UpdateAsync(int id, CuentaUpdateDto cuentaDto)
        {
            var cuenta = await _unitOfWork.Cuentas.GetByIdAsync(id);
            
            if (cuenta == null)
                throw new BusinessException($"Cuenta con ID {id} no encontrada");

            if (!string.IsNullOrEmpty(cuentaDto.TipoCuenta))
                cuenta.TipoCuenta = cuentaDto.TipoCuenta;
            
            if (cuentaDto.SaldoInicial.HasValue)
                cuenta.SaldoInicial = cuentaDto.SaldoInicial.Value;
            
            if (cuentaDto.Estado.HasValue)
                cuenta.Estado = cuentaDto.Estado.Value;

            await _unitOfWork.Cuentas.UpdateAsync(cuenta);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(cuenta);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cuenta = await _unitOfWork.Cuentas.GetWithMovimientosAsync(id);
            
            if (cuenta == null)
                return false;

            // Validar que no tenga movimientos
            if (cuenta.Movimientos.Any())
                throw new BusinessException("No se puede eliminar una cuenta con movimientos registrados");

            await _unitOfWork.Cuentas.DeleteAsync(cuenta);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private static CuentaDto MapToDto(Cuenta cuenta)
        {
            return new CuentaDto
            {
                CuentaId = cuenta.CuentaId,
                NumeroCuenta = cuenta.NumeroCuenta,
                TipoCuenta = cuenta.TipoCuenta,
                SaldoInicial = cuenta.SaldoInicial,
                Estado = cuenta.Estado,
                ClienteId = cuenta.ClienteId,
                NombreCliente = cuenta.Cliente?.Nombre
            };
        }
    }
}

