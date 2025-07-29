using Microsoft.EntityFrameworkCore;
using ApiBiblio.Database;
using ApiBiblio.Models;
using ApiBiblio.Services.Interfaces;
using static ApiBiblio.DTOs.GenreDTO;

namespace ApiBiblio.Services.Integrations
{
    public class GenreService : IGenreService
    {
        private readonly BiblioDb _context;
        private readonly ILogger<GenreService> _logger;

        public GenreService(BiblioDb context, ILogger<GenreService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<GenreResponseDto>> GetAllAsync()
        {
            try
            {
                var genres = await _context.Genres
                    .Select(g => new GenreResponseDto
                    {
                        Id = g.Id,
                        NomGenre = g.NomGenre,
                    })
                    .ToListAsync();

                return genres;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les genres");
                throw;
            }
        }

        public async Task<GenreResponseDto?> GetByIdAsync(int id)
        {
            try
            {
                var genre = await _context.Genres
                    .Where(g => g.Id == id)
                    .Select(g => new GenreResponseDto
                    {
                        Id = g.Id,
                        NomGenre = g.NomGenre,
                    })
                    .FirstOrDefaultAsync();

                return genre;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération du genre {id}");
                throw;
            }
        }

        public async Task<GenreResponseDto> CreateAsync(CreateGenreDto createGenreDto)
        {
            try
            {
                // Vérifier si le genre existe déjà
                var existingGenre = await _context.Genres
                    .FirstOrDefaultAsync(g => g.NomGenre == createGenreDto.NomGenre);

                if (existingGenre != null)
                {
                    throw new InvalidOperationException($"Le genre '{createGenreDto.NomGenre}' existe déjà");
                }

                var genre = new Genre
                {
                    NomGenre = createGenreDto.NomGenre,
                };

                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();

                return new GenreResponseDto
                {
                    Id = genre.Id,
                    NomGenre = genre.NomGenre,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du genre");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateGenreDto updateGenreDto)
        {
            try
            {
                var genre = await _context.Genres.FindAsync(id);
                if (genre == null)
                    return false;

                // Mettre à jour seulement les champs non nulls
                if (!string.IsNullOrWhiteSpace(updateGenreDto.NomGenre))
                    genre.NomGenre = updateGenreDto.NomGenre;

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la mise à jour du genre {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var genre = await _context.Genres.FindAsync(id);
                if (genre == null)
                    return false;

                // Vérifier s'il y a des livres associés
                var hasBooks = await _context.Livres.AnyAsync(l => l.IdGenre == id);
                if (hasBooks)
                {
                    throw new InvalidOperationException("Impossible de supprimer le genre car il a des livres associés");
                }

                _context.Genres.Remove(genre);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la suppression du genre {id}");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Genres.AnyAsync(g => g.Id == id);
        }
    }
}