using ApiBiblio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ApiBiblio.DTOs.GenreDTO;

namespace ApiBiblio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreResponseDto>>> GetAll()
        {
            try
            {
                var genres = await _genreService.GetAllAsync();
                return Ok(genres);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreResponseDto>> GetById(int id)
        {
            try
            {
                var genre = await _genreService.GetByIdAsync(id);
                if (genre == null)
                    return NotFound(new { message = "Genre non trouvé" });

                return Ok(genre);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<GenreResponseDto>> Create([FromBody] CreateGenreDto createGenreDto)
        {
            try
            {
                var genre = await _genreService.CreateAsync(createGenreDto);
                return CreatedAtAction(nameof(GetById), new { id = genre.Id }, genre);
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
        public async Task<ActionResult> Update(int id, [FromBody] UpdateGenreDto updateGenreDto)
        {
            try
            {
                var result = await _genreService.UpdateAsync(id, updateGenreDto);
                if (!result)
                    return NotFound(new { message = "Genre non trouvé" });

                return Ok(new { message = "Genre mis à jour avec succès" });
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
                var result = await _genreService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = "Genre non trouvé" });

                return Ok(new { message = "Genre supprimé avec succès" });
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