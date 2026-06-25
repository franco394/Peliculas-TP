using Peliculas.Models.Movie;
using Peliculas.Models.Movie.Dto;
using Peliculas.Repositories;

namespace Peliculas.Services
{
    public class MovieService
    {
        private readonly IMovieRepository _movieRepo;

        public MovieService(IMovieRepository movieRepo)
        {
            _movieRepo = movieRepo;
        }

        public async Task<List<MovieDTO>> GetAll(string? search, string? genre)
        {
            var movies = await _movieRepo.GetAllWithFilters(search, genre);
            return movies.Select(ToDTO).ToList();
        }

        public async Task<MovieDTO?> GetById(int id)
        {
            var movie = await _movieRepo.GetByIdWithDetails(id);
            return movie == null ? null : ToDTO(movie);
        }

        public async Task<MovieDTO> Create(CreateMovieDTO dto)
        {
            var movie = new Movie
            {
                Title = dto.Title,
                Director = dto.Director,
                Year = dto.Year,
                Genre = dto.Genre,
                Description = dto.Description,
                PosterUrl = dto.PosterUrl
            };

            var created = await _movieRepo.Create(movie);
            return ToDTO(created);
        }

        public async Task<MovieDTO?> Update(int id, UpdateMovieDTO dto)
        {
            var movie = await _movieRepo.GetById(id);
            if (movie == null) return null;

            movie.Title = dto.Title;
            movie.Director = dto.Director;
            movie.Year = dto.Year;
            movie.Genre = dto.Genre;
            movie.Description = dto.Description;
            movie.PosterUrl = dto.PosterUrl;

            var updated = await _movieRepo.Update(movie);
            return ToDTO(updated);
        }

        public async Task<bool> Delete(int id) =>
            await _movieRepo.Delete(id);

        private static MovieDTO ToDTO(Movie movie) => new()
        {
            Id = movie.Id,
            Title = movie.Title,
            Director = movie.Director,
            Year = movie.Year,
            Genre = movie.Genre,
            Description = movie.Description,
            PosterUrl = movie.PosterUrl,
            AverageRating = movie.Ratings.Count > 0
                ? movie.Ratings.Average(r => r.Score)
                : 0,
            RatingCount = movie.Ratings.Count
        };
    }
}