using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peliculas.Models.MovieList.Dto;
using Peliculas.Services;
using System.Security.Claims;

namespace Peliculas.Controllers
{
    [Route("api/lists")]
    [ApiController]
    [Authorize]
    public class MovieListsController : ControllerBase
    {
        private readonly MovieListService _listService;

        public MovieListsController(MovieListService listService)
        {
            _listService = listService;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // Trae las listas del usuario logueado
        [HttpGet]
        [ProducesResponseType(typeof(List<MovieListDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<MovieListDTO>>> GetMine()
        {
            var result = await _listService.GetByUser(GetCurrentUserId());
            return Ok(result);
        }

        // Ver el detalle de una lista no requiere estar logueado (si es pública)
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(MovieListDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<MovieListDetailDTO>> GetById(int id)
        {
            try
            {
                // Si no está logueado currentUserId es null
                int? currentUserId = User.Identity?.IsAuthenticated == true
                    ? GetCurrentUserId()
                    : null;

                var result = await _listService.GetById(id, currentUserId);
                return result == null ? NotFound() : Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(MovieListDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MovieListDTO>> Create([FromBody] CreateListDTO dto)
        {
            try
            {
                var result = await _listService.Create(GetCurrentUserId(), dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _listService.Delete(GetCurrentUserId(), id);

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/movies")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddMovie(int id, [FromBody] AddMovieToListDTO dto)
        {
            try
            {
                await _listService.AddMovie(GetCurrentUserId(), id, dto);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/movies/{movieId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RemoveMovie(int id, int movieId)
        {
            try
            {
                await _listService.RemoveMovie(GetCurrentUserId(), id, movieId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}