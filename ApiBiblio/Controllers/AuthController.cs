using ApiBiblio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static ApiBiblio.DTOs.AuthDto;

namespace BibliothequeApp.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmployeService _employeService;

        public AuthController(IAuthService authService, IEmployeService employeService)
        {
            _authService = authService;
            _employeService = employeService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Ajoutez des logs pour debugging
                Console.WriteLine($"Tentative de connexion pour: {loginDto.Email}");

                var result = await _authService.LoginAsync(loginDto);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Erreur d'autorisation: {ex.Message}");
                return Unauthorized(new { message = ex.Message, details = "Credentials invalides" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur générale: {ex.Message}");
                return BadRequest(new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost("register-employe")]
        public async Task<ActionResult> RegisterEmploye([FromBody] CreateEmployeDto createEmployeDto)
        {
            try
            {
                var result = await _employeService.CreateAsync(createEmployeDto);
                if (result)
                    return Ok(new { message = "Employé créé avec succès" });

                return BadRequest(new { message = "Erreur lors de la création de l'employé" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}