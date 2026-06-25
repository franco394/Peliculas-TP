using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Peliculas.Config;
using Microsoft.EntityFrameworkCore;

namespace Peliculas.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("stats")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetStats()
        {
            var totalMovies = await _db.Movies.CountAsync();
            var totalUsers = await _db.Users.CountAsync();
            var totalRatings = await _db.Ratings.CountAsync();
            var totalReviews = await _db.Reviews.CountAsync();

            var moviesByGenre = await _db.Movies
                .GroupBy(m => m.Genre)
                .Select(g => new { Genre = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(new
            {
                totalMovies,
                totalUsers,
                totalRatings,
                totalReviews,
                moviesByGenre
            });
        }
    }
}