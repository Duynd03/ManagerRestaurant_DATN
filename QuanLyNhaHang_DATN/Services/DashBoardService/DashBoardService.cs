using QuanLyNhaHang_DATN.Areas.Admin.ViewModels;
using QuanLyNhaHang_DATN.Models;
using QuanLyNhaHang_DATN.Services.HoaDonService;
using QuanLyNhaHang_DATN.ViewModels;

namespace QuanLyNhaHang_DATN.Services.DashBoardService
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IHoaDonService _hoaDonService;

        public DashBoardService(IHoaDonService hoaDonService)
        {
            _hoaDonService = hoaDonService;
        }

        public async Task<DashBoardViewModel> GetDashBoardDataAsync(string filterType = "daily", string fromDate = null, string toDate = null, int? month = null, int? year = null, int? compareYear = null)
        {
            var model = new DashBoardViewModel();
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today.AddDays(1).AddTicks(-1);
            DateTime? compareStartDate = null;
            DateTime? compareEndDate = null;
            bool showChart = false;
            string chartTitle = "";
            string compareTitle = "";

            switch (filterType)
            {
                case "daily":
                    if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(toDate))
                    {
                        if (DateTime.TryParse(fromDate, out var parsedFromDate) &&
                            DateTime.TryParse(toDate, out var parsedToDate))
                        {
                            startDate = parsedFromDate.Date;
                            endDate = parsedToDate.Date.AddDays(1).AddTicks(-1);
                            showChart = (endDate.Date - startDate.Date).TotalDays >= 1;
                            chartTitle = $"từ {startDate:dd/MM/yyyy} đến {endDate:dd/MM/yyyy}";
                        }
                    }
                    break;

                case "monthly":
                    if (month.HasValue && year.HasValue)
                    {
                        startDate = new DateTime(year.Value, month.Value, 1);
                        endDate = startDate.AddMonths(1).AddTicks(-1);
                        showChart = true;
                        chartTitle = $"tháng {month.Value}/{year.Value}";
                    }
                    break;

                case "yearly":
                    if (year.HasValue)
                    {
                        startDate = new DateTime(year.Value, 1, 1);
                        endDate = startDate.AddYears(1).AddTicks(-1);
                        showChart = true;
                        chartTitle = $"năm {year.Value}";
                    }
                    if (compareYear.HasValue)
                    {
                        compareStartDate = new DateTime(compareYear.Value, 1, 1);
                        compareEndDate = compareStartDate.Value.AddYears(1).AddTicks(-1);
                        compareTitle = $"năm {compareYear.Value}";
                    }
                    break;
            }

            var filter = new HoaDonFilterModel { TrangThai = TrangThaiHoaDon.DaThanhToan };
            var (hoaDons, _) = await _hoaDonService.GetPagedHoaDonsAsync(1, int.MaxValue, filter);

            var filteredHoaDons = hoaDons
                .Where(hd => hd.NgayThanhToan >= startDate && hd.NgayThanhToan <= endDate)
                .ToList();

            model.SoHoaDonDaThanhToan = filteredHoaDons.Count;
            model.DoanhThu = filteredHoaDons.Sum(hd => hd.TongTien);
            model.SoLuongKhachHang = filteredHoaDons
                .Where(hd => hd.DatBan?.KhachHangId != null)
                .Select(hd => hd.DatBan.KhachHangId)
                .Distinct()
                .Count();
            model.ChartTitle = chartTitle;
            model.CompareTitle = compareTitle;

            model.DoanhThuTheoThoiGian = showChart
                ? GenerateChartData(filteredHoaDons, startDate, endDate, filterType)
                : new List<DoanhThuTheoThoiGian>();

            if (filterType == "yearly" && compareStartDate.HasValue && compareEndDate.HasValue)
            {
                var compareHoaDons = hoaDons
                    .Where(hd => hd.NgayThanhToan >= compareStartDate && hd.NgayThanhToan <= compareEndDate)
                    .ToList();
                model.DoanhThuSoSanh = GenerateChartData(compareHoaDons, compareStartDate.Value, compareEndDate.Value, filterType);
            }

            model.ShowChart = showChart;
            return model;
        }

        private List<DoanhThuTheoThoiGian> GenerateChartData(List<HoaDon> hoaDons, DateTime startDate, DateTime endDate, string filterType)
        {
            var result = new List<DoanhThuTheoThoiGian>();

            switch (filterType)
            {
                case "daily":
                    for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                    {
                        var dayEnd = date.AddDays(1).AddTicks(-1);
                        var doanhThu = hoaDons
                            .Where(hd => hd.NgayThanhToan >= date && hd.NgayThanhToan <= dayEnd)
                            .Sum(hd => hd.TongTien);

                        result.Add(new DoanhThuTheoThoiGian
                        {
                            Label = date.ToString("dd/MM"),
                            DoanhThu = doanhThu
                        });
                    }
                    break;

                case "monthly":
                    for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                    {
                        var dayEnd = date.AddDays(1).AddTicks(-1);
                        var doanhThu = hoaDons
                            .Where(hd => hd.NgayThanhToan >= date && hd.NgayThanhToan <= dayEnd)
                            .Sum(hd => hd.TongTien);

                        result.Add(new DoanhThuTheoThoiGian
                        {
                            Label = date.ToString("dd"),
                            DoanhThu = doanhThu
                        });
                    }
                    break;

                case "yearly":
                    for (var month = startDate.Month; month <= 12; month++)
                    {
                        var monthStart = new DateTime(startDate.Year, month, 1);
                        var monthEnd = monthStart.AddMonths(1).AddTicks(-1);
                        var doanhThu = hoaDons
                            .Where(hd => hd.NgayThanhToan >= monthStart && hd.NgayThanhToan <= monthEnd)
                            .Sum(hd => hd.TongTien);

                        result.Add(new DoanhThuTheoThoiGian
                        {
                            Label = $"Tháng {month}",
                            DoanhThu = doanhThu
                        });
                    }
                    break;
            }

            return result;
        }
    }
}