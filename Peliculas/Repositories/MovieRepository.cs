using Microsoft.EntityFrameworkCore;
using Peliculas.Config;
using Peliculas.Models.Movie;

namespace Peliculas.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(AppDbContext db) : base(db) { }

        // Trae todas las películas con filtros y sus ratings incluidos
        public async Task<List<Movie>> GetAllWithFilters(string? search, string? genre)
        {
            var query = _db.Movies
                .Include(m => m.Ratings)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(m => m.Title.Contains(search) || m.Director.Contains(search));

            if (!string.IsNullOrEmpty(genre))
                query = query.Where(m => m.Genre == genre);

            return await query.OrderBy(m => m.Title).ToListAsync();
        }

        // Trae una película con todos sus datos relacionados
        public async Task<Movie?> GetByIdWithDetails(int id)
        {
            return await _db.Movies
                .Include(m => m.Ratings)
                .Include(m => m.Reviews).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        // Agrupa las películas por género para las estadísticas del admin
        public async Task<List<object>> GetCountByGenre()
        {
            return await _db.Movies
                .GroupBy(m => m.Genre)
                .Select(g => (object)new { Genre = g.Key, Count = g.Count() })
                .ToListAsync();
        }
    }
}