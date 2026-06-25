using Peliculas.Models.Review;
using Peliculas.Models.Review.Dto;
using Peliculas.Repositories;

namespace Peliculas.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepo;

        public ReviewService(IReviewRepository reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public async Task<List<ReviewDTO>> GetByMovie(int movieId)
        {
            var reviews = await _reviewRepo.GetByMovie(movieId);
            return reviews.Select(ToDTO).ToList();
        }

        public async Task<ReviewDTO> Create(int userId, int movieId, CreateReviewDTO dto)
        {
            var alreadyExists = await _reviewRepo.ExistsByUserAndMovie(userId, movieId);
            if (alreadyExists)
                throw new Exception("Ya tenés una reseña para esta película.");

            var review = new Review
            {
                UserId = userId,
                MovieId = movieId,
                Content = dto.Content,
                ContainsSpoilers = dto.ContainsSpoilers
            };

            var created = await _reviewRepo.Create(review);
            var withDetails = await _reviewRepo.GetByIdWithDetails(created.Id);
            return ToDTO(withDetails!);
        }

        public async Task<ReviewDTO?> Update(int userId, int reviewId, UpdateReviewDTO dto)
        {
            var review = await _reviewRepo.GetByIdWithDetails(reviewId);
            if (review == null) return null;

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("No podés editar una reseña que no es tuya.");

            review.Content = dto.Content;
            review.ContainsSpoilers = dto.ContainsSpoilers;
            review.UpdatedAt = DateTime.UtcNow;

            await _reviewRepo.Update(review);
            return ToDTO(review);
        }

        public async Task<bool> Delete(int userId, int reviewId, bool isAdmin)
        {
            var review = await _reviewRepo.GetById(reviewId);
            if (review == null) return false;

            if (!isAdmin && review.UserId != userId)
                throw new UnauthorizedAccessException("No podés eliminar una reseña que no es tuya.");

            return await _reviewRepo.Delete(reviewId);
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