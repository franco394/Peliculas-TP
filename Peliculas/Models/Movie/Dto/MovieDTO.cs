using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.Movie.Dto
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Genre { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
        public decimal AverageRating { get; set; }
        public int RatingCount { get; set; }
    }

    public class CreateMovieDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Director { get; set; } = string.Empty;
        [Range(1888, 2100)]
        public int Year { get; set; }
        [Required]
        public string Genre { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
    }

    public class UpdateMovieDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Director { get; set; } = string.Empty;
        [Range(1888, 2100)]
        public int Year { get; set; }
        [Required]
        public string Genre { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
    }
}