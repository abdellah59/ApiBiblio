using ApiBiblio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ApiBiblio.DTOs.CategorieDTO;

namespace ApiBiblio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategorieController : ControllerBase
    {
        private readonly ICategorieService _categorieService;

        public CategorieController(ICategorieService categorieService)
        {
            _categorieService = categorieService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategorieResponseDto>>> GetAll()
        {
            try
            {
                var categories = await _categorieService.GetAllAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategorieResponseDto>> GetById(int id)
        {
            try
            {
                var categorie = await _categorieService.GetByIdAsync(id);
                if (categorie == null)
                    return NotFound(new { message = "Catégorie non trouvée" });

                return Ok(categorie);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CategorieResponseDto>> Create([FromBody] CreateCategorieDto createCategorieDto)
        {
            try
            {
                var categorie = await _categorieService.CreateAsync(createCategorieDto);
                return CreatedAtAction(nameof(GetById), new { id = categorie.Id }, categorie);
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
        public async Task<ActionResult> Update(int id, [FromBody] UpdateCategorieDto updateCategorieDto)
        {
            try
            {
                var result = await _categorieService.UpdateAsync(id, updateCategorieDto);
                if (!result)
                    return NotFound(new { message = "Catégorie non trouvée" });

                return Ok(new { message = "Catégorie mise à jour avec succès" });
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
                var result = await _categorieService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "Catégorie non trouvée" });

                return Ok(new { message = "Catégorie supprimée avec succès" });
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