using Microsoft.EntityFrameworkCore;
using Peliculas.Config;
using Peliculas.Models.Movie;
using Peliculas.Models.Movie.Dto;

namespace Peliculas.Services
{
    public class MovieService
    {
        private readonly AppDbContext _db;

        public MovieService(AppDbContext db)
        {
            _db = db;
        }

        // Trae todas las películas, con filtros opcionales de búsqueda y género
        public async Task<List<MovieDTO>> GetAll(string? search, string? genre)
        {
            var query = _db.Movies
                .Include(m => m.Ratings)
                .AsQueryable(); // AsQueryable permite ir encadenando filtros antes de ir a la DB

            if (!string.IsNullOrEmpty(search))
                query = query.Where(m => m.Title.Contains(search) || m.Director.Contains(search));

            if (!string.IsNullOrEmpty(genre))
                query = query.Where(m => m.Genre == genre);

            var movies = await query.OrderBy(m => m.Title).ToListAsync();
            return movies.Select(ToDTO).ToList();
        }

        // Trae una película por ID con sus reviews incluidas
        public async Task<MovieDTO?> GetById(int id)
        {
            var movie = await _db.Movies
                .Include(m => m.Ratings)
                .Include(m => m.Reviews).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            return movie == null ? null : ToDTO(movie);
        }

        // Crea una película nueva, solo accesible para Admin
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

            _db.Movies.Add(movie);
            await _db.SaveChangesAsync();
            return ToDTO(movie);
        }

        // Actualiza una película existente
        public async Task<MovieDTO?> Update(int id, UpdateMovieDTO dto)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null) return null;

            movie.Title = dto.Title;
            movie.Director = dto.Director;
            movie.Year = dto.Year;
            movie.Genre = dto.Genre;
            movie.Description = dto.Description;
            movie.PosterUrl = dto.PosterUrl;

            await _db.SaveChangesAsync();
            return ToDTO(movie);
        }

        // Elimina una película
        public async Task<bool> Delete(int id)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null) return false;

            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();
            return true;
        }

        // Convierte la entidad Movie a MovieDTO
        // Separamos esto en un método privado para no repetir la misma lógica en cada método
        private static MovieDTO ToDTO(Movie movie) => new()
        {
            Id = movie.Id,
            Title = movie.Title,
            Director = movie.Director,
            Year = movie.Year,
            Genre = movie.Genre,
            Description = movie.Description,
            PosterUrl = movie.PosterUrl,
            // Calculamos el promedio de ratings en memoria
            AverageRating = movie.Ratings.Count > 0
                ? movie.Ratings.Average(r => r.Score)
                : 0,
            RatingCount = movie.Ratings.Count
        };
    }
}