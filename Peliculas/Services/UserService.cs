using System.Net;
using AutoMapper;
using Peliculas.Models.User;
using Peliculas.Models.User.Dto;
using Peliculas.Repositories;
using Peliculas.Utils;

namespace Peliculas.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;

        public UserService(IMapper mapper, IUserRepository userRepo)
        {
            _mapper = mapper;
            _userRepo = userRepo;
        }

        public async Task<List<User>> GetAll() => await _userRepo.GetAll();

        public async Task<User> GetOneById(int id)
        {
            var user = await _userRepo.GetOne(x => x.Id == id);

            if(user == null)
            {
                throw new ErrorResponse
                (
                    HttpStatusCode.NotFound,
                    $"User con ID {id} no encontrado."
                );
            }

            return user;
        }

        public async Task<User> CreateOne(User u)
        {
            return await _userRepo.Create(u);
        }

        public async Task<User> UpdateOneById(int id, UpdateUserDTO updateDto)
        {
            var u = await GetOneById(id);
            var updated = _mapper.Map(updateDto, u);
            return await _userRepo.Update(updated);
        }

        public async Task<User> UpdateEntity(User user)
        {
            return await _userRepo.Update(user);
        }

        public async Task<User> GetOneByEmail(string? email)
        {
            if (email == null || email == string.Empty)
            {
                throw new ErrorResponse(
                        HttpStatusCode.BadRequest,
                        "Debe proporcionar un email o un username"
                    );
            }
            return await _userRepo.GetOne(x => x.Email == email);
        }
    }
}