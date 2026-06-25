using Microsoft.EntityFrameworkCore;
using Peliculas.Config;

namespace Peliculas.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _db;

        public Repository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<T>> GetAll() =>
            await _db.Set<T>().ToListAsync();

        public async Task<T?> GetById(int id) =>
            await _db.Set<T>().FindAsync(id);

        public async Task<T> Create(T entity)
        {
            _db.Set<T>().Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<T> Update(T entity)
        {
            _db.Set<T>().Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await GetById(id);
            if (entity == null) return false;
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}