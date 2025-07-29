using Microsoft.EntityFrameworkCore;
using ApiBiblio.Database;
using ApiBiblio.Models;
using ApiBiblio.Services.Interfaces;
using Microsoft.CodeAnalysis.Scripting;
using static ApiBiblio.DTOs.AuthDto;

namespace ApiBiblio.Services.Integrations
{
    public class EmployeService : IEmployeService
    {
        private readonly BiblioDb _context;
        private readonly ILogger<EmployeService> _logger;

        public EmployeService(BiblioDb context, ILogger<EmployeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> CreateAsync(CreateEmployeDto createEmployeDto)
        {
            try
            {
                _logger.LogInformation($"Tentative de création d'employé avec email: {createEmployeDto.LoginEmploye}");

                // Vérifier si l'email existe déjà
                var existingEmploye = await _context.Employes
                    .FirstOrDefaultAsync(e => e.LoginEmploye == createEmployeDto.LoginEmploye);

                if (existingEmploye != null)
                {
                    _logger.LogWarning($"Employé avec email {createEmployeDto.LoginEmploye} existe déjà");
                    throw new InvalidOperationException("Un employé avec cet email existe déjà");
                }

                // Vérifier que le rôle existe
                var roleExists = await _context.Roles.AnyAsync(r => r.Id == createEmployeDto.IdRole);
                if (!roleExists)
                {
                    _logger.LogError($"Rôle avec ID {createEmployeDto.IdRole} n'existe pas");
                    throw new InvalidOperationException($"Le rôle avec l'ID {createEmployeDto.IdRole} n'existe pas");
                }

                // Hasher le mot de passe
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createEmployeDto.MdpEmploye);
                _logger.LogInformation($"Mot de passe haché pour {createEmployeDto.LoginEmploye}");

                var employe = new Employe
                {
                    NomEmploye = createEmployeDto.NomEmploye,
                    PrenomEmploye = createEmployeDto.PrenomEmploye,
                    LoginEmploye = createEmployeDto.LoginEmploye,
                    MdpEmploye = hashedPassword,
                    IdRole = createEmployeDto.IdRole
                };

                _context.Employes.Add(employe);
                var result = await _context.SaveChangesAsync();

                _logger.LogInformation($"Employé créé avec succès: {createEmployeDto.LoginEmploye}");
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la création de l'employé {createEmployeDto.LoginEmploye}");
                throw;
            }
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            try
            {
                _logger.LogInformation($"Validation des credentials pour: {email}");

                var employe = await _context.Employes
                    .FirstOrDefaultAsync(e => e.LoginEmploye == email);

                if (employe == null)
                {
                    _logger.LogWarning($"Aucun employé trouvé avec l'email: {email}");
                    return false;
                }

                _logger.LogInformation($"Employé trouvé: {employe.NomEmploye} {employe.PrenomEmploye}");

                var isValidPassword = BCrypt.Net.BCrypt.Verify(password, employe.MdpEmploye);
                _logger.LogInformation($"Validation mot de passe pour {email}: {isValidPassword}");

                return isValidPassword;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la validation des credentials pour {email}");
                return false;
            }
        }

        public async Task<string> GetRoleAsync(string email)
        {
            try
            {
                _logger.LogInformation($"Récupération du rôle pour: {email}");

                var employe = await _context.Employes
                    .Include(e => e.Role)
                    .FirstOrDefaultAsync(e => e.LoginEmploye == email);

                if (employe == null)
                {
                    _logger.LogError($"Employé non trouvé pour l'email: {email}");
                    throw new InvalidOperationException("Employé non trouvé");
                }

                if (employe.Role == null)
                {
                    _logger.LogError($"Aucun rôle associé à l'employé: {email}");
                    throw new InvalidOperationException("Aucun rôle associé à cet employé");
                }

                _logger.LogInformation($"Rôle trouvé pour {email}: {employe.Role.NomRole}");
                return employe.Role.NomRole;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération du rôle pour {email}");
                throw;
            }
        }

        public async Task<int> GetEmployeIdAsync(string email)
        {
            try
            {
                var employe = await _context.Employes
                    .FirstOrDefaultAsync(e => e.LoginEmploye == email);

                if (employe == null)
                {
                    _logger.LogError($"Employé non trouvé pour l'email: {email}");
                    throw new InvalidOperationException("Employé non trouvé");
                }

                return employe.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la récupération de l'ID employé pour {email}");
                throw;
            }
        }
    }
}
