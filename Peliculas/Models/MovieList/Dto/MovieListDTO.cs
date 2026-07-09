using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.MovieList.Dto
{
    public class MovieListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public int MovieCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}