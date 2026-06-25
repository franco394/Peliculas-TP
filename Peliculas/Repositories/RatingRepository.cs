using Microsoft.EntityFrameworkCore;
using Peliculas.Config;
using Peliculas.Models.Rating;

namespace Peliculas.Repositories
{
    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        public RatingRepository(AppDbContext db) : base(db) { }

        // Busca el rating de un usuario para una película específica
        public async Task<Rating?> GetByUserAndMovie(int userId, int movieId)
        {
            return await _db.Ratings
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);
        }

        // Crea o actualiza el rating según si ya existe
        public async Task<Rating> Upsert(Rating rating)
        {
            var existing = await _db.Ratings
                .FirstOrDefaultAsync(r => r.UserId == rating.UserId && r.MovieId == rating.MovieId);

            if (existing != null)
            {
                existing.Score = rating.Score;
                existing.UpdatedAt = DateTime.UtcNow;
                _db.Ratings.Update(existing);
                await _db.SaveChangesAsync();
                await _db.Entry(existing).Reference(r => r.Movie).LoadAsync();
                return existing;
            }

            _db.Ratings.Add(rating);
            await _db.SaveChangesAsync();
            await _db.Entry(rating).Reference(r => r.Movie).LoadAsync();
            return rating;
        }
    }
}