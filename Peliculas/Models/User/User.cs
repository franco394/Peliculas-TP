using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Peliculas.Models.User
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Role.Role> Roles { get; set; } = new List<Role.Role>();
        public ICollection<Rating.Rating> Ratings { get; set; } = new List<Rating.Rating>();
        public ICollection<Review.Review> Reviews { get; set; } = new List<Review.Review>();
        public ICollection<MovieList.MovieList> MovieLists { get; set; } = new List<MovieList.MovieList>();
    }
}
