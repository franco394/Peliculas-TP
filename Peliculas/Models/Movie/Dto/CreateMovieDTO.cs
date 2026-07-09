using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.Movie.Dto
{
    public class CreateMovieDTO
    {
        [Required(ErrorMessage = "Title requerido")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Director requerido")]
        public string Director { get; set; } = string.Empty;
        [Range(1888, 2100, ErrorMessage = "Year debe estar entre 1888 y 2100")]
        public int Year { get; set; }
        [Required(ErrorMessage = "Géneros requeridos")]
        public List<int> GenresIds { get; set; } = new();
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
    }
}
