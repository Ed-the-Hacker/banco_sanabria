using BancoSanabria.Application.DTOs;

namespace BancoSanabria.Application.Services
{
    public interface IMovimientoService
    {
        Task<IEnumerable<MovimientoDto>> GetAllAsync();
        Task<MovimientoDto?> GetByIdAsync(int id);
        Task<IEnumerable<MovimientoDto>> GetByCuentaIdAsync(int cuentaId);
        Task<MovimientoDto> CreateAsync(MovimientoCreateDto movimientoDto);
        Task<bool> DeleteAsync(int id);
    }
}

