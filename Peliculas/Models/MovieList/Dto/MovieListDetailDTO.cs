namespace Peliculas.Models.MovieList.Dto
{
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
}
