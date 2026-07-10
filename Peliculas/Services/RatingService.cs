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
        public async Task<RatingDTO> GetUserRating(int userId, int movieId)
        {
            var rating = await _ratingRepo.GetByUserAndMovie(userId, movieId);

            if (rating == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    "Rating no encontrado"
                );
            }

            var mapped = _mapper.Map<RatingDTO>(rating);
            return mapped;
        }

        public async Task<RatingDTO> Upsert(int userId, int movieId, UpsertRatingDTO upsertDto)
        {
            if (upsertDto.Score == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    "Score no puede ser nulo"
                );
            }

            var rating = _mapper.Map<Rating>(upsertDto);
            rating.UserId = userId;
            rating.MovieId = movieId;
            rating.Score = upsertDto.Score.Value;
            var saved = await _ratingRepo.Upsert(rating);
            var mapped = _mapper.Map<RatingDTO>(saved);
            return mapped;
        }

        public async Task Delete(int userId, int movieId)
        {
            var rating = await _ratingRepo.GetByUserAndMovie(userId, movieId);

            if (rating == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    "Rating no encontrado"
                );
            }

            await _ratingRepo.DeleteOne(rating);
        }

    }
}