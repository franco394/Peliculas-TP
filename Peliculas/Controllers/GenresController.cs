using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Peliculas.Models.Genre;
using Peliculas.Models.Genre.DTO;
using Peliculas.Services;
using Peliculas.Utils;

namespace Peliculas.Controllers
{
    [Route("api/genre")]
    [ApiController]
    [ProducesResponseType(typeof(ResponseValidation), StatusCodes.Status500InternalServerError)]
    public class GenresController : ControllerBase
    {
        private GenreService _genreService;

        public GenresController(GenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Genre), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Genre>>> GetAll()
        {
            var list = await _genreService.GetAll();
            return Ok(list);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GenresDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseValidation), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GenresDTO>> GetOnyById(int id)
        {
            try
            {
                var genre = await _genreService.GetOneById(id);
                return Ok(genre);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseValidation), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Genre), StatusCodes.Status201Created)]
        public async Task<ActionResult<Genre>> CreateOne([FromBody] CreateGenreDTO createGenre)
        {
            try
            {
                var context = HttpContext;
                var modelState = ModelState;
                var genre = await _genreService.CreateOne(createGenre);
                return Created("POST: api/genre", genre);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Genre), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseValidation), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Genre>> UpdateOneById(int id, [FromBody] UpdateGenreDTO updateDto)
        {
            try
            {
                var genre = await _genreService.UpdateOneById(id, updateDto);
                return Ok(genre);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<ActionResult> DeleteOneById(int id)
        {
            try
            {
                await _genreService.DeleteOneById(id);
                ResponseMessage msg = new ResponseMessage($"Genre con ID {id} eliminado");
                return Ok(msg);
            }
            catch (ErrorResponse ex)
            {
                return StatusCode((int)ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                ResponseMessage msg = new ResponseMessage(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, msg);
            }
        }
    }
}
