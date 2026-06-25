namespace Peliculas.Models
{
    public class MovieListItem
    {
        public int Id { get; set; }
        public int MovieListId { get; set; }
        public int MovieId { get; set; }
        public int Order { get; set; }
        public string? Note { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public MovieList.MovieList MovieList { get; set; } = null!;
        public Movie.Movie Movie { get; set; } = null!;
    }
}