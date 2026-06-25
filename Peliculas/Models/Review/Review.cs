namespace Peliculas.Models.Review
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool ContainsSpoilers { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public User.User User { get; set; } = null!;
        public Movie.Movie Movie { get; set; } = null!;
    }
}
