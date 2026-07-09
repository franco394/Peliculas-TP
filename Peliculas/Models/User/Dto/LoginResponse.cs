namespace Peliculas.Models.User.Dto
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserDTO User { get; set; } = null!;
    }
}
