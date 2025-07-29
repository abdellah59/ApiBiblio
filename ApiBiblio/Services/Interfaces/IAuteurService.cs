using static ApiBiblio.DTOs.AuteurDTO;

namespace ApiBiblio.Services.Interfaces
{
    public interface IAuteurService
    {
        Task<IEnumerable<AuteurResponseDto>> GetAllAsync();
        Task<AuteurResponseDto?> GetByIdAsync(int id);
        Task<AuteurResponseDto> CreateAsync(CreateAuteurDto createAuteurDto);
        Task<bool> UpdateAsync(int id, UpdateAuteurDto updateAuteurDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
