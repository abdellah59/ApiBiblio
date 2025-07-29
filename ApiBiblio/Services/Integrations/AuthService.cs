using Microsoft.EntityFrameworkCore;
using ApiBiblio.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static ApiBiblio.DTOs.AuthDto;

namespace ApiBiblio.Services.Integrations
{
    public class AuthService : IAuthService
    {
        private readonly IEmployeService _employeService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IEmployeService employeService, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _employeService = employeService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation($"Tentative de connexion pour l'email: {loginDto.Email}");

                // Vérifier que l'email n'est pas vide
                if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                {
                    _logger.LogWarning("Email ou mot de passe vide");
                    throw new UnauthorizedAccessException("Email et mot de passe requis");
                }

                // Valider les credentials
                var isValid = await _employeService.ValidateCredentialsAsync(loginDto.Email, loginDto.Password);
                _logger.LogInformation($"Validation des credentials pour {loginDto.Email}: {isValid}");

                if (!isValid)
                {
                    _logger.LogWarning($"Credentials invalides pour l'email: {loginDto.Email}");
                    throw new UnauthorizedAccessException("Email ou mot de passe incorrect");
                }

                // Récupérer le rôle
                var role = await _employeService.GetRoleAsync(loginDto.Email);
                _logger.LogInformation($"Rôle récupéré pour {loginDto.Email}: {role}");

                if (string.IsNullOrWhiteSpace(role))
                {
                    _logger.LogWarning($"Aucun rôle trouvé pour l'email: {loginDto.Email}");
                    throw new UnauthorizedAccessException("Utilisateur sans rôle défini");
                }

                // Vérifier la configuration JWT
                var jwtSettings = _configuration.GetSection("JwtSettings");
                if (string.IsNullOrWhiteSpace(jwtSettings["SecretKey"]))
                {
                    _logger.LogError("SecretKey JWT manquante dans la configuration");
                    throw new InvalidOperationException("Configuration JWT invalide");
                }

                // Générer le token
                var token = await GenerateJwtTokenAsync(loginDto.Email, role);
                _logger.LogInformation($"Token généré avec succès pour {loginDto.Email}");

                return new LoginResponseDto
                {
                    Token = token,
                    Email = loginDto.Email,
                    Role = role,
                    Expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiryMinutes"]))
                };
            }
            catch (UnauthorizedAccessException)
            {
                throw; // Re-lancer les erreurs d'autorisation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de la connexion pour {loginDto.Email}");
                throw new UnauthorizedAccessException("Erreur lors de la connexion");
            }
        }

        public async Task<string> GenerateJwtTokenAsync(string email, string role)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
                };

                var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]);
                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la génération du token JWT");
                throw;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var secretKey = jwtSettings["SecretKey"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                await tokenHandler.ValidateTokenAsync(token, validationParameters);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token JWT invalide");
                return false;
            }
        }
    }
}