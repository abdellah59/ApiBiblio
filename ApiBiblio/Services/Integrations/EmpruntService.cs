using Microsoft.EntityFrameworkCore;
using ApiBiblio.Database;
using ApiBiblio.DTOs;
using ApiBiblio.Models;
using ApiBiblio.Services.Interfaces;

namespace ApiBiblio.Services.Integrations
{
    public class EmpruntService : IEmpruntService
    {
        private readonly BiblioDb _context;
        private readonly ILivreService _livreService;

        public EmpruntService(BiblioDb context, ILivreService livreService)
        {
            _context = context;
            _livreService = livreService;
        }

        public async Task<IEnumerable<EmpruntDTO>> GetAllAsync()
        {
            var emprunts = await _context.Emprunts
                .Include(e => e.Membre)
                .Include(e => e.Employe)
                .Include(e => e.EmpruntLivres)
                    .ThenInclude(el => el.Livre)
                .OrderByDescending(e => e.DateEmprunt)
                .ToListAsync();

            return emprunts.Select(e => new EmpruntDTO
            {
                Id = e.Id,
                DateEmprunt = e.DateEmprunt,
                Statut = e.Statut,
                DateRetour = e.DateRetour,
                Membre = $"{e.Membre.PrenomMembre} {e.Membre.NomMembre}",
                Employe = $"{e.Employe.PrenomEmploye} {e.Employe.NomEmploye}",
                Livres = e.EmpruntLivres.Select(el => el.Livre.Titre).ToList()
            });
        }

        public async Task<EmpruntDTO> GetByIdAsync(int id)
        {
            var emprunt = await _context.Emprunts
                .Include(e => e.Membre)
                .Include(e => e.Employe)
                .Include(e => e.EmpruntLivres)
                    .ThenInclude(el => el.Livre)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (emprunt == null)
                throw new KeyNotFoundException($"Emprunt avec l'ID {id} non trouvé");

            return new EmpruntDTO
            {
                Id = emprunt.Id,
                DateEmprunt = emprunt.DateEmprunt,
                Statut = emprunt.Statut,
                DateRetour = emprunt.DateRetour,
                Membre = $"{emprunt.Membre.PrenomMembre} {emprunt.Membre.NomMembre}",
                Employe = $"{emprunt.Employe.PrenomEmploye} {emprunt.Employe.NomEmploye}",
                Livres = emprunt.EmpruntLivres.Select(el => el.Livre.Titre).ToList()
            };
        }

        public async Task<EmpruntDTO> CreateAsync(CreateEmpruntDto createEmpruntDTO, int employeId)
        {
            // Vérifier la disponibilité des livres
            foreach (var livreId in createEmpruntDTO.IdLivres)
            {
                var livre = await _context.Livres.FindAsync(livreId);
                if (livre == null)
                    throw new KeyNotFoundException($"Livre avec l'ID {livreId} non trouvé");

                if (!livre.Disponible)
                    throw new InvalidOperationException($"Le livre '{livre.Titre}' n'est pas disponible");
            }

            // Créer l'emprunt
            var emprunt = new Emprunt
            {
                DateEmprunt = DateTime.UtcNow,          
                Statut = "En cours",
                IdMembre = createEmpruntDTO.IdMembre,
                IdEmploye = employeId
            };

            _context.Emprunts.Add(emprunt);
            await _context.SaveChangesAsync();

            // Ajouter les livres à l'emprunt et mettre à jour leur disponibilité
            foreach (var livreId in createEmpruntDTO.IdLivres)
            {
                var empruntLivre = new EmpruntLivre
                {
                    IdEmprunt = emprunt.Id,
                    IdLivre = livreId
                };

                _context.EmpruntLivres.Add(empruntLivre);
                await _livreService.UpdateAvailabilityAsync(livreId, false);
            }

            await _context.SaveChangesAsync();
            return await GetByIdAsync(emprunt.Id);
        }

        public async Task<bool> RetournerLivresAsync(int empruntId, int employeId)
        {
            var emprunt = await _context.Emprunts
                .Include(e => e.EmpruntLivres)
                .FirstOrDefaultAsync(e => e.Id == empruntId);

            if (emprunt == null)
                return false;

            if (emprunt.Statut != "En cours")
                throw new InvalidOperationException("Cet emprunt n'est pas en cours");

            // Mettre à jour le statut de l'emprunt
            emprunt.Statut = "Terminé";
            emprunt.DateRetour = DateTime.UtcNow;

            // Remettre les livres comme disponibles
            foreach (var empruntLivre in emprunt.EmpruntLivres)
            {
                await _livreService.UpdateAvailabilityAsync(empruntLivre.IdLivre, true);
            }

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<EmpruntDTO>> GetEmpruntsByMembreAsync(int membreId)
        {
            var emprunts = await _context.Emprunts
                .Where(e => e.IdMembre == membreId)
                .Include(e => e.Membre)
                .Include(e => e.Employe)
                .Include(e => e.EmpruntLivres)
                    .ThenInclude(el => el.Livre)
                .OrderByDescending(e => e.DateEmprunt)
                .ToListAsync();

            return emprunts.Select(e => new EmpruntDTO
            {
                Id = e.Id,
                DateEmprunt = e.DateEmprunt,
                Statut = e.Statut,
                DateRetour = e.DateRetour,
                Membre = $"{e.Membre.PrenomMembre} {e.Membre.NomMembre}",
                Employe = $"{e.Employe.PrenomEmploye} {e.Employe.NomEmploye}",
                Livres = e.EmpruntLivres.Select(el => el.Livre.Titre).ToList()
            });
        }

        public async Task<IEnumerable<EmpruntDTO>> GetEmpruntsEnCoursAsync()
        {
            var emprunts = await _context.Emprunts
                .Where(e => e.Statut == "En cours")
                .Include(e => e.Membre)
                .Include(e => e.Employe)
                .Include(e => e.EmpruntLivres)
                    .ThenInclude(el => el.Livre)
                .OrderByDescending(e => e.DateEmprunt)
                .ToListAsync();

            return emprunts.Select(e => new EmpruntDTO
            {
                Id = e.Id,
                DateEmprunt = e.DateEmprunt,
                Statut = e.Statut,
                DateRetour = e.DateRetour,
                Membre = $"{e.Membre.PrenomMembre} {e.Membre.NomMembre}",
                Employe = $"{e.Employe.PrenomEmploye} {e.Employe.NomEmploye}",
                Livres = e.EmpruntLivres.Select(el => el.Livre.Titre).ToList()
            });
        }
    }
}


