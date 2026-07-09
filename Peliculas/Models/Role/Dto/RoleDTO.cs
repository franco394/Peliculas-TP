using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.Role.Dto
{
    public class RoleDTO
    {
        [Required]
        public string? RoleName { get; set; } = string.Empty;
    }
}
