using Peliculas.Models.Movie;

namespace Peliculas.Repositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<List<Movie>> GetAllWithFilters(string? search, string? genre);
        Task<Movie?> GetByIdWithDetails(int id);
        Task<List<object>> GetCountByGenre();
    }
}