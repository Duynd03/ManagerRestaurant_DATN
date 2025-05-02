using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Repositories;

namespace QuanLyNhaHang_DATN.Services
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        protected readonly IBaseRepository<T> _repository;
        protected readonly AppDbContext _context;

        public BaseService(IBaseRepository<T> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _repository.GetAllAsync();

        public async Task<T> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task AddAsync(T entity)
        {
            await _repository.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            await _repository.UpdateAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
                await _context.SaveChangesAsync();
            }
        }

        public IQueryable<T> Query() => _repository.Query();

        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize)
        {
            var query = _repository.Query(); // hoặc _context.Set<T>() nếu không dùng repository
            var total = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return (items, total);
        }
    }


}
