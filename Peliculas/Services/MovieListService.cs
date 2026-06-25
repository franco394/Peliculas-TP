using Peliculas.Models;
using Peliculas.Models.MovieList;
using Peliculas.Models.MovieList.Dto;
using Peliculas.Repositories;

namespace Peliculas.Services
{
    public class MovieListService
    {
        private readonly IMovieListRepository _listRepo;

        public MovieListService(IMovieListRepository listRepo)
        {
            _listRepo = listRepo;
        }

        public async Task<List<MovieListDTO>> GetByUser(int userId)
        {
            var lists = await _listRepo.GetByUser(userId);
            return lists.Select(ToDTO).ToList();
        }

        public async Task<MovieListDetailDTO?> GetById(int id, int? currentUserId)
        {
            var list = await _listRepo.GetByIdWithDetails(id);
            if (list == null) return null;

            if (!list.IsPublic && list.UserId != currentUserId)
                throw new UnauthorizedAccessException("Esta lista es privada.");

            return ToDetailDTO(list);
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
            return ToDTO(created);
        }

        public async Task<bool> Delete(int userId, int listId)
        {
            var list = await _listRepo.GetById(listId);
            if (list == null) return false;

            if (list.UserId != userId)
                throw new UnauthorizedAccessException("No podés eliminar una lista que no es tuya.");

            return await _listRepo.Delete(listId);
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
            var list = await _listRepo.GetById(listId)
                ?? throw new Exception("Lista no encontrada.");

            if (list.UserId != userId)
                throw new UnauthorizedAccessException("No podés modificar una lista que no es tuya.");

            await _listRepo.RemoveMovie(listId, movieId);
        }

        private static MovieListDTO ToDTO(MovieList list) => new()
        {
            Id = list.Id,
            Name = list.Name,
            Description = list.Description,
            IsPublic = list.IsPublic,
            MovieCount = list.Items.Count,
            CreatedAt = list.CreatedAt
        };

        private static MovieListDetailDTO ToDetailDTO(MovieList list) => new()
        {
            Id = list.Id,
            Name = list.Name,
            Description = list.Description,
            IsPublic = list.IsPublic,
            OwnerUserName = list.User?.UserName ?? "",
            CreatedAt = list.CreatedAt,
            Items = list.Items
                .OrderBy(i => i.Order)
                .Select(i => new MovieListItemDTO
                {
                    MovieId = i.MovieId,
                    Title = i.Movie?.Title ?? "",
                    PosterUrl = i.Movie?.PosterUrl,
                    Order = i.Order,
                    Note = i.Note
                }).ToList()
        };
    }
}