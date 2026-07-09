using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.Genre.DTO
{
    public class UpdateGenreDTO
    {
        [MinLength(3, ErrorMessage = "Mínimo 3 carácteres")]
        [MaxLength(32, ErrorMessage = "Máximo 32 carácteres")]
        public string? GenreName { get; set; } = null!;
    }
}
