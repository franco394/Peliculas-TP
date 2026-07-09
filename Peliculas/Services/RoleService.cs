using System.Net;
using AutoMapper;
using Peliculas.Models.Role;
using Peliculas.Models.Role.Dto;
using Peliculas.Repositories;
using Peliculas.Utils;

namespace Peliculas.Services
{
    public class RoleService
    {
        public readonly IMapper _mapper;
        public readonly IRepository<Role> _repo;
        public RoleService(IMapper mapper, IRepository<Role> repo)
        {
            _mapper = mapper;
            _repo = repo;
        }
        public async Task<List<Role>> GetAll() => await _repo.GetAll();

        public async Task<Role> GetOneById(int id)
        {
            var role = await _repo.GetOne(x => x.Id == id);

            if (role == null)
            {
                throw new ErrorResponse(
                        HttpStatusCode.NotFound,
                        $"Rol con ID {id} no encontrado"
                    );
            }

            return role;
        }
        public async Task<Role> GetOneByName(string name)
        {
            var role = await _repo.GetOne(x => x.Name == name);

            if (role == null)
            {
                throw new ErrorResponse(
                        HttpStatusCode.NotFound,
                        $"Rol con Name = {name} no encontrado"
                    );
            }

            return role;
        }

        public async Task<Role> CreateOne(RoleDTO role)
        {
            var r = _mapper.Map<Role>(role);
            return await _repo.Create(r);
        }

        public async Task<Role> UpdateOneById(int id, RoleDTO roleDTO)
        {
            var r = await GetOneById(id);
            var updated = _mapper.Map(roleDTO, r);
            return await _repo.Update(updated);
        }

        public async Task DeleteOneById(int id)
        {
            var r = await GetOneById(id);
            await _repo.DeleteOne(r);
        }

        public async Task<List<Role>> GetManyByIds(List<int> rolesIds)
        {
            if (rolesIds.Count == 0 || rolesIds == null)
            {
                throw new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    "La lista de RolesIDs no puede estar vacía"
                );
            }

            var list = await _repo.GetAll(x => rolesIds.Contains(x.Id));
            if (list.Count == 0)
            {
                throw new ErrorResponse(
                    HttpStatusCode.NotFound,
                    "No coincide ningun Id"
                );
            }
            return list;
        }
    }
}
