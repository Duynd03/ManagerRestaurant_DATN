using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;

namespace QuanLyNhaHang_DATN.Repositories.NhanVienRepository
{
    public class NhanVienRepository : BaseRepository<NhanVien>, INhanVienRepository
    {
        public NhanVienRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<NhanVien> GetByTaiKhoanUsernameAsync(string username)
        {
            return await _context.NhanViens
                .Include(nv => nv.TaiKhoan)
                .FirstOrDefaultAsync(nv => nv.TaiKhoan.UserName == username);
        }

        public async Task<NhanVien> GetByTaiKhoanIdAsync(int taiKhoanId)
        {
            return await _context.NhanViens
                .FirstOrDefaultAsync(nv => nv.TaiKhoanId == taiKhoanId);
        }

        public async Task<bool> CheckExistSdtAsync(string sdt)
        {
            return await _context.NhanViens
                .AnyAsync(nv => nv.Sdt == sdt);
        }
    }
}
