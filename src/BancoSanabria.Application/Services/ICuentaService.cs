using BancoSanabria.Application.DTOs;

namespace BancoSanabria.Application.Services
{
    public interface ICuentaService
    {
        Task<IEnumerable<CuentaDto>> GetAllAsync();
        Task<CuentaDto?> GetByIdAsync(int id);
        Task<IEnumerable<CuentaDto>> GetByClienteIdAsync(int clienteId);
        Task<CuentaDto> CreateAsync(CuentaCreateDto cuentaDto);
        Task<CuentaDto> UpdateAsync(int id, CuentaUpdateDto cuentaDto);
        Task<bool> DeleteAsync(int id);
    }
}

