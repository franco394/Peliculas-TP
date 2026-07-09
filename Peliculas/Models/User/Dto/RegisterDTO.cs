using System.ComponentModel.DataAnnotations;

namespace Peliculas.Models.User.Dto
{
    public class RegisterDTO
    {
        [Required]
        [MinLength(3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [MinLength(6)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
