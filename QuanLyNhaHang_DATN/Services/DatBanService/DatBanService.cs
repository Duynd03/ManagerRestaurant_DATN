using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories;
using QuanLyNhaHang_DATN.Repositories.DatBanRepository;
using QuanLyNhaHang_DATN.Services.KhachHangService;
using QuanLyNhaHang_DATN.ViewModels;
using QuanLyNhaHang_DATN.Common;
using QuanLyNhaHang_DATN.Services.BanService;
using QuanLyNhaHang_DATN.Repositories.BanRepository;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;

namespace QuanLyNhaHang_DATN.Services.DatBanService
{
    public class DatBanService : BaseService<DatBan>, IDatBanService
    {
        private readonly IDatBanRepository _datBanRepository;
        private readonly IKhachHangService _khachHangService;
        private readonly IBanRepository _banRepository;

        public DatBanService(
            IDatBanRepository datBanRepository,
            IKhachHangService khachHangService,
            IBanRepository banRepository,
            AppDbContext context)
            : base(datBanRepository, context)
        {
            _datBanRepository = datBanRepository;
            _khachHangService = khachHangService;
            _banRepository = banRepository;
        }

        public async Task<Result<DatBan>> CreateDatBanAsync(DatBanViewModel viewModel, string username)
        {
            var errors = new List<string>();

            // Validate dữ liệu đầu vào
            if (viewModel == null)
                return new Result<DatBan>(false, "Dữ liệu đặt bàn không được để trống.", default, new List<string> { "Dữ liệu đầu vào không hợp lệ." });

            if (viewModel.SoLuongNguoi <= 0)
                errors.Add("Số lượng người phải lớn hơn 0.");

            if (viewModel.ThoiGianDatBan == default(DateTime) || viewModel.ThoiGianDatBan < DateTime.Now)
                errors.Add("Thời gian đặt bàn phải lớn hơn hiện tại.");

            KhachHang khachHang = null;

            // Nếu người dùng đã đăng nhập, lấy thông tin KhachHang từ username
            if (!string.IsNullOrEmpty(username))
            {
                khachHang = await _khachHangService.GetByTaiKhoanUsernameAsync(username);
                if (khachHang != null)
                {
                    // Ghi đè TenKhachHang và SDT từ thông tin KhachHang
                    viewModel.TenKhachHang = khachHang.TenKhachHang;
                    viewModel.SDT = khachHang.SDT;
                }
            }

            // Nếu không có KhachHang (chưa đăng nhập), tạo mới KhachHang
            if (khachHang == null)
            {
                if (string.IsNullOrEmpty(viewModel.TenKhachHang))
                    errors.Add("Tên khách hàng không được để trống.");

                if (string.IsNullOrEmpty(viewModel.SDT))
                    errors.Add("Số điện thoại không được để trống.");
                else if (!System.Text.RegularExpressions.Regex.IsMatch(viewModel.SDT, @"^\d{10}$"))
                    errors.Add("Số điện thoại phải có đúng 10 chữ số.");

                if (errors.Any())
                    return new Result<DatBan>(false, "Có lỗi xảy ra khi tạo đặt bàn.", default, errors);

                var result = await _khachHangService.TaoKhachHangAsync(
                    viewModel.TenKhachHang,
                    viewModel.SDT,
                    null,
                    null,
                    null);
                if (!result.Success)
                    return new Result<DatBan>(false, "Không thể tạo khách hàng.", default, result.Errors);
                khachHang = result.Data;
            }

            // Validate TenLienHe và SDTLienHe nếu IsDatHo = true
            if (viewModel.IsDatHo)
            {
                if (string.IsNullOrEmpty(viewModel.TenLienHe))
                    errors.Add("Tên người liên hệ không được để trống.");

                if (string.IsNullOrEmpty(viewModel.SDTLienHe))
                    errors.Add("Số điện thoại người liên hệ không được để trống.");
                else if (!System.Text.RegularExpressions.Regex.IsMatch(viewModel.SDTLienHe, @"^\d{10}$"))
                    errors.Add("Số điện thoại người liên hệ phải có đúng 10 chữ số.");
            }

            if (errors.Any())
                return new Result<DatBan>(false, "Có lỗi xảy ra khi tạo đặt bàn.", default, errors);

            // Tạo mới DatBan
            var datBan = new DatBan
            {
                KhachHangId = khachHang.Id,
                ThoiGianDatBan = viewModel.ThoiGianDatBan,
                SoLuongNguoi = viewModel.SoLuongNguoi,
                CocTien = viewModel.CocTien,
                BanId = viewModel.BanId,
                Loai = viewModel.Loai,
                TrangThai = TrangThaiBanDat.ChoXacNhan,
                ThoiGianTao = DateTime.Now,
                IsDatHo = viewModel.IsDatHo,
                TenLienHe = viewModel.IsDatHo ? viewModel.TenLienHe : null,
                SDTLienHe = viewModel.IsDatHo ? viewModel.SDTLienHe : null,
                GhiChu = viewModel.GhiChu
            };

            // Sử dụng AddAsync của BaseService để lưu DatBan
            await AddAsync(datBan);

            return new Result<DatBan>(true, "Đặt bàn thành công.", datBan);
        }

        public async Task<List<Ban>> GetAvailableBansAsync()
        {
            var bans = await _banRepository.GetAvailableAsync();
            return bans.ToList();
        }

        public async Task<Ban> GetBanByIdAsync(int banId)
        {
            return await _banRepository.GetByIdAsync(banId);
        }

        public async Task UpdateBanAsync(Ban ban)
        {
            await _banRepository.UpdateAsync(ban);
        }
        public async Task<List<DatBan>> GetByTrangThaiAsync(TrangThaiBanDat trangThai)
        {
            return await _datBanRepository.GetByTrangThaiAsync(trangThai);
        }
        public async Task<bool> HasTimeConflictAsync(int banId, DateTime thoiGianDatBan)
        {
            return await _datBanRepository.HasTimeConflictAsync(banId, thoiGianDatBan);
        }
        public async Task<(IEnumerable<DatBan> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, DatBanFilterModel filter)
        {
            IQueryable<DatBan> query = _context.DatBans.Include(db => db.KhachHang).AsQueryable();     // Lấy thông tin khách hàng


            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.TenKhachHang))
                {
                    query = query.Where(db => db.KhachHang.TenKhachHang.Contains(filter.TenKhachHang));
                }

                if (!string.IsNullOrWhiteSpace(filter.SDT))
                {
                    query = query.Where(db => db.KhachHang.SDT.Contains(filter.SDT));
                }

                if (filter.TrangThai.HasValue)
                {
                    query = query.Where(db => db.TrangThai == filter.TrangThai.Value);
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