using Peliculas.Models.Rating;
using Peliculas.Models.Rating.Dto;
using Peliculas.Repositories;

namespace Peliculas.Services
{
    public class RatingService
    {
        private readonly IRatingRepository _ratingRepo;

        public RatingService(IRatingRepository ratingRepo)
        {
            _ratingRepo = ratingRepo;
        }

        public async Task<RatingDTO> Upsert(int userId, int movieId, UpsertRatingDTO dto)
        {
            var rating = new Rating
            {
                UserId = userId,
                MovieId = movieId,
                Score = dto.Score
            };

            var saved = await _ratingRepo.Upsert(rating);
            return ToDTO(saved);
        }

        public async Task<bool> Delete(int userId, int movieId)
        {
            var rating = await _ratingRepo.GetByUserAndMovie(userId, movieId);
            if (rating == null) return false;
            return await _ratingRepo.Delete(rating.Id);
        }

        public async Task<RatingDTO?> GetUserRating(int userId, int movieId)
        {
            var rating = await _ratingRepo.GetByUserAndMovie(userId, movieId);
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