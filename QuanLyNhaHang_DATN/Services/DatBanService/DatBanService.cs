﻿
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
using QuanLyNhaHang_DATN.Repositories.NhanVienRepository;

namespace QuanLyNhaHang_DATN.Services.DatBanService
{
    public class DatBanService : BaseService<DatBan>, IDatBanService
    {
        private readonly IDatBanRepository _datBanRepository;
        private readonly IKhachHangService _khachHangService;
        private readonly IBanRepository _banRepository;
        private readonly INhanVienRepository _nhanVienRepository;
        private const int MaxSoGhe = 6; 
        private const double DefaultDurationHours = 2.0; 
        private const double bufferTimeMinutes = 30;
        public DatBanService(
            IDatBanRepository datBanRepository,
            IKhachHangService khachHangService,
            IBanRepository banRepository,
            INhanVienRepository nhanVienRepository,
            AppDbContext context)
            : base(datBanRepository, context)
        {
            _datBanRepository = datBanRepository;
            _khachHangService = khachHangService;
            _banRepository = banRepository;
            _nhanVienRepository = nhanVienRepository;
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
                //BanId = viewModel.BanId,
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

        public async Task<Result<DatBan>> UpdateDatBanAsync(DatBanViewModel viewModel, string username)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Kiểm tra tồn tại
                    var datBan = await _context.DatBans
                        .Include(db => db.KhachHang)
                        .FirstOrDefaultAsync(db => db.Id == viewModel.Id);

                    if (datBan == null)
                        return new Result<DatBan>(false, "Đơn đặt bàn không tồn tại.", null);


                    if (viewModel.ThoiGianDatBan < DateTime.Now) 
                        return new Result<DatBan>(false, "Thời gian đặt bàn phải lớn hơn hiện tại.", null);

                    // Cập nhật KhachHang
                    datBan.KhachHang.TenKhachHang = viewModel.TenKhachHang;
                    datBan.KhachHang.SDT = viewModel.SDT;
                    _context.KhachHangs.Update(datBan.KhachHang);

                    // Cập nhật DatBan
                    datBan.ThoiGianDatBan = viewModel.ThoiGianDatBan;
                    datBan.SoLuongNguoi = viewModel.SoLuongNguoi;
                    datBan.GhiChu = viewModel.GhiChu;

                    // Cập nhật nhân viên xử lý
                    if (!string.IsNullOrEmpty(username))
                    {
                        var nhanVien = await _nhanVienRepository.GetByTaiKhoanUsernameAsync(username);
                        if (nhanVien != null)
                            datBan.NhanVienId = nhanVien.Id;
                    }

                    await _datBanRepository.UpdateAsync(datBan);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new Result<DatBan>(true, "Cập nhật đơn đặt bàn thành công!", datBan);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new Result<DatBan>(false, $"Lỗi khi cập nhật: {ex.Message}", null);
                }
            }
        }
        public async Task<DatBan> GetDatBanByIdAsync(int id)
        {
            return await _datBanRepository.GetDatBanByIdAsync(id);
        }
        public async Task<List<Ban>> GetAvailableBansAsync()  
        {
            var bans = await _banRepository.GetAvailableAsync();
            return bans.ToList();
        }
        public async Task<DatBan> GetByIdWithDetailsAsync(int id)
        {
            return await _context.DatBans
                .Include(db => db.KhachHang)
                .Include(db => db.DatBanBans)
                .ThenInclude(dbb => dbb.Ban)
                .FirstOrDefaultAsync(db => db.Id == id);
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
            IQueryable<DatBan> query = _context.DatBans
                .Include(db => db.KhachHang)
                .Include(db => db.DatBanBans)
                .ThenInclude(dbb => dbb.Ban)
                 .Include(db => db.NhanVien)
                .AsQueryable();

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

        public async Task<Result<DatBan>> XepBanAsync(int datBanId, List<int> banIds, int? nhanVienId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var datBan = await _datBanRepository.GetByIdWithDatBanBansAsync(datBanId);
                    if (datBan == null)
                        return new Result<DatBan>(false, "Đơn đặt bàn không tồn tại.", null, new List<string> { "Không tìm thấy đơn." });

                    // Thêm kiểm tra để tránh xếp lại đơn đã có bàn
                    if (datBan.DatBanBans != null && datBan.DatBanBans.Any())
                        return new Result<DatBan>(false, "Đặt bàn đã được xếp, không thể xếp lại.", null);

                    // Kiểm tra số lượng bàn
                    var (minAllowedBans, maxAllowedBans, tableMessage) = CalculateRequiredTables(datBan.SoLuongNguoi);
                    if (banIds.Count < minAllowedBans || banIds.Count > maxAllowedBans)
                        return new Result<DatBan>(false, tableMessage, null, new List<string> { tableMessage });

                    // Kiểm tra khả dụng 
                    var bookingEndTime = datBan.ThoiGianDatBan.AddHours(DefaultDurationHours).AddMinutes(bufferTimeMinutes);
                    foreach (var banId in banIds)
                    {
                        var khaDung = await CheckTableAvailabilityAsync(banId, datBan.ThoiGianDatBan, bookingEndTime, datBanId);
                        if (!khaDung.KhaDung)
                            return new Result<DatBan>(false, $"Bàn {banId} không khả dụng.", null, new List<string> { $"Bàn {banId} đã được đặt." });
                    }

                    // Tạo liên kết và lịch mới
                    var now = DateTime.Now;
                    var newSchedules = banIds.Select(banId => new BanSchedule
                    {
                        BanId = banId,
                        DatBanId = datBanId,
                        ThoiGianBatDau = datBan.ThoiGianDatBan,
                        ThoiGianKetThuc = datBan.ThoiGianDatBan.AddHours(DefaultDurationHours),
                        TrangThai = TrangThaiBan.DaDatTruoc,
                        NgayTao = now
                    }).ToList();
                    await _datBanRepository.AddRangeBanSchedulesAsync(newSchedules);

                    var newDatBanBans = banIds.Select(banId => new DatBan_Ban
                    {
                        DatBanId = datBanId,
                        BanId = banId
                    }).ToList();
                    await _datBanRepository.AddRangeDatBanBansAsync(newDatBanBans);
                    // Cập nhật thông tin đơn
                    datBan.NhanVienId = nhanVienId;
                    datBan.TrangThai = TrangThaiBanDat.DaXacNhan;
                    await _datBanRepository.UpdateAsync(datBan);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Result<DatBan>(true, "Xếp bàn thành công!", datBan);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new Result<DatBan>(false, $"Lỗi khi xếp bàn: {ex.Message}", null);
                }
            }
        }

        public IQueryable<BanSchedule> GetBanSchedules()
        {
            return _context.BanSchedules.AsQueryable();
        }
        public async Task AddDatBanBanAsync(DatBan_Ban datBanBan)
        {
            _context.DatBan_Bans.Add(datBanBan);
            await _context.SaveChangesAsync();
        }
        public async Task<List<string>> GetBanGhepAsync(int datBanId)
        {
            return await _context.DatBan_Bans
                .Where(db => db.DatBanId == datBanId)
                .Include(db => db.Ban)
                .Select(db => db.Ban.TenBan)
                .ToListAsync();
        }
        public async Task<DatBan> GetByIdWithKhachHangAsync(int id)
        {
            return await _context.DatBans
                .Include(db => db.KhachHang)
                .FirstOrDefaultAsync(db => db.Id == id);
        }
        public async Task UpdateBanSauThanhToanAsync(int datBanId, DateTime ThoiGianKetThuc)
        {
            var datBan = await _context.DatBans
                .Include(db => db.DatBanBans)
                .FirstOrDefaultAsync(db => db.Id == datBanId);

            if (datBan == null)
            {
                throw new InvalidOperationException("Đặt bàn không tồn tại.");
            }

            // Cập nhật trạng thái DatBan thành Hoàn thành
            datBan.TrangThai = TrangThaiBanDat.HoanThanh;
            datBan.ThoiGianKetThuc = ThoiGianKetThuc;
            await UpdateAsync(datBan);

            // Cập nhật trạng thái bàn trong BanSchedule thành Trống
            var banSchedules = await _context.BanSchedules
                .Where(bs => bs.DatBanId == datBanId)
                .ToListAsync();

            foreach (var schedule in banSchedules)
            {
                schedule.TrangThai = TrangThaiBan.Trong;
                schedule.ThoiGianKetThuc = ThoiGianKetThuc;
                _context.BanSchedules.Update(schedule);
            }

            await _context.SaveChangesAsync();
        }
        //
        public async Task<List<int>> GetCurrentBanIdsAsync(int datBanId)
        {
            return await _context.DatBan_Bans
                .Where(db => db.DatBanId == datBanId)
                .Select(db => db.BanId)
                .ToListAsync();
        }
        public (int MinAllowedBans, int MaxAllowedBans, string TableMessage) CalculateRequiredTables(int soLuongNguoi)
        {
            //if (soLuongNguoi <= 0)
            //{
            //    return (1, 1, "Vui lòng chọn 1 bàn."); // Giá trị mặc định
            //}

            int N = soLuongNguoi / MaxSoGhe;
            int soDu = soLuongNguoi % MaxSoGhe;
            int minAllowedBans, maxAllowedBans;
            string tableMessage;

            if (soDu <= 2)
            {
                minAllowedBans = N;
                maxAllowedBans = N + 1;
                tableMessage = $"Vui lòng chọn từ {minAllowedBans} đến {maxAllowedBans} bàn.";
            }
            else
            {
                minAllowedBans = N + 1;
                maxAllowedBans = N + 1;
                // Nếu số người <= MaxSoGhe, chỉ hiển thị "Vui lòng chọn 1 bàn"
                tableMessage = soLuongNguoi <= MaxSoGhe ? "Vui lòng chọn 1 bàn." : $"Vui lòng chọn {minAllowedBans} bàn.";
            }

            minAllowedBans = Math.Max(1, minAllowedBans);

            return (minAllowedBans, maxAllowedBans, tableMessage);
        }
        
        public async Task<TrangThaiBanViewModel> GetTableStatusAsync(int banId, DateTime checkTime)
        {
            var endTime = checkTime.AddHours(DefaultDurationHours).AddMinutes(bufferTimeMinutes);
            var schedule = await _context.BanSchedules
                .Where(bs => bs.BanId == banId &&
                             bs.ThoiGianBatDau <= endTime &&
                             (bs.ThoiGianKetThuc == null ||
                              (bs.ThoiGianKetThuc.HasValue && bs.ThoiGianKetThuc.Value.AddMinutes(bufferTimeMinutes) >= checkTime)) &&
                             bs.DatBan.TrangThai != TrangThaiBanDat.HoanThanh &&
                             bs.DatBan.TrangThai != TrangThaiBanDat.DaHuy)
                .OrderBy(bs => bs.ThoiGianBatDau)
                .FirstOrDefaultAsync();
            if (schedule == null)
            {
                return new TrangThaiBanViewModel
                {
                    TrangThaiValue = (int)TrangThaiBan.Trong,
                    TrangThaiDisplay = "Trống"
                };
            }

            return new TrangThaiBanViewModel
            {
                TrangThaiValue = (int)schedule.TrangThai,
                TrangThaiDisplay = schedule.TrangThai == TrangThaiBan.DaDatTruoc ? "Đã đặt trước" : "Đang sử dụng"
            };
        }
        public async Task<KhaDungBanViewModel> CheckTableAvailabilityAsync(int banId, DateTime bookingTime, DateTime bookingEndTime, int? datBanId = null)
        {
            var status = await GetTableStatusAsync(banId, bookingTime); 
            var allSchedules = await _context.BanSchedules
                .Include(bs => bs.DatBan)
                .Where(bs => bs.BanId == banId
                    && (bs.TrangThai == TrangThaiBan.DaDatTruoc || bs.TrangThai == TrangThaiBan.DangSuDung)
                    && bs.DatBan.TrangThai != TrangThaiBanDat.HoanThanh
                    && bs.DatBan.TrangThai != TrangThaiBanDat.DaHuy)
                .ToListAsync();

            // Bao gồm cả lịch liên quan đến thời gian đặt (bookingTime)
            var displaySchedules = allSchedules
                .Where(bs => bs.ThoiGianBatDau <= bookingEndTime && (bs.ThoiGianKetThuc == null || bs.ThoiGianKetThuc >= bookingTime))
                .ToList();
            //Kiểm tra xem bàn này có phải là bàn hiện tại của đơn đặt bàn k
            bool isCurrentTable = datBanId.HasValue && await _context.DatBan_Bans
                .AnyAsync(dbb => dbb.DatBanId == datBanId.Value && dbb.BanId == banId);
            bool khaDung = status.TrangThaiValue == (int)TrangThaiBan.Trong || isCurrentTable;

            if (khaDung)
            {
                khaDung = !allSchedules.Any(bs =>
                {
                    var existingEnd = bs.ThoiGianKetThuc ?? bs.ThoiGianBatDau.AddHours(DefaultDurationHours).AddMinutes(bufferTimeMinutes);
                    return (bookingTime < existingEnd && bookingEndTime > bs.ThoiGianBatDau) && (datBanId == null || bs.DatBanId != datBanId);
                });
            }
            if (isCurrentTable)
            {
                status = new TrangThaiBanViewModel
                {
                    TrangThaiValue = (int)TrangThaiBan.Trong,
                    TrangThaiDisplay = "Trống"
                };
            }
            var lichDat = displaySchedules
                .Where(bs => !datBanId.HasValue || bs.DatBanId != datBanId)
                .Select(bs => new LichDatViewModel
                {
                    DatBanId = bs.DatBanId,
                    ThoiGianBatDau = bs.ThoiGianBatDau
                })
                .ToList();

            return new KhaDungBanViewModel
            {
                KhaDung = khaDung,
                LichDat = lichDat,
                TrangThai = status,
                IsCurrentTable = isCurrentTable
            };
        }
        public async Task UpdateBanGoiMonAsync(int datBanId)
        {
            var banSchedules = await _context.BanSchedules
                .Where(bs => bs.DatBanId == datBanId)
                .ToListAsync();

            if (!banSchedules.Any())
            {
                throw new InvalidOperationException("Không tìm thấy lịch sử bàn cho đặt bàn này.");
            }

            foreach (var schedule in banSchedules)
            {
                schedule.TrangThai = TrangThaiBan.DangSuDung;
                _context.BanSchedules.Update(schedule);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Result<DatBan>> ChuyenBanAsync(int datBanId, List<int> banIds, int nhanVienId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var datBan = await _datBanRepository.GetByIdWithDatBanBansAsync(datBanId);
                    if (datBan == null)
                        return new Result<DatBan>(false, "Đơn đặt bàn không tồn tại.", null, new List<string> { "Không tìm thấy đơn." });

                    if (datBan.TrangThai != TrangThaiBanDat.DaXacNhan)
                        return new Result<DatBan>(false, "Chỉ có thể chuyển bàn cho đơn đã xác nhận.", null, new List<string> { "Trạng thái đơn không hợp lệ." });

                    // Kiểm tra số lượng bàn
                    var (minAllowedBans, maxAllowedBans, tableMessage) = CalculateRequiredTables(datBan.SoLuongNguoi);
                    if (banIds.Count < minAllowedBans || banIds.Count > maxAllowedBans)
                        return new Result<DatBan>(false, tableMessage, null, new List<string> { tableMessage });

                    // Kiểm tra khả dụng bằng CheckTableAvailabilityAsync
                    var bookingEndTime = datBan.ThoiGianDatBan.AddHours(DefaultDurationHours).AddMinutes(bufferTimeMinutes);
                    foreach (var banId in banIds)
                    {
                        var khaDung = await CheckTableAvailabilityAsync(banId, datBan.ThoiGianDatBan, bookingEndTime, datBanId);
                        if (!khaDung.KhaDung && !khaDung.IsCurrentTable)
                            return new Result<DatBan>(false, $"Bàn {banId} không khả dụng.", null, new List<string> { $"Bàn {banId} đã được đặt." });
                    }

                    // Xóa liên kết và lịch cũ
                    await _datBanRepository.RemoveRangeDatBanBansAsync(datBan.DatBanBans);
                    await _datBanRepository.RemoveBanSchedulesByDatBanIdAsync(datBanId);

                    // Tạo liên kết và lịch mới, giữ trạng thái cũ
                    var currentTrangThai = (await _context.BanSchedules.AnyAsync(bs => bs.DatBanId == datBanId && bs.TrangThai == TrangThaiBan.DangSuDung))
                        ? TrangThaiBan.DangSuDung
                        : TrangThaiBan.DaDatTruoc;

                    var now = DateTime.Now;
                    var newSchedules = banIds.Select(banId => new BanSchedule
                    {
                        BanId = banId,
                        DatBanId = datBanId,
                        ThoiGianBatDau = datBan.ThoiGianDatBan,
                        ThoiGianKetThuc = datBan.ThoiGianDatBan.AddHours(DefaultDurationHours),
                        TrangThai = currentTrangThai,
                        NgayTao = now
                    }).ToList();
                    await _datBanRepository.AddRangeBanSchedulesAsync(newSchedules);

                    var newDatBanBans = banIds.Select(banId => new DatBan_Ban
                    {
                        DatBanId = datBanId,
                        BanId = banId
                    }).ToList();
                    await _datBanRepository.AddRangeDatBanBansAsync(newDatBanBans);

                    // Cập nhật thông tin đơn
                    datBan.NhanVienId = nhanVienId;
                    await _datBanRepository.UpdateAsync(datBan);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Result<DatBan>(true, "Chuyển bàn thành công!", datBan);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new Result<DatBan>(false, $"Lỗi khi chuyển bàn: {ex.Message}", null);
                }
            }
        }

        public async Task<Result<DatBan>> HuyBanAsync(int datBanId, int nhanVienId, string? lyDoHuy)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var datBan = await _datBanRepository.GetByIdWithDatBanBansAsync(datBanId);
                    if (datBan == null)
                        return new Result<DatBan>(false, "Đơn đặt bàn không tồn tại.", null, new List<string> { "Không tìm thấy đơn." });

                    if (datBan.TrangThai != TrangThaiBanDat.ChoXacNhan && datBan.TrangThai != TrangThaiBanDat.DaXacNhan)
                        return new Result<DatBan>(false, "Chỉ có thể hủy đơn chờ xác nhận hoặc đã xác nhận.", null, new List<string> { "Trạng thái đơn không hợp lệ." });

                    datBan.TrangThai = TrangThaiBanDat.DaHuy;
                    datBan.NhanVienId = nhanVienId;
                    datBan.LyDoHuy = lyDoHuy;

                    await _datBanRepository.RemoveBanSchedulesByDatBanIdAsync(datBanId);
                    await _datBanRepository.RemoveRangeDatBanBansAsync(datBan.DatBanBans);

                    await _datBanRepository.UpdateAsync(datBan);

                    await transaction.CommitAsync();
                    return new Result<DatBan>(true, string.IsNullOrEmpty(lyDoHuy) ? "Hủy bàn thành công!" : $"Hủy bàn thành công! Lý do: {lyDoHuy}", datBan);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new Result<DatBan>(false, "Lỗi khi hủy bàn.", null, new List<string> { ex.Message });
                }
            }
        }

        public async Task<List<dynamic>> GetBanScheduleDetailsForBan(int banId, DateTime currentTime)
        {
            var banSchedules = await _context.BanSchedules
                .Include(bs => bs.DatBan).ThenInclude(db => db.KhachHang)
                .Include(bs => bs.DatBan).ThenInclude(db => db.NhanVien)
                .Include(bs => bs.DatBan).ThenInclude(db => db.DatBanBans)
                .Where(bs => bs.BanId == banId
                    && bs.ThoiGianBatDau <= currentTime
                    && (bs.ThoiGianKetThuc == null || bs.ThoiGianKetThuc >= currentTime))
                .OrderBy(bs => bs.ThoiGianBatDau)
                .ToListAsync();

            return banSchedules.Select(bs => new
            {
                DatBanId = bs.DatBanId,
                TenKhachHang = bs.DatBan.KhachHang?.TenKhachHang ?? "Khách vãng lai",
                SoDienThoai = bs.DatBan.KhachHang?.SDT ?? string.Empty,
                ThoiGianDatBan = bs.ThoiGianBatDau.ToString("HH:mm dd/MM/yyyy"),
                SoLuongKhach = bs.DatBan.SoLuongNguoi,
                BanGhep = bs.DatBan.DatBanBans.Any() ? string.Join(", ", bs.DatBan.DatBanBans.Select(dbb => dbb.Ban.TenBan)) : "Chưa xếp",
                NhanVienXuLy = bs.DatBan.NhanVien?.TenNhanVien ?? "Chưa có nhân viên",
                GhiChu = bs.DatBan.GhiChu ?? "Không có"
            }).ToList<dynamic>();
        }

       
    }
}