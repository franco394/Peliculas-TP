using Peliculas.Models.Genre;
using Peliculas.Models.Movie;
using Peliculas.Models.Movie.Dto;

namespace Peliculas.Repositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<PagedResponseDTO<Movie>> GetAllWithFilters(MovieQueryDTO query);
        Task<Movie?> GetByIdWithDetails(int id);
        Task<List<object>> GetCountByGenre();
    }
}