using Microsoft.EntityFrameworkCore;
using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Data;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Repositories.GoiMonRepository;
using QuanLyNhaHang_DATN.Repositories.HoaDonRepository;
using QuanLyNhaHang_DATN.Services.DatBanService;

namespace QuanLyNhaHang_DATN.Services.HoaDonService
{
    public class HoaDonService : BaseService<HoaDon>, IHoaDonService
    {
        private readonly IHoaDonRepository _hoaDonRepository;
        private readonly IGoiMonRepository _goiMonRepository;
        private readonly IDatBanService _datBanService;
        private const decimal VatRate = 0.10m; // Tỷ lệ VAT 10%
        public HoaDonService(
            AppDbContext context,
            IHoaDonRepository repository,
            IGoiMonRepository goiMonRepository,
            IDatBanService datBanService)
            : base(repository, context)
        {
            _hoaDonRepository = repository;
            _goiMonRepository = goiMonRepository;
            _datBanService = datBanService;
        }

        public async Task<HoaDon> CreateHoaDonAsync(int datBanId)
        {
            var existingHoaDon = await _hoaDonRepository.GetByDatBanIdAsync(datBanId);
            if (existingHoaDon != null)
            {
                throw new InvalidOperationException("Đã tồn tại hóa đơn chưa thanh toán cho bàn này.");
            }

            var datBan = await _context.DatBans
                .FirstOrDefaultAsync(db => db.Id == datBanId);
            if (datBan == null)
            {
                throw new InvalidOperationException("Đặt bàn không tồn tại.");
            }

            // Tính tổng tiền món ăn từ GoiMon
            var tongTienGoiMon = await _goiMonRepository.CalculateTongTienByDatBanIdAsync(datBanId);
            var tienCoc = datBan.CocTien; // Tiền cọc từ DatBan

            // Tính thuế VAT: (Tổng tiền món ăn - Tiền cọc) * Tỷ lệ VAT
            var soTienTruocVat = tongTienGoiMon - tienCoc;
            var thueVat = soTienTruocVat > 0 ? soTienTruocVat * VatRate : 0; // Chỉ tính VAT nếu số tiền trước VAT > 0

            // Tính tổng tiền thanh toán: (Tổng tiền món ăn - Tiền cọc) + Thuế VAT
            var tongTienThanhToan = soTienTruocVat + thueVat;

            var hoaDon = new HoaDon
            {
                DatBanId = datBanId,
                MaHoaDon = GenerateMaHoaDon(datBanId),
                TongTienGoiMon = tongTienGoiMon, // Gán tổng tiền gọi món ban đầu
                TongTienThanhToan = tongTienThanhToan, // Thêm tổng tiền thanh toán
                TrangThai = TrangThaiHoaDon.ChuaThanhToan,
            };

            await _repository.AddAsync(hoaDon);
            await _context.SaveChangesAsync();

            return hoaDon;
        }

        public async Task ConfirmPaymentAsync(int id, PhuongThucThanhToan phuongThuc, int nhanVienId)
        {
            var hoaDon = await GetByIdWithDetailsAsync(id);
            if (hoaDon == null)
            {
                throw new InvalidOperationException("Hóa đơn không tồn tại.");
            }

            if (hoaDon.TrangThai == TrangThaiHoaDon.DaThanhToan)
            {
                throw new InvalidOperationException("Hóa đơn đã được thanh toán.");
            }
            // Kiểm tra lại tổng tiền thanh toán trước khi xác nhận
            var tienCoc = hoaDon.DatBan?.CocTien ?? 0;
            var soTienTruocVat = hoaDon.TongTienGoiMon - tienCoc;
            var thueVat = soTienTruocVat > 0 ? soTienTruocVat * VatRate : 0;
            var tongTienThanhToan = soTienTruocVat + thueVat;

            hoaDon.TrangThai = TrangThaiHoaDon.DaThanhToan;
            hoaDon.PhuongThuc = phuongThuc;
            hoaDon.NgayThanhToan = DateTime.Now;
            hoaDon.NhanVienId = nhanVienId;
          
            // Gọi DatBanService để cập nhật trạng thái DatBan và BanSchedule
            await _datBanService.UpdateBanSauThanhToanAsync(hoaDon.DatBanId, DateTime.Now);
            await _repository.UpdateAsync(hoaDon);
            await _context.SaveChangesAsync();
        }

        public async Task<HoaDon> GetHoaDonChuaThanhToanByDatBanIdAsync(int datBanId)
        {
            return await _hoaDonRepository.GetByDatBanIdAsync(datBanId);
        }

        public async Task<IEnumerable<HoaDon>> GetLichSuHoaDonAsync()
        {
            return await _hoaDonRepository.GetByTrangThaiAsync(TrangThaiHoaDon.DaThanhToan);
        }

        public async Task<HoaDon> GetByIdWithDetailsAsync(int id)
        {
            return await _context.HoaDons
                .Include(hd => hd.DatBan)
                    .ThenInclude(db => db.KhachHang)
                    .ThenInclude(kh => kh.TaiKhoan)
                .Include(hd => hd.DatBan)
                    .ThenInclude(db => db.DatBanBans)
                        .ThenInclude(dbb => dbb.Ban)
                .FirstOrDefaultAsync(hd => hd.Id == id);
        }

        public async Task<(IEnumerable<HoaDon> Items, int TotalCount)> GetPagedHoaDonsAsync(int pageIndex, int pageSize, HoaDonFilterModel filter)
        {
            IQueryable<HoaDon> query = _context.HoaDons
                .Include(hd => hd.DatBan)
                    .ThenInclude(db => db.KhachHang)
                .Include(hd => hd.DatBan)
                    .ThenInclude(db => db.DatBanBans)
                        .ThenInclude(dbb => dbb.Ban)
                        .Include(hd => hd.NhanVien);
            if (filter.TrangThai.HasValue)
            {
                query = query.Where(hd => hd.TrangThai == filter.TrangThai.Value);
            }
            if (!string.IsNullOrWhiteSpace(filter.TenKhachHang))
            {
                query = query.Where(hd => hd.DatBan.KhachHang.TenKhachHang.Contains(filter.TenKhachHang));
            }

            if (!string.IsNullOrWhiteSpace(filter.SDT))
            {
                query = query.Where(hd => hd.DatBan.KhachHang.SDT.Contains(filter.SDT));
            }

            if (!string.IsNullOrWhiteSpace(filter.MaHoaDon))
            {
                query = query.Where(hd => hd.MaHoaDon.Contains(filter.MaHoaDon));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public string GenerateMaHoaDon(int datBanId)
        {
            var datBan = _context.DatBans
                .FirstOrDefault(db => db.Id == datBanId);
            if (datBan == null)
            {
                throw new InvalidOperationException("Đặt bàn không tồn tại.");
            }

            string datePart = datBan.ThoiGianDatBan.ToString("yyyyMMdd");
            string idPart = datBanId.ToString();
            return $"{datePart}{idPart}";
        }
    }
}