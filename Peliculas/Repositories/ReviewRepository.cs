using Microsoft.EntityFrameworkCore;
using Peliculas.Config;
using Peliculas.Models.Review;

namespace Peliculas.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext db) : base(db) { }

        // Trae una review con el usuario y la película incluidos
        public async Task<Review?> GetByIdWithDetails(int id)
        {
            return await _db.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Trae todas las reviews de una película ordenadas por fecha
        public async Task<List<Review>> GetByMovie(int movieId)
        {
            return await _db.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie)
                .Where(r => r.MovieId == movieId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // Verifica si un usuario ya tiene una review para una película
        public async Task<bool> ExistsByUserAndMovie(int userId, int movieId)
        {
            return await _db.Reviews
                .AnyAsync(r => r.UserId == userId && r.MovieId == movieId);
        }
    }
}