using System.ComponentModel.DataAnnotations;
using Peliculas.Models.Genre;

namespace Peliculas.Models.Movie.Dto
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int Year { get; set; }
        public List<string> Genres { get; set; } = new();
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
        public decimal AverageRating { get; set; }
        public int RatingCount { get; set; }
    }
}