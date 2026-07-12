using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peliculas.Models.Rating.Dto;
using Peliculas.Services;
using Peliculas.Utils;

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

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int movieId)
        {
            try
            {
                await _ratingService.Delete(GetCurrentUserId(), movieId);
                return Ok();
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

        [HttpGet("me")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(RatingDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RatingDTO>> GetMine(int movieId)
        {
            try
            {
                var result = await _ratingService.GetUserRating(GetCurrentUserId(), movieId);
                return result;
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