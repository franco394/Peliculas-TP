using Microsoft.EntityFrameworkCore;
using Peliculas.Config;
using Peliculas.Models.Rating;
using Peliculas.Models.Rating.Dto;

namespace Peliculas.Services
{
    public class RatingService
    {
        private readonly AppDbContext _db;

        public RatingService(AppDbContext db)
        {
            _db = db;
        }

        // Crea o actualiza el rating de un usuario para una película
        // Si ya existe lo actualiza, si no existe lo crea (upsert)
        public async Task<RatingDTO> Upsert(int userId, int movieId, UpsertRatingDTO dto)
        {
            var existing = await _db.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);

            if (existing != null)
            {
                // Ya calificó esta película, actualizamos el score
                existing.Score = dto.Score;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Primera vez que califica esta película
                existing = new Rating
                {
                    UserId = userId,
                    MovieId = movieId,
                    Score = dto.Score
                };
                _db.Ratings.Add(existing);
            }

            await _db.SaveChangesAsync();

            // Recargamos con la película incluida para el DTO
            await _db.Entry(existing).Reference(r => r.Movie).LoadAsync();
            return ToDTO(existing);
        }

        // Elimina el rating del usuario para una película
        public async Task<bool> Delete(int userId, int movieId)
        {
            var rating = await _db.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);

            if (rating == null) return false;

            _db.Ratings.Remove(rating);
            await _db.SaveChangesAsync();
            return true;
        }

        // Trae el rating del usuario para una película específica
        public async Task<RatingDTO?> GetUserRating(int userId, int movieId)
        {
            var rating = await _db.Ratings
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);

            return rating == null ? null : ToDTO(rating);
        }

        private static RatingDTO ToDTO(Rating rating) => new()
        {
            Id = rating.Id,
            MovieId = rating.MovieId,
            MovieTitle = rating.Movie?.Title ?? "",
            Score = rating.Score,
            CreatedAt = rating.CreatedAt
        };
    }
}