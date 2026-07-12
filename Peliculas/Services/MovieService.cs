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

        public async Task<PagedResponseDTO<MovieDTO>> GetAll(MovieQueryDTO query)
        {
            var paged = await _movieRepo.GetAllWithFilters(query);
            var mappedData = _mapper.Map<List<MovieDTO>>(paged.Data);
            return _mapper.Map<PagedResponseDTO<MovieDTO>>(paged, opt =>
            {
                opt.AfterMap((src, dest) =>
                {
                    dest.Data = mappedData;
                });
            });
        }
        public async Task<MovieDTO> GetById(int id)
        {
            var movie = await _movieRepo.GetByIdWithDetails(id);

            if (movie == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"Movie con ID {id} no encontrada."
                );
            }

            return _mapper.Map<MovieDTO>(movie);
        }

        public async Task<MovieDTO> Create(CreateMovieDTO createDto)
        {
            var movie = _mapper.Map<Movie>(createDto);
            var genres = await _genreService.GetManyByIds(createDto.GenresIds);
            movie.Genres = genres;
            var created = await _movieRepo.Create(movie);
            return _mapper.Map<MovieDTO>(created);
        }

        public async Task<MovieDTO> Update(int id, UpdateMovieDTO updateDto)
        {
            var movie = await _movieRepo.GetByIdWithDetails(id);

            if (movie == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"Movie con ID {id} no encontrada."
                );
            }

            if (updateDto.GenresIds != null && updateDto.GenresIds.Count != 0)
            {
                var genres = await _genreService.GetManyByIds(updateDto.GenresIds);
                movie.Genres = genres;
            }


            movie.Title = updateDto.Title;
            movie.Director = updateDto.Director;
            movie.PosterUrl = updateDto.PosterUrl;
            var updated = await _movieRepo.Update(movie);
            return _mapper.Map<MovieDTO>(updated);
        }

        public async Task DeleteOneById(int id)
        {
            var movie = await _movieRepo.GetByIdWithDetails(id);

            if (movie == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"Movie con ID {id} no encontrada."
                );
            }

            await _movieRepo.DeleteOne(movie);
        }
    }
}