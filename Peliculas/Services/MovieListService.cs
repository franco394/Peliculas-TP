using Microsoft.EntityFrameworkCore;
using Peliculas.Config;
using Peliculas.Models;
using Peliculas.Models.MovieList;
using Peliculas.Models.MovieList.Dto;

namespace Peliculas.Services
{
    public class MovieListService
    {
        private readonly AppDbContext _db;

        public MovieListService(AppDbContext db)
        {
            _db = db;
        }

        // Trae todas las listas de un usuario
        public async Task<List<MovieListDTO>> GetByUser(int userId)
        {
            var lists = await _db.MovieLists
                .Include(l => l.Items)
                .Where(l => l.UserId == userId)
                .ToListAsync();

            return lists.Select(ToDTO).ToList();
        }

        // Trae el detalle de una lista con sus películas
        public async Task<MovieListDetailDTO?> GetById(int id, int? currentUserId)
        {
            var list = await _db.MovieLists
                .Include(l => l.User)
                .Include(l => l.Items).ThenInclude(i => i.Movie)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (list == null) return null;

            // Si la lista es privada, solo el dueño puede verla
            if (!list.IsPublic && list.UserId != currentUserId)
                throw new UnauthorizedAccessException("Esta lista es privada.");

            return ToDetailDTO(list);
        }

        // Crea una lista nueva
        public async Task<MovieListDTO> Create(int userId, CreateListDTO dto)
        {
            var list = new MovieList
            {
                UserId = userId,
                Name = dto.Name,
                Description = dto.Description,
                IsPublic = dto.IsPublic
            };

            _db.MovieLists.Add(list);
            await _db.SaveChangesAsync();
            return ToDTO(list);
        }

        // Elimina una lista propia
        public async Task<bool> Delete(int userId, int listId)
        {
            var list = await _db.MovieLists.FindAsync(listId);
            if (list == null) return false;

            if (list.UserId != userId)
                throw new UnauthorizedAccessException("No podés eliminar una lista que no es tuya.");

            _db.MovieLists.Remove(list);
            await _db.SaveChangesAsync();
            return true;
        }

        // Agrega una película a una lista
        public async Task AddMovie(int userId, int listId, AddMovieToListDTO dto)
        {
            var list = await _db.MovieLists
                .Include(l => l.Items)
                .FirstOrDefaultAsync(l => l.Id == listId)
                ?? throw new Exception("Lista no encontrada.");

            if (list.UserId != userId)
                throw new UnauthorizedAccessException("No podés modificar una lista que no es tuya.");

            // Verificamos que la película no esté ya en la lista
            var alreadyIn = list.Items.Any(i => i.MovieId == dto.MovieId);
            if (alreadyIn)
                throw new Exception("La película ya está en la lista.");

            // Verificamos que la película existe
            var movie = await _db.Movies.FindAsync(dto.MovieId)
                ?? throw new Exception("Película no encontrada.");

            var item = new MovieListItem
            {
                MovieListId = listId,
                MovieId = dto.MovieId,
                Order = list.Items.Count + 1,
                Note = dto.Note
            };

            _db.MovieListItems.Add(item);
            await _db.SaveChangesAsync();
        }

        // Elimina una película de una lista
        public async Task RemoveMovie(int userId, int listId, int movieId)
        {
            var list = await _db.MovieLists.FindAsync(listId)
                ?? throw new Exception("Lista no encontrada.");

            if (list.UserId != userId)
                throw new UnauthorizedAccessException("No podés modificar una lista que no es tuya.");

            var item = await _db.MovieListItems
                .FirstOrDefaultAsync(i => i.MovieListId == listId && i.MovieId == movieId)
                ?? throw new Exception("La película no está en la lista.");

            _db.MovieListItems.Remove(item);
            await _db.SaveChangesAsync();
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