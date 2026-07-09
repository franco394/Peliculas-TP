using System.Net;
using AutoMapper;
using Peliculas.Models.Genre;
using Peliculas.Models.Genre.DTO;
using Peliculas.Repositories;
using Peliculas.Utils;

namespace Peliculas.Services
{
    public class GenreService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Genre> _repo;

        public GenreService(IMapper mapper, IRepository<Genre> repo)
        {
            _mapper = mapper;
            _repo = repo;
        }

        public async Task<List<GenresDTO>> GetAll()
        {
            var list = await _repo.GetAll();
            var genres = _mapper.Map<List<GenresDTO>>(list);
            return genres;
        }

        private async Task<Genre> _GetOneById(int id)
        {
            var genre = await _repo.GetOne(g => g.Id == id);

            if(genre == null)
            {
                throw new ErrorResponse(
                        HttpStatusCode.NotFound,
                        $"Genre con ID {id} no encontrado"
                    );
            }

            return genre;
        }

        public async Task<Genre> GetOneById(int id) => await _GetOneById(id);

        public async Task<Genre> CreateOne(CreateGenreDTO createDto)
        {
            var g = _mapper.Map<Genre>(createDto);
            return await _repo.Create(g);
        }

        public async Task<Genre> UpdateOneById(int id, UpdateGenreDTO updateDto)
        {
            var genre = await _GetOneById(id);
            var updated = _mapper.Map(updateDto, genre);
            return await _repo.Update(updated);
        }

        public async Task DeleteOneById(int id)
        {
            var genre = await _repo.GetOne(g => g.Id == id);
            await _repo.DeleteOne(genre);
        }

        public async Task<List<Genre>> GetManyByIds(List<int> genresIds)
        {
            if(genresIds.Count == 0 || genresIds == null)
            {
                throw new ErrorResponse(
                        HttpStatusCode.BadRequest,
                        "La lista de IDs no puede estar vacía"
                    );
            }

            var list = await _repo.GetAll(x => genresIds.Contains(x.Id));
            if(list.Count == 0)
            {
                throw new ErrorResponse(
                        HttpStatusCode.NotFound,
                        "No coincide ningun ID"
                    );
            }

            return list;
        }
    }
}
