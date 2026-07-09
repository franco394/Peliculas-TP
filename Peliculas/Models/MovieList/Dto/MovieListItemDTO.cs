namespace Peliculas.Models.MovieList.Dto
{
    public class MovieListItemDTO
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public int Order { get; set; }
        public string? Note { get; set; }
    }
}
