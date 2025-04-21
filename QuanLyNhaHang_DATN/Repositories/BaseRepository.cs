using QuanLyNhaHang_DATN.Data;
using Microsoft.EntityFrameworkCore;

namespace QuanLyNhaHang_DATN.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public Task AddAsync(T entity) => Task.FromResult(_dbSet.Add(entity));

        public Task UpdateAsync(T entity) => Task.FromResult(_dbSet.Update(entity));

        public Task DeleteAsync(T entity) => Task.FromResult(_dbSet.Remove(entity));

        public IQueryable<T> Query() => _dbSet.AsQueryable();
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }


}
