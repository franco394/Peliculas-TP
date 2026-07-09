using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.MovieList.Dto
{
    public class AddMovieToListDTO
    {
        [Required]
        public int MovieId { get; set; }
        public string? Note { get; set; }
    }
}
