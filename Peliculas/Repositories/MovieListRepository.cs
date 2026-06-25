using Microsoft.EntityFrameworkCore;
using Peliculas.Config;
using Peliculas.Models;
using Peliculas.Models.MovieList;

namespace Peliculas.Repositories
{
    public class MovieListRepository : Repository<MovieList>, IMovieListRepository
    {
        public MovieListRepository(AppDbContext db) : base(db) { }

        // Trae todas las listas de un usuario
        public async Task<List<MovieList>> GetByUser(int userId)
        {
            return await _db.MovieLists
                .Include(l => l.Items)
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        // Trae una lista con todas sus películas incluidas
        public async Task<MovieList?> GetByIdWithDetails(int id)
        {
            return await _db.MovieLists
                .Include(l => l.User)
                .Include(l => l.Items).ThenInclude(i => i.Movie)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        // Agrega una película a la lista
        public async Task AddMovie(MovieListItem item)
        {
            _db.MovieListItems.Add(item);
            await _db.SaveChangesAsync();
        }

        // Elimina una película de la lista
        public async Task RemoveMovie(int listId, int movieId)
        {
            var item = await _db.MovieListItems
                .FirstOrDefaultAsync(i => i.MovieListId == listId && i.MovieId == movieId)
                ?? throw new Exception("La película no está en la lista.");

            _db.MovieListItems.Remove(item);
            await _db.SaveChangesAsync();
        }

        // Verifica si una película ya está en la lista
        public async Task<bool> MovieExistsInList(int listId, int movieId)
        {
            return await _db.MovieListItems
                .AnyAsync(i => i.MovieListId == listId && i.MovieId == movieId);
        }
    }
}