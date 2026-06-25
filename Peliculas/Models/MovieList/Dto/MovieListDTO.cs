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

    public class MovieListDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public string OwnerUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ICollection<MovieListItemDTO> Items { get; set; } = new List<MovieListItemDTO>();
    }

    public class MovieListItemDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public int Order { get; set; }
        public string? Note { get; set; }
    }

    public class CreateListDTO
    {
        [Required]
        [MinLength(1)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsPublic { get; set; } = true;
    }

    public class AddMovieToListDTO
    {
        [Required]
        public int MovieId { get; set; }
        public string? Note { get; set; }
    }
}