using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.MovieList.Dto
{
    public class CreateListDTO
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = true;
    }
}
