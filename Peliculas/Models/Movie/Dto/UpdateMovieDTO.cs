using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.Movie.Dto
{
    public class UpdateMovieDTO
    {
        public string? Title { get; set; } = string.Empty;
        public string? Director { get; set; } = string.Empty;
        [Range(1888, 2100, ErrorMessage = "Year debe estar entre 1888 y 2100")]
        public int? Year { get; set; }
        public List<int>? GenresIds { get; set; } = new();
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
    }
}
