using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Peliculas.Config;
using Peliculas.Models.Role;
using Peliculas.Models.User;
using Peliculas.Models.User.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Peliculas.Services
{
    public interface IAuthService
    {
        Task<UserDTO> Register(RegisterDTO register);
        Task<LoginResponse> Login(LoginDTO login);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<UserDTO> Register(RegisterDTO register)
        {
            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == register.Email || u.UserName == register.UserName);

            if (existingUser != null)
                throw new Exception("El email o nombre de usuario ya está en uso.");

            var userRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "User")
                ?? throw new Exception("Rol User no encontrado.");

            var user = new User
            {
                UserName = register.UserName,
                Email = register.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Roles = new List<Role> { userRole }
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return ToDTO(user);
        }

        public async Task<LoginResponse> Login(LoginDTO login)
        {
            var user = await _db.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == login.Email)
                ?? throw new Exception("Credenciales inválidas.");

            if (!BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                throw new Exception("Credenciales inválidas.");

            return new LoginResponse
            {
                Token = GenerateToken(user),
                User = ToDTO(user)
            };
        }

        private string GenerateToken(User user)
        {
            var secret = _config["Secrets:jwt"]
                ?? throw new Exception("JWT secret no configurado.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            foreach (var role in user.Roles)
                claims.Add(new Claim(ClaimTypes.Role, role.Name));

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddDays(7),
                claims: claims,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static UserDTO ToDTO(User user) => new()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Roles = user.Roles.Select(r => r.Name).ToList()
        };
    }
}