using Peliculas.Models.Genre;

namespace Peliculas.Models.Movie
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int Year { get; set; }
        public List<Genre.Genre> Genres { get; set; } = new();
        public string Description { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Rating.Rating> Ratings { get; set; } = new List<Rating.Rating>();
        public ICollection<Review.Review> Reviews { get; set; } = new List<Review.Review>();
        public ICollection<MovieListItem> MovieListItems { get; set; } = new List<MovieListItem>();
    }
}
