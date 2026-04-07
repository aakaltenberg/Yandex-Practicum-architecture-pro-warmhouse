using GateService.Controllers;
using GateService.Models;

namespace GateService.Services
{
    public interface IGateService
    {
        Task<List<Gate>> GetAllAsync();
        Task<Gate?> GetByIdAsync(int id);
        Task<Gate> CreateAsync(GateCreateDto dto);
        Task<Gate?> UpdateAsync(int id, GateUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> OpenGateAsync(int id);
        Task<bool> CloseGateAsync(int id);
    }
}
