using ApiBiblio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ApiBiblio.DTOs.AuteurDTO;

namespace ApiBiblio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Nécessite une authentification
    public class AuteurController : ControllerBase
    {
        private readonly IAuteurService _auteurService;

        public AuteurController(IAuteurService auteurService)
        {
            _auteurService = auteurService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuteurResponseDto>>> GetAll()
        {
            try
            {
                var auteurs = await _auteurService.GetAllAsync();
                return Ok(auteurs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuteurResponseDto>> GetById(int id)
        {
            try
            {
                var auteur = await _auteurService.GetByIdAsync(id);
                if (auteur == null)
                    return NotFound(new { message = "Auteur non trouvé" });

                return Ok(auteur);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<AuteurResponseDto>> Create([FromBody] CreateAuteurDto createAuteurDto)
        {
            try
            {
                var auteur = await _auteurService.CreateAsync(createAuteurDto);
                return CreatedAtAction(nameof(GetById), new { id = auteur.Id }, auteur);
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

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateAuteurDto updateAuteurDto)
        {
            try
            {
                var result = await _auteurService.UpdateAsync(id, updateAuteurDto);
                if (!result)
                    return NotFound(new { message = "Auteur non trouvé" });

                return Ok(new { message = "Auteur mis à jour avec succès" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _auteurService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "Auteur non trouvé" });

                return Ok(new { message = "Auteur supprimé avec succès" });
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