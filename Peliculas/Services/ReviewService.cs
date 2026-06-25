using Microsoft.EntityFrameworkCore;
using Peliculas.Config;
using Peliculas.Models.Review;
using Peliculas.Models.Review.Dto;

namespace Peliculas.Services
{
    public class ReviewService
    {
        private readonly AppDbContext _db;

        public ReviewService(AppDbContext db)
        {
            _db = db;
        }

        // Trae todas las reviews de una película
        public async Task<List<ReviewDTO>> GetByMovie(int movieId)
        {
            var reviews = await _db.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie)
                .Where(r => r.MovieId == movieId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(ToDTO).ToList();
        }

        // Crea una review para una película
        public async Task<ReviewDTO> Create(int userId, int movieId, CreateReviewDTO dto)
        {
            // Verificamos que la película existe
            var movie = await _db.Movies.FindAsync(movieId)
                ?? throw new Exception("Película no encontrada.");

            // Un usuario solo puede tener una review por película
            var existing = await _db.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);

            if (existing != null)
                throw new Exception("Ya tenés una reseña para esta película.");

            var review = new Review
            {
                UserId = userId,
                MovieId = movieId,
                Content = dto.Content,
                ContainsSpoilers = dto.ContainsSpoilers
            };

            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();

            // Cargamos User y Movie para el DTO
            await _db.Entry(review).Reference(r => r.User).LoadAsync();
            await _db.Entry(review).Reference(r => r.Movie).LoadAsync();

            return ToDTO(review);
        }

        // Actualiza una review propia
        public async Task<ReviewDTO?> Update(int userId, int reviewId, UpdateReviewDTO dto)
        {
            var review = await _db.Reviews
                .Include(r => r.User)
                .Include(r => r.Movie)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null) return null;

            // Solo el dueño puede editar su review
            if (review.UserId != userId)
                throw new UnauthorizedAccessException("No podés editar una reseña que no es tuya.");

            review.Content = dto.Content;
            review.ContainsSpoilers = dto.ContainsSpoilers;
            review.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return ToDTO(review);
        }

        // Elimina una review. El dueño o un admin pueden hacerlo
        public async Task<bool> Delete(int userId, int reviewId, bool isAdmin)
        {
            var review = await _db.Reviews.FindAsync(reviewId);
            if (review == null) return false;

            if (!isAdmin && review.UserId != userId)
                throw new UnauthorizedAccessException("No podés eliminar una reseña que no es tuya.");

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
            return true;
        }

        private static ReviewDTO ToDTO(Review review) => new()
        {
            Id = review.Id,
            MovieId = review.MovieId,
            MovieTitle = review.Movie?.Title ?? "",
            UserId = review.UserId,
            UserName = review.User?.UserName ?? "",
            Content = review.Content,
            ContainsSpoilers = review.ContainsSpoilers,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt
        };
    }
}
