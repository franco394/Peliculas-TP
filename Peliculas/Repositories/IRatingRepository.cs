using Peliculas.Models.Rating;

namespace Peliculas.Repositories
{
    public interface IRatingRepository : IRepository<Rating>
    {
        Task<Rating?> GetByUserAndMovie(int userId, int movieId);
        Task<Rating> Upsert(Rating rating);
    }
}