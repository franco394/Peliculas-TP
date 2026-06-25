namespace Peliculas.Models.MovieList
{
    public class MovieList
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User.User User { get; set; } = null!;
        public ICollection<MovieListItem> Items { get; set; } = new List<MovieListItem>();
    }
}