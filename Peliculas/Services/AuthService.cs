using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Peliculas.Config;
using Peliculas.Models.Role;
using Peliculas.Models.User;
using Peliculas.Models.User.Dto;
using Peliculas.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Peliculas.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginDTO login, HttpContext context);
        Task Logout(HttpContext context);
        Task<UserDTO> Register(RegisterDTO register);
    }

    public class AuthService : IAuthService
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly AppDbContext _db;
        private readonly IEncoderService _encoderService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthService(UserService userService, RoleService roleService, AppDbContext db, IEncoderService encoderService, IMapper mapper, IConfiguration config)
        {
            _userService = userService;
            _roleService = roleService;
            _db = db;
            _encoderService = encoderService;
            _mapper = mapper;
            _config = config;
        }

        public async Task<UserDTO> Register(RegisterDTO register)
        {
            User? existingUser = await _userService
                .GetOneByEmail(
                    register.Email
                );

            if (existingUser != null){
                throw new ErrorResponse(
                        HttpStatusCode.BadRequest,
                        "El email o nombre de usuario ya está en uso."
                    );
            }

            if (register.Password != register.ConfirmPassword)
            {
                throw new ErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Las contraseñas no coinciden."
                    );
            }

            var user = _mapper.Map<User>(register);
            user.PasswordHash = _encoderService.Encrypt(register.Password);
            var userRole = await _roleService.GetOneByName("User");
            user.Roles.Add(userRole);

            var created = await _userService.CreateOne(user);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<LoginResponse> Login(LoginDTO login, HttpContext context)
        {
            User? user = await _userService.GetOneByEmail(login.Email);

            if(user == null)
            {
                throw new ErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Email inválido."
                );
            }

            bool verified = _encoderService.Verify(user.PasswordHash, login.Password);
            if (!verified)
            {
                throw new ErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Credenciales inválidas."
                );
            }

            var loginResponse = new LoginResponse()
            {
                Token = GenerateToken(user),
                User = _mapper.Map<UserDTO>(user)
            };

            return loginResponse;
        }

        public async Task Logout(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
    }
}