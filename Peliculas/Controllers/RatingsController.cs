using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peliculas.Models.Rating.Dto;
using Peliculas.Services;
using System.Security.Claims;

namespace Peliculas.Controllers
{
    [Route("api/movies/{movieId}/ratings")]
    [ApiController]
    [Authorize]
    public class RatingsController : ControllerBase
    {
        private readonly RatingService _ratingService;

        public RatingsController(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        [HttpPut]
        [ProducesResponseType(typeof(RatingDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RatingDTO>> Upsert(int movieId, [FromBody] UpsertRatingDTO dto)
        {
            try
            {
                var result = await _ratingService.Upsert(GetCurrentUserId(), movieId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int movieId)
        {
            try
            {
                await _ratingService.Delete(GetCurrentUserId(), movieId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(RatingDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RatingDTO>> GetMine(int movieId)
        {
            var result = await _ratingService.GetUserRating(GetCurrentUserId(), movieId);
            return result == null ? NotFound() : Ok(result);
        }
    }
}