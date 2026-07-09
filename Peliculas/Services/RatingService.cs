using System.Net;
using AutoMapper;
using Peliculas.Models.Rating;
using Peliculas.Models.Rating.Dto;
using Peliculas.Repositories;
using Peliculas.Utils;

namespace Peliculas.Services
{
    public class RatingService
    {
        private readonly IRatingRepository _ratingRepo;
        private readonly IMapper _mapper;

        public RatingService(IRatingRepository ratingRepo, IMapper mapper)
        {
            _ratingRepo = ratingRepo;
            _mapper = mapper;
        }
        public async Task<Rating> GetUserRating(int userId, int movieId)
        {
            var rating = await _ratingRepo.GetByUserAndMovie(userId, movieId);

            if (rating == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    "Rating no encontrado"
                );
            }

            return rating;
        }

        public async Task<Rating> Upsert(int userId, int movieId, UpsertRatingDTO upsertDto)
        {
            var rating = await GetUserRating(userId, movieId);
            var saved = await _ratingRepo.Upsert(rating);
            return saved;
        }

        public async Task Delete(int userId, int movieId)
        {
            var rating = await GetUserRating(userId, movieId);
            await _ratingRepo.DeleteOne(rating);
        }

    }
}