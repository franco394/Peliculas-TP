using System.Net;
using AutoMapper;
using Peliculas.Models.Review;
using Peliculas.Models.Review.Dto;
using Peliculas.Repositories;
using Peliculas.Utils;

namespace Peliculas.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepo, IMapper mapper)
        {
            _reviewRepo = reviewRepo;
            _mapper = mapper;
        }

        public async Task<List<ReviewDTO>> GetByMovie(int movieId)
        {
            var reviews = await _reviewRepo.GetByMovie(movieId);
            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<Review> Create(int userId, int movieId, CreateReviewDTO createDto)
        {
            var r = _mapper.Map<Review>(createDto);
            var alreadyExists = await _reviewRepo.ExistsByUserAndMovie(userId, movieId);
            if (alreadyExists)
                throw new Exception("Ya tenés una reseña para esta película.");
            r.UserId = userId;
            r.MovieId = movieId;
            return await _reviewRepo.Create(r);
        }

        public async Task<Review> UpdateOneById(int userId, int reviewId, UpdateReviewDTO updateDto)
        {
            var review = await _reviewRepo.GetByIdWithDetails(reviewId);

            if (review.UserId != userId)
                throw new UnauthorizedAccessException("No podés editar una reseña que no es tuya.");

            if(updateDto.Content != null)
            {
                review.Content = updateDto.Content;
            }

            if(updateDto.ContainsSpoilers.HasValue)
            {
                review.ContainsSpoilers = updateDto.ContainsSpoilers.Value;
            }

            return await _reviewRepo.Update(review);

        }

        public async Task Delete(int userId, int reviewId, bool isAdmin)
        {
            var review = await _reviewRepo.GetOne(r => r.Id == reviewId);

            if(review == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"Review con ID {reviewId} no encontrada"
                );
            }

            if (!isAdmin && review.UserId != userId)
                throw new UnauthorizedAccessException("No podés eliminar una reseña que no es tuya.");

            await _reviewRepo.DeleteOne(review);
        }
    }
}