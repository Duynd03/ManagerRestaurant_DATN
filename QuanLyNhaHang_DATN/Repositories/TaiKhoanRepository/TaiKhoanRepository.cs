using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using System.Threading.Tasks;

namespace QuanLyNhaHang_DATN.Repositories.TaiKhoanRepository
{
    public class TaiKhoanRepository : BaseRepository<TaiKhoan>, ITaiKhoanRepository
    {
        private readonly UserManager<TaiKhoan> _userManager;

        public TaiKhoanRepository(AppDbContext context, UserManager<TaiKhoan> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<TaiKhoan> GetByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<bool> CheckExistUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username) != null;
        }

        public async Task<IdentityResult> CreateAsync(TaiKhoan taiKhoan, string password)
        {
            return await _userManager.CreateAsync(taiKhoan, password);
        }

         
    }
}