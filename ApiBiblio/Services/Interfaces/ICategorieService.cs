using static ApiBiblio.DTOs.CategorieDTO;

namespace ApiBiblio.Services.Interfaces
{
    public interface ICategorieService
    {
        Task<IEnumerable<CategorieResponseDto>> GetAllAsync();
        Task<CategorieResponseDto?> GetByIdAsync(int id);
        Task<CategorieResponseDto> CreateAsync(CreateCategorieDto createCategorieDto);
        Task<bool> UpdateAsync(int id, UpdateCategorieDto updateCategorieDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}