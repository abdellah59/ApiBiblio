using Microsoft.EntityFrameworkCore;
using ApiBiblio.Database;
using ApiBiblio.Models;
using ApiBiblio.Services.Interfaces;
using static ApiBiblio.DTOs.AuteurDTO;

namespace ApiBiblio.Services.Integrations
{
    public class AuteurService : IAuteurService
    {
        private readonly BiblioDb _context;
        private readonly ILogger<AuteurService> _logger;

        public AuteurService(BiblioDb context, ILogger<AuteurService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<AuteurResponseDto>> GetAllAsync()
        {
            try
            {
                var auteurs = await _context.Auteurs
                    .Select(a => new AuteurResponseDto
                    {
                        Id = a.Id,
                        NomAuteur = a.NomAuteur,
                        PrenomAuteur = a.PrenomAuteur,
                       
                    })
                    .ToListAsync();

                return auteurs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de tous les auteurs");
                throw;
            }
        }

        public async Task<AuteurResponseDto?> GetByIdAsync(int id)
        {
            try
            {
                var auteur = await _context.Auteurs
                    .Where(a => a.Id == id)
                    .Select(a => new AuteurResponseDto
                    {
                        Id = a.Id,
                        NomAuteur = a.NomAuteur,
                        PrenomAuteur = a.PrenomAuteur,
                    })
                    .FirstOrDefaultAsync();

                return auteur;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération de l'auteur {id}");
                throw;
            }
        }

        public async Task<AuteurResponseDto> CreateAsync(CreateAuteurDto createAuteurDto)
        {
            try
            {
                // Vérifier si l'auteur existe déjà
                var existingAuteur = await _context.Auteurs
                    .FirstOrDefaultAsync(a => a.NomAuteur == createAuteurDto.NomAuteur
                                           && a.PrenomAuteur == createAuteurDto.PrenomAuteur);

                if (existingAuteur != null)
                {
                    throw new InvalidOperationException($"L'auteur {createAuteurDto.PrenomAuteur} {createAuteurDto.NomAuteur} existe déjà");
                }

                var auteur = new Auteur
                {
                    NomAuteur = createAuteurDto.NomAuteur,
                    PrenomAuteur = createAuteurDto.PrenomAuteur,
                    
                };

                _context.Auteurs.Add(auteur);
                await _context.SaveChangesAsync();

                return new AuteurResponseDto
                {
                    Id = auteur.Id,
                    NomAuteur = auteur.NomAuteur,
                    PrenomAuteur = auteur.PrenomAuteur,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de l'auteur");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateAuteurDto updateAuteurDto)
        {
            try
            {
                var auteur = await _context.Auteurs.FindAsync(id);
                if (auteur == null)
                    return false;

                // Mettre à jour seulement les champs non nulls
                if (!string.IsNullOrWhiteSpace(updateAuteurDto.NomAuteur))
                    auteur.NomAuteur = updateAuteurDto.NomAuteur;

                if (!string.IsNullOrWhiteSpace(updateAuteurDto.PrenomAuteur))
                    auteur.PrenomAuteur = updateAuteurDto.PrenomAuteur;

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la mise à jour de l'auteur {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var auteur = await _context.Auteurs.FindAsync(id);
                if (auteur == null)
                    return false;

                // Vérifier s'il y a des livres associés
                var hasBooks = await _context.Livres.AnyAsync(l => l.IdAuteur == id);
                if (hasBooks)
                {
                    throw new InvalidOperationException("Impossible de supprimer l'auteur car il a des livres associés");
                }

                _context.Auteurs.Remove(auteur);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la suppression de l'auteur {id}");
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Auteurs.AnyAsync(a => a.Id == id);
        }
    }
}
