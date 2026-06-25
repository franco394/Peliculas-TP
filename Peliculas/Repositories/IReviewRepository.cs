using Peliculas.Models.Review;

namespace Peliculas.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<Review?> GetByIdWithDetails(int id);
        Task<List<Review>> GetByMovie(int movieId);
        Task<bool> ExistsByUserAndMovie(int userId, int movieId);
    }
}