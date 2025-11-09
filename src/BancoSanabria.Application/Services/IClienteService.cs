using BancoSanabria.Application.DTOs;

namespace BancoSanabria.Application.Services
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDto>> GetAllAsync();
        Task<ClienteDto?> GetByIdAsync(int id);
        Task<ClienteDto> CreateAsync(ClienteCreateDto clienteDto);
        Task<ClienteDto> UpdateAsync(int id, ClienteUpdateDto clienteDto);
        Task<bool> DeleteAsync(int id);
    }
}

