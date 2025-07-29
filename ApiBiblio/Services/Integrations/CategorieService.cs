using Microsoft.EntityFrameworkCore;
using ApiBiblio.Database;
using ApiBiblio.Models;
using ApiBiblio.Services.Interfaces;
using static ApiBiblio.DTOs.CategorieDTO;

namespace ApiBiblio.Services.Integrations
{
    public class CategorieService : ICategorieService
    {
        private readonly BiblioDb _context;
        private readonly ILogger<CategorieService> _logger;

        public CategorieService(BiblioDb context, ILogger<CategorieService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CategorieResponseDto>> GetAllAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new CategorieResponseDto
                    {
                        Id = c.Id,
                        NomCategorie = c.NomCategorie,
                    })
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de toutes les catégories");
                throw;
            }
        }

        public async Task<CategorieResponseDto?> GetByIdAsync(int id)
        {
            try
            {
                var categorie = await _context.Categories
                    .Where(c => c.Id == id)
                    .Select(c => new CategorieResponseDto
                    {
                        Id = c.Id,
                        NomCategorie = c.NomCategorie,
                    })
                    .FirstOrDefaultAsync();

                return categorie;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération de la catégorie {id}");
                throw;
            }
        }

        public async Task<CategorieResponseDto> CreateAsync(CreateCategorieDto createCategorieDto)
        {
            try
            {
                // Vérifier si la catégorie existe déjà
                var existingCategorie = await _context.Categories
                    .FirstOrDefaultAsync(c => c.NomCategorie == createCategorieDto.NomCategorie);

                if (existingCategorie != null)
                {
                    throw new InvalidOperationException($"La catégorie '{createCategorieDto.NomCategorie}' existe déjà");
                }

                var categorie = new Categorie
                {
                    NomCategorie = createCategorieDto.NomCategorie,
                };

                _context.Categories.Add(categorie);
                await _context.SaveChangesAsync();

                return new CategorieResponseDto
                {
                    Id = categorie.Id,
                    NomCategorie = categorie.NomCategorie,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de la catégorie");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategorieDto updateCategorieDto)
        {
            try
            {
                var categorie = await _context.Categories.FindAsync(id);
                if (categorie == null)
                    return false;

                // Mettre à jour seulement les champs non nulls
                if (!string.IsNullOrWhiteSpace(updateCategorieDto.NomCategorie))
                    categorie.NomCategorie = updateCategorieDto.NomCategorie;

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la mise à jour de la catégorie {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var categorie = await _context.Categories.FindAsync(id);
                if (categorie == null)
                    return false;

                // Vérifier s'il y a des livres associés
                var hasBooks = await _context.Livres.AnyAsync(l => l.IdCategorie == id);
                if (hasBooks)
                {
                    throw new InvalidOperationException("Impossible de supprimer la catégorie car elle a des livres associés");
                }

                _context.Categories.Remove(categorie);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la suppression de la catégorie {id}");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Categories.AnyAsync(c => c.Id == id);
        }
    }
}