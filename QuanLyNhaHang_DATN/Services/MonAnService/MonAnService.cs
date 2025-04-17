using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.MonAnRepository;
using QuanLyNhaHang_DATN.Repositories.DanhMucRepository;
using Microsoft.EntityFrameworkCore.Query;
using QuanLyNhaHang_DATN.Areas.Admin.Models;

namespace QuanLyNhaHang_DATN.Services.MonAnService
{
    public class MonAnService : BaseService<MonAn>, IMonAnService
    {
        private readonly IMonAnRepository _monAnRepository;
        private readonly IDanhMucRepository _danhMucRepository;

        //public MonAnService(IMonAnRepository repository, AppDbContext context)
        //    : base(repository, context)
        //{
        //    _monAnRepository = repository;
        //}
        public MonAnService(IMonAnRepository repository, IDanhMucRepository danhMucRepository, AppDbContext context): base(repository, context)
        {
            _monAnRepository = repository;
            _danhMucRepository = danhMucRepository;
        }

        public async Task<IEnumerable<MonAn>> GetByDanhMucAsync(int danhMucId)
        {
            return await _monAnRepository.GetByDanhMucAsync(danhMucId);
        }

        public async Task<IEnumerable<MonAn>> GetAvailableAsync()
        {
            return await _monAnRepository.GetAvailableAsync();
        }

        public async Task<(IEnumerable<MonAn> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, MonAnFilterModel filter)
        {
            // Khai báo query với kiểu rõ ràng là IQueryable<MonAn>
            IQueryable<MonAn> query = _monAnRepository.Query().Include(m => m.DanhMuc);

            // Kiểm tra filter null để tránh lỗi NullReferenceException
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.TenMonAn))
                {
                    query = query.Where(m => m.TenMonAn.Contains(filter.TenMonAn));
                }

                if (filter.DanhMucId.HasValue)
                {
                    query = query.Where(m => m.DanhMucId == filter.DanhMucId);
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(m => (int)m.TrangThai == filter.TrangThai.Value);
                }
            }

            // Tính tổng số bản ghi
            var total = await query.CountAsync();

            // Lấy danh sách bản ghi với phân trang
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

    }
}
