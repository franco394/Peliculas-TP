using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.User.Dto
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}