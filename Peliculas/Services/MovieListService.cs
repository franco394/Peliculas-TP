using System.Net;
using AutoMapper;
using Peliculas.Models;
using Peliculas.Models.MovieList;
using Peliculas.Models.MovieList.Dto;
using Peliculas.Repositories;
using Peliculas.Utils;

namespace Peliculas.Services
{
    public class MovieListService
    {
        private readonly IMovieListRepository _listRepo;
        private readonly IMapper _mapper;

        public MovieListService(IMovieListRepository listRepo, IMapper mapper)
        {
            _listRepo = listRepo;
            _mapper = mapper;
        }

        public async Task<List<MovieList>> GetByUser(int userId)
        {
            var lists = await _listRepo.GetByUser(userId);
            return lists.ToList();
        }

        public async Task<MovieListDetailDTO> GetById(int id, int? currentUserId)
        {
            var list = await _listRepo.GetByIdWithDetails(id);
            if (list == null) return null;

            if (!list.IsPublic && list.UserId != currentUserId)
                throw new UnauthorizedAccessException("Esta lista es privada.");

            return _mapper.Map<MovieListDetailDTO>(list);
        }

        public async Task<MovieListDTO> Create(int userId, CreateListDTO dto)
        {
            var list = new MovieList
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                IsPublic = dto.IsPublic
            };

            var created = await _listRepo.Create(list);
            return _mapper.Map<MovieListDTO>(created);
        }

        public async Task Delete(int userId, int listId)
        {
            var list = await _listRepo.GetOne();
            if (list == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"Lista con ID {listId} no encontrada"
                );
            }

            if (list.UserId != userId)
                throw new UnauthorizedAccessException("No podés eliminar una lista que no es tuya.");

        }

        public async Task AddMovie(int userId, int listId, AddMovieToListDTO dto)
        {
            var list = await _listRepo.GetByIdWithDetails(listId)
                ?? throw new Exception("Lista no encontrada.");

            if (list.UserId != userId)
                throw new UnauthorizedAccessException("No podés modificar una lista que no es tuya.");

            var alreadyIn = await _listRepo.MovieExistsInList(listId, dto.MovieId);
            if (alreadyIn)
                throw new Exception("La película ya está en la lista.");

            var item = new MovieListItem
            {
                MovieListId = listId,
                MovieId = dto.MovieId,
                Order = list.Items.Count + 1,
                Note = dto.Note
            };

            await _listRepo.AddMovie(item);
        }

        public async Task RemoveMovie(int userId, int listId, int movieId)
        {
            var list = await _listRepo.GetOne();

            if (list.UserId != userId)
                throw new UnauthorizedAccessException("No podés modificar una lista que no es tuya.");

            await _listRepo.RemoveMovie(listId, movieId);
        }
    }
}