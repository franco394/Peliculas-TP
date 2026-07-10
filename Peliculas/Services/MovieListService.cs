using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task<List<MovieListDTO>> GetByUser(int userId)
        {
            var lists = await _listRepo.GetByUser(userId);
            var mapped = _mapper.Map<List<MovieListDTO>>(lists);
            return mapped;
        }

        public async Task<MovieListDetailDTO> GetById(int id, int? currentUserId)
        {
            var list = await _listRepo.GetByIdWithDetails(id);

            if (list == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"Lista con ID {id} no encontrada"
                );
            }

            if (!list.IsPublic && list.UserId != currentUserId)
            {
                throw new ErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "No podés ver una lista privada que no es tuya."
                );
            }

            return _mapper.Map<MovieListDetailDTO>(list);
        }

        public async Task<MovieListDTO> Create(int userId, CreateListDTO createDto)
        {
            var list = _mapper.Map<MovieList>(createDto);
            list.UserId = userId;
            var created = await _listRepo.Create(list);
            return _mapper.Map<MovieListDTO>(created);
        }

        public async Task Delete(int userId, int listId)
        {
            var list = await _listRepo.GetByIdWithDetails(listId);

            if (list == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"Lista con ID {listId} no encontrada"
                );
            }

            if (list.UserId != userId)
            {
                throw new ErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "No podés eliminar una lista que no es tuya."
                );
            }

            await _listRepo.DeleteOne(list);
        }

        public async Task AddMovie(int userId, int listId, AddMovieToListDTO dto)
        {
            var list = await _listRepo.GetByIdWithDetails(listId);

            if (list == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"Lista con ID {listId} no encontrada"
                );
            }

            if (list.UserId != userId)
            {
                throw new ErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "No podés modificar una lista que no es tuya."
                );
            }

            var alreadyIn = await _listRepo.MovieExistsInList(listId, dto.MovieId);
            if (alreadyIn)
            {
                throw new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    "La película ya está en la lista."
                );
            }

            var item = _mapper.Map<MovieListItem>(dto);
            item.MovieListId = listId;
            item.Order++;
            await _listRepo.AddMovie(item);
        }

        public async Task RemoveMovie(int userId, int listId, int movieId)
        {
            var list = await _listRepo.GetByIdWithDetails(listId);

            if (list == null){
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    $"Lista con ID {listId} no encontrada"
                );
            }

            if (list.UserId != userId)
            {
                throw new ErrorResponse(
                    HttpStatusCode.Unauthorized,
                    "No podés modificar una lista que no es tuya."
                );
            }
            var exists = await _listRepo.MovieExistsInList(listId, movieId);
            if (!exists)
            {
                throw new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    "La película no está en la lista."
                );
            }

            await _listRepo.RemoveMovie(listId, movieId);
        }
    }
}