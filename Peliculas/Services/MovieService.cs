using System.Net;
using AutoMapper;
using Peliculas.Models.Movie;
using Peliculas.Models.Movie.Dto;
using Peliculas.Repositories;
using Peliculas.Utils;

namespace Peliculas.Services
{
    public class MovieService
    {
        private readonly GenreService _genreService;
        private readonly IMovieRepository _movieRepo;
        private readonly IMapper _mapper;

        public MovieService(GenreService genreService, IMovieRepository movieRepo, IMapper mapper)
        {
            _genreService = genreService;
            _movieRepo = movieRepo;
            _mapper = mapper;
        }

        public async Task<PagedResponseDTO<Movie>> GetAll(MovieQueryDTO query)
        {
            return await _movieRepo.GetAllWithFilters(query);
        }
        public async Task<Movie> GetById(int id)
        {
            var movie = await _movieRepo.GetByIdWithDetails(id);

            if(movie == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"Movie con ID {id} no encontrada."
                );
            }

            return movie;
        }

        public async Task<Movie> Create(CreateMovieDTO createDto)
        {
            var movie = _mapper.Map<Movie>(createDto);
            var genres = await _genreService.GetManyByIds(createDto.GenresIds);
            movie.Genres = genres;
            return await _movieRepo.Create(movie);
        }

        public async Task<Movie> Update(int id, UpdateMovieDTO updateDto)
        {
            var movie = await GetById(id);

            if (updateDto.GenresIds != null)
            {
                var genres = await _genreService.GetManyByIds(updateDto.GenresIds);
                movie.Genres = genres;
            }

            var updated = _mapper.Map(updateDto, movie);
            return await _movieRepo.Update(updated);
        }

        public async Task DeleteOneById(int id)
        {
            var movie = await GetById(id);
            await _movieRepo.DeleteOne(movie);
        }
    }
}