using Microsoft.EntityFrameworkCore;
using Peliculas.Config;
using Peliculas.Models.Genre;
using Peliculas.Models.Movie;
using Peliculas.Models.Movie.Dto;

namespace Peliculas.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(AppDbContext db) : base(db) { }

        // Trae todas las películas con filtros y sus ratings incluidos
        public async Task<PagedResponseDTO<Movie>> GetAllWithFilters(MovieQueryDTO queryDto)
        {
            var query = _db.Movies
                .Include(m => m.Genres)
                .Include(m => m.Ratings)
                .AsQueryable();

            // Búsqueda
            if (!string.IsNullOrWhiteSpace(queryDto.Search))
            {
                query = query.Where(m =>
                    m.Title.Contains(queryDto.Search) ||
                    m.Director.Contains(queryDto.Search));
            }

            if (queryDto.GenreId.HasValue)
            {
                query = query.Where(m =>
                    m.Genres.Any(g => g.Id == queryDto.GenreId.Value));
            }

            // Orden por título
            query = query.OrderBy(m => m.Title);

            // Total de registros antes de paginar
            var totalRecords = await query.CountAsync();

            // Paginación
            query = query
                .Skip((queryDto.Page - 1) * queryDto.PageSize)
                .Take(queryDto.PageSize);

            // Obtener las películas
            var movies = await query.ToListAsync();

            return new PagedResponseDTO<Movie>
            {
                Data = movies,
                Page = queryDto.Page,
                PageSize = queryDto.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / queryDto.PageSize)
            };
        }

        // Trae una película con todos sus datos relacionados
        public async Task<Movie?> GetByIdWithDetails(int id)
        {
            return await _db.Movies
                .Include(g => g.Genres)
                .Include(m => m.Ratings)
                .Include(m => m.Reviews).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        // Agrupa las películas por género para las estadísticas del admin
        public async Task<List<object>> GetCountByGenre()
        {
            return await _db.Movies
                .SelectMany(m => m.Genres)
                .GroupBy(g => new { g.Id, g.GenreName })
                .Select(g => (object)new { 
                    Genre = g.Key, Count = g.Count() 
                })
                .ToListAsync();
        }
    }
}