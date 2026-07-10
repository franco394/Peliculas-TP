using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peliculas.Models.Review.Dto;
using Peliculas.Services;
using Peliculas.Utils;
using System.Security.Claims;

namespace Peliculas.Controllers
{
    [Route("api/movies/{movieId}/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _reviewService;

        public ReviewsController(ReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        private bool IsAdmin =>
            User.IsInRole("Admin");

        // Ver reviews de una película no requiere estar logueado
        [HttpGet]
        [ProducesResponseType(typeof(List<ReviewDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ReviewDTO>>> GetByMovie(int movieId)
        {
            var result = await _reviewService.GetByMovie(movieId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ReviewDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ReviewDTO>> Create(int movieId, [FromBody] CreateReviewDTO dto)
        {
            try
            {
                var result = await _reviewService.Create(GetCurrentUserId(), movieId, dto);
                return CreatedAtAction(nameof(GetByMovie), new { movieId }, result);
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

        [HttpPut("{reviewId}")]
        [Authorize]
        [ProducesResponseType(typeof(ReviewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ReviewDTO>> Update(int movieId, int reviewId, [FromBody] UpdateReviewDTO dto)
        {
            try
            {
                var result = await _reviewService.UpdateOneById(GetCurrentUserId(), reviewId, dto);
                return result == null ? NotFound() : Ok(result);
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

        [HttpDelete("{reviewId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int movieId, int reviewId)
        {
            try
            {
                await _reviewService.Delete(GetCurrentUserId(), reviewId, IsAdmin);
                ResponseMessage msg = new ResponseMessage($"Review con ID {reviewId} eliminada");
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