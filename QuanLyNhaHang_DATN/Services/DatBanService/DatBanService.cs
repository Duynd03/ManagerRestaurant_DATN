
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
        private const int MaxSoGhe = 6; // Số ghế tối đa mỗi bàn (4 hoặc 6)
        private const double DefaultDurationHours = 2.0; // Thời gian mặc định là 2 giờ
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

        public async Task<List<Ban>> GetAvailableBansAsync()  // BỎ
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
                    var errors = new List<string>();
                    var datBan = await GetByIdWithKhachHangAsync(datBanId);
                    if (datBan == null)
                        return new Result<DatBan>(false, "Đặt bàn không tồn tại.", null, new List<string> { "Đặt bàn không tồn tại." });

                    if (datBan.TrangThai != TrangThaiBanDat.ChoXacNhan)
                        return new Result<DatBan>(false, "Đặt bàn không ở trạng thái chờ xác nhận.", null);

                    var bans = new List<Ban>();
                    foreach (var banId in banIds)
                    {
                        var ban = await _banRepository.GetByIdAsync(banId);
                        if (ban == null)
                            errors.Add($"Bàn {banId} không tồn tại.");
                        else
                            bans.Add(ban);
                    }

                    var requiredBans = (int)Math.Ceiling((double)datBan.SoLuongNguoi / MaxSoGhe);
                    if (bans.Count < requiredBans)
                        errors.Add($"Đơn đặt bàn cần {requiredBans} bàn cho {datBan.SoLuongNguoi} người. Bạn đã chọn {bans.Count} bàn.");

                    const double minimumTimeGapHours = 3.0;
                    const double bufferTimeMinutes = 30;
                    const double defaultDurationHours = 2.0;

                    var thoiGianDuKienKetThuc = datBan.ThoiGianDatBan.AddHours(defaultDurationHours);

                    var banIdsList = bans.Select(b => b.Id).ToList();
                    var existingSchedules = await _context.BanSchedules
                        .Where(bs => banIdsList.Contains(bs.BanId) && bs.DatBanId != datBanId)
                        .Where(bs => bs.TrangThai == TrangThaiBan.DaDatTruoc)
                        .ToListAsync();

                    foreach (var ban in bans)
                    {
                        var schedulesForBan = existingSchedules.Where(bs => bs.BanId == ban.Id);
                        foreach (var schedule in schedulesForBan)
                        {
                            var existingStart = schedule.ThoiGianBatDau;
                            var existingEnd = existingStart.AddHours(defaultDurationHours).AddMinutes(bufferTimeMinutes);

                            var newStart = datBan.ThoiGianDatBan;
                            var newEnd = thoiGianDuKienKetThuc;

                            if (newStart < existingEnd && newEnd > existingStart)
                                errors.Add($"Bàn {ban.TenBan} đã được đặt vào khoảng thời gian này.");

                            var timeDifference = Math.Abs((newStart - existingStart).TotalHours);
                            if (timeDifference < minimumTimeGapHours)
                                errors.Add($"Bàn {ban.TenBan} đã được đặt vào thời gian gần ({existingStart}). Vui lòng chọn thời gian cách ít nhất {minimumTimeGapHours} tiếng.");
                        }
                    }

                    if (errors.Any())
                        return new Result<DatBan>(false, "Có lỗi khi xếp bàn.", datBan, errors);

                    datBan.TrangThai = TrangThaiBanDat.DaXacNhan;
                    datBan.NhanVienId = nhanVienId;

                    foreach (var ban in bans)
                    {
                        await AddDatBanBanAsync(new DatBan_Ban
                        {
                            DatBanId = datBanId,
                            BanId = ban.Id
                        });

                        _context.BanSchedules.Add(new BanSchedule
                        {
                            BanId = ban.Id,
                            DatBanId = datBanId,
                            ThoiGianBatDau = datBan.ThoiGianDatBan,
                            ThoiGianKetThuc = datBan.ThoiGianDatBan.AddHours(defaultDurationHours), // Lưu ThoiGianKetThuc
                            TrangThai = TrangThaiBan.DaDatTruoc,
                            NgayTao = DateTime.Now
                        });

                        //ban.TrangThai = TrangThaiBan.DaDatTruoc; // Có thể bỏ nếu không cần trạng thái tĩnh
                        //await _banRepository.UpdateAsync(ban);
                    }

                    await _context.SaveChangesAsync();
                    await UpdateAsync(datBan);
                    await transaction.CommitAsync();

                    return new Result<DatBan>(true, "Xếp bàn thành công.", datBan);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new Result<DatBan>(false, $"Lỗi khi xếp bàn: {ex.Message}", null);
                }
            }
        }

        // Thêm phương thức để truy vấn BanSchedules
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
        //public async Task<TrangThaiBanViewModel> GetTableStatusAsync(int banId, DateTime currentTime)
        //{
        //    var schedule = await _context.BanSchedules
        //        .Where(bs => bs.BanId == banId &&
        //                    bs.ThoiGianBatDau <= currentTime &&
        //                    (bs.ThoiGianKetThuc == null || bs.ThoiGianKetThuc >= currentTime) &&
        //                    bs.DatBan.TrangThai != TrangThaiBanDat.HoanThanh &&
        //                    bs.DatBan.TrangThai != TrangThaiBanDat.DaHuy)
        //        .OrderBy(bs => bs.ThoiGianBatDau)
        //        .FirstOrDefaultAsync();

        //    if (schedule == null)
        //    {
        //        return new TrangThaiBanViewModel
        //        {
        //            TrangThaiValue = (int)TrangThaiBan.Trong,
        //            TrangThaiDisplay = "Trống"
        //        };
        //    }

        //    if (schedule.ThoiGianBatDau > currentTime)
        //    {
        //        return new TrangThaiBanViewModel
        //        {
        //            TrangThaiValue = (int)TrangThaiBan.Trong,
        //            TrangThaiDisplay = "Trống"
        //        };
        //    }

        //    return new TrangThaiBanViewModel
        //    {
        //        TrangThaiValue = (int)schedule.TrangThai,
        //        TrangThaiDisplay = schedule.TrangThai == TrangThaiBan.DaDatTruoc ? "Đã đặt trước" : "Đang sử dụng"
        //    };
        //}
        public async Task<TrangThaiBanViewModel> GetTableStatusAsync(int banId, DateTime currentTime)
        {
            const double DefaultDurationHours = 2.0;
            const double bufferTimeMinutes = 30;
            var endTime = currentTime.AddHours(DefaultDurationHours).AddMinutes(bufferTimeMinutes);

            var schedule = await _context.BanSchedules
                .Where(bs => bs.BanId == banId &&
                             bs.ThoiGianBatDau <= endTime &&
                             (bs.ThoiGianKetThuc == null || bs.ThoiGianKetThuc >= currentTime) &&
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
            const double bufferTimeMinutes = 30;
            const double minimumTimeGapHours = 3.0;

            var status = await GetTableStatusAsync(banId, bookingTime);

            var schedules = await _context.BanSchedules
                .Include(bs => bs.DatBan)
                .Where(bs => bs.BanId == banId &&
                            (bs.TrangThai == TrangThaiBan.DaDatTruoc || bs.TrangThai == TrangThaiBan.DangSuDung) &&
                            bs.DatBan.TrangThai != TrangThaiBanDat.HoanThanh)
                .ToListAsync();

            bool isCurrentTable = false;
            if (datBanId.HasValue)
            {
                isCurrentTable = await _context.DatBan_Bans
                    .AnyAsync(dbb => dbb.DatBanId == datBanId.Value && dbb.BanId == banId);
            }

            bool khaDung = status.TrangThaiValue == (int)TrangThaiBan.Trong || isCurrentTable;
            if (khaDung && !isCurrentTable)
            {
                khaDung = !schedules.Any(bs =>
                {
                    var existingEnd = bs.ThoiGianBatDau.AddHours(DefaultDurationHours).AddMinutes(bufferTimeMinutes);
                    var timeDifference = Math.Abs((bookingTime - bs.ThoiGianBatDau).TotalHours);
                    return (bookingTime < existingEnd && bookingEndTime > bs.ThoiGianBatDau) ||
                           timeDifference < minimumTimeGapHours;
                });
            }

            var lichDat = schedules
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

                    int requiredBans = (int)Math.Ceiling((double)datBan.SoLuongNguoi / MaxSoGhe);
                    if (banIds.Count < requiredBans)
                        return new Result<DatBan>(false, $"Cần ít nhất {requiredBans} bàn cho {datBan.SoLuongNguoi} người.", null, new List<string> { "Số bàn không đủ." });

                    var bookingEndTime = datBan.ThoiGianDatBan.AddHours(DefaultDurationHours);
                    var currentBanIds = datBan.DatBanBans?.Select(dbb => dbb.BanId).ToList() ?? new List<int>();

                    foreach (var banId in banIds)
                    {
                        var khaDung = await CheckTableAvailabilityAsync(banId, datBan.ThoiGianDatBan, bookingEndTime, datBanId);
                        if (!khaDung.KhaDung)
                            return new Result<DatBan>(false, $"Bàn {banId} không khả dụng.", null, new List<string> { $"Bàn {banId} đã được đặt." });
                    }

                    var currentSchedules = await _context.BanSchedules
                        .Where(bs => bs.DatBanId == datBanId)
                        .ToListAsync();
                    var currentTrangThai = currentSchedules.Any() && currentSchedules.Any(bs => bs.TrangThai == TrangThaiBan.DangSuDung)
                        ? TrangThaiBan.DangSuDung
                        : TrangThaiBan.DaDatTruoc;

                    await _datBanRepository.RemoveBanSchedulesByDatBanIdAsync(datBanId);
                    await _datBanRepository.RemoveRangeDatBanBansAsync(datBan.DatBanBans);

                    var now = DateTime.Now;
                    var newSchedules = banIds.Select(banId => new BanSchedule
                    {
                        BanId = banId,
                        DatBanId = datBanId,
                        ThoiGianBatDau = datBan.ThoiGianDatBan,
                        ThoiGianKetThuc = bookingEndTime,
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

                    datBan.NhanVienId = nhanVienId;
                    await _datBanRepository.UpdateAsync(datBan);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new Result<DatBan>(true, "Chuyển bàn thành công!", datBan);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new Result<DatBan>(false, "Lỗi khi chuyển bàn.", null, new List<string> { ex.Message });
                }
            }
        }

        public async Task<Result<DatBan>> HuyBanAsync(int datBanId, int nhanVienId)
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

                    // Cập nhật trạng thái đơn
                    datBan.TrangThai = TrangThaiBanDat.DaHuy;
                    datBan.NhanVienId = nhanVienId;
                   

                    // Xóa lịch và liên kết
                    await _datBanRepository.RemoveBanSchedulesByDatBanIdAsync(datBanId);
                    await _datBanRepository.RemoveRangeDatBanBansAsync(datBan.DatBanBans);

                    await _datBanRepository.UpdateAsync(datBan);

                    await transaction.CommitAsync();
                    return new Result<DatBan>(true, "Hủy bàn thành công!", datBan);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new Result<DatBan>(false, "Lỗi khi hủy bàn.", null, new List<string> { ex.Message });
                }
            }
        }
    }
}