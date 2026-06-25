using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peliculas.Models.Review.Dto;
using Peliculas.Services;
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
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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
                var result = await _reviewService.Update(GetCurrentUserId(), reviewId, dto);
                return result == null ? NotFound() : Ok(result);
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

        [HttpDelete("{reviewId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int movieId, int reviewId)
        {
            try
            {
                var deleted = await _reviewService.Delete(GetCurrentUserId(), reviewId, IsAdmin);
                return deleted ? NoContent() : NotFound();
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