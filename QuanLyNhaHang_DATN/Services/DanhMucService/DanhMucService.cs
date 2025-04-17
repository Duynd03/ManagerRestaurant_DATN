using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Areas.Admin.Models;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.DanhMucRepository;

namespace QuanLyNhaHang_DATN.Services.DanhMucService
{
    public class DanhMucService : BaseService<DanhMuc>, IDanhMucService
    {
        private readonly IDanhMucRepository _danhMucRepository;

        public DanhMucService(IDanhMucRepository repository, AppDbContext context)
            : base(repository, context)
        {
            _danhMucRepository = repository;
        }
        //public async Task<(IEnumerable<DanhMuc> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, string? search)
        //{
        //    var query = _danhMucRepository.Query(); // IQueryable<DanhMuc>

        //    if (!string.IsNullOrWhiteSpace(search))
        //    {
        //        query = query.Where(dm => dm.TenDanhMuc.Contains(search));
        //    }
        //    var countTask = query.CountAsync();
        //    var itemsTask = query
        //        .OrderBy(dm => dm.Id)
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    await Task.WhenAll(countTask, itemsTask);

        //    return (itemsTask.Result, countTask.Result);
        //    //var total = await query.CountAsync();

        //    //var items = await query
        //    //    .OrderBy(dm => dm.Id) // nên có OrderBy để tránh lỗi SQL
        //    //    .Skip((pageIndex - 1) * pageSize)
        //    //    .Take(pageSize)
        //    //    .ToListAsync();

        //    //return (items, total);
        //}
        public async Task<(IEnumerable<DanhMuc> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, DanhMucFilterModel filter)
        {
            var query = _danhMucRepository.Query();

            // Kiểm tra filter null để tránh lỗi NullReferenceException
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.TenDanhMuc))
                {
                    query = query.Where(m => m.TenDanhMuc.Contains(filter.TenDanhMuc));
                }
            }
            var total = await query.CountAsync();

            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }

}
