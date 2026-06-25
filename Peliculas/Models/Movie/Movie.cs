using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Peliculas.Models.Movie
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Rating.Rating> Ratings { get; set; } = new List<Rating.Rating>();
        public ICollection<Review.Review> Reviews { get; set; } = new List<Review.Review>();
        public ICollection<MovieListItem> MovieListItems { get; set; } = new List<MovieListItem>();
    }
}
