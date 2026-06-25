namespace Peliculas.Models.Rating
{
    public class Rating
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public decimal Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public User.User User { get; set; } = null!;
        public Movie.Movie Movie { get; set; } = null!;
    }
}
