using Peliculas.Models.MovieList;

namespace Peliculas.Repositories
{
    public interface IMovieListRepository : IRepository<MovieList>
    {
        Task<List<MovieList>> GetByUser(int userId);
        Task<MovieList?> GetByIdWithDetails(int id);
        Task AddMovie(Models.MovieListItem item);
        Task RemoveMovie(int listId, int movieId);
        Task<bool> MovieExistsInList(int listId, int movieId);
    }
}