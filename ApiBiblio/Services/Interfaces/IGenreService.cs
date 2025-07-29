using static ApiBiblio.DTOs.GenreDTO;

namespace ApiBiblio.Services.Interfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreResponseDto>> GetAllAsync();
        Task<GenreResponseDto?> GetByIdAsync(int id);
        Task<GenreResponseDto> CreateAsync(CreateGenreDto createGenreDto);
        Task<bool> UpdateAsync(int id, UpdateGenreDto updateGenreDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}