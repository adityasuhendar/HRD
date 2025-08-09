using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRD.Web.Models.DTOs;
using HRD.Web.Services;

namespace HRD.Web.Controllers
{
    [Authorize(Roles = "HRD")]
    public class PayrollController : Controller
    {
        private readonly ApiService _apiService;

        public PayrollController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPayrollSummary(int bulan, int tahun)
        {
            try
            {
                var result = await _apiService.GetAsync<PayrollSummaryResponse>($"api/payroll/{bulan}/{tahun}");
                return Json(new { success = result?.Success ?? false, data = result?.Data, message = result?.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePayroll([FromBody] GeneratePayrollRequest request)
        {
            try
            {
                var result = await _apiService.PostAsync<PayrollSummaryResponse>("api/payroll/generate", request);
                return Json(new { success = result?.Success ?? false, data = result?.Data, message = result?.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] List<int> idGajiList)
        {
            try
            {
                if (!idGajiList.Any())
                {
                    return Json(new { success = false, message = "Pilih minimal satu payroll untuk diproses" });
                }

                var request = new { IdGajiList = idGajiList };
                var result = await _apiService.PostAsync<string>("api/payroll/process-payment", request);

                return Json(new
                {
                    success = result?.Success ?? false,
                    message = result?.Message ?? "Gagal memproses pembayaran"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        public async Task<IActionResult> Reports(int? bulan = null, int? tahun = null, string? divisi = null)
        {
            try
            {
                var currentMonth = bulan ?? DateTime.Now.Month;
                var currentYear = tahun ?? DateTime.Now.Year;

                ViewBag.CurrentMonth = currentMonth;
                ViewBag.CurrentYear = currentYear;
                ViewBag.SelectedDivisi = divisi;
                ViewBag.MonthName = GetMonthName(currentMonth);
                ViewBag.AvailableYears = GetAvailableYears();
                ViewBag.AvailableMonths = GetAvailableMonths();

                // Get all employees for division filter
                var employeesResult = await _apiService.GetAsync<List<KaryawanResponse>>("api/karyawan");
                if (employeesResult?.Success == true && employeesResult.Data != null)
                {
                    var divisions = employeesResult.Data
                        .Select(e => e.Divisi)
                        .Distinct()
                        .OrderBy(d => d)
                        .ToList();
                    ViewBag.AvailableDivisions = divisions;
                }

                // Get payroll data for the selected period
                var payrollUrl = $"api/payroll/{currentMonth}/{currentYear}";
                if (!string.IsNullOrEmpty(divisi))
                {
                    payrollUrl += $"?divisi={divisi}";
                }

                var payrollResult = await _apiService.GetAsync<PayrollSummaryResponse>(payrollUrl);
                
                if (payrollResult?.Success == true && payrollResult.Data != null)
                {
                    return View(payrollResult.Data);
                }
                else
                {
                    ViewBag.Message = payrollResult?.Message ?? "Data payroll tidak ditemukan untuk periode yang dipilih";
                    return View(new PayrollSummaryResponse());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new PayrollSummaryResponse());
            }
        }

        private string GetMonthName(int month)
        {
            string[] months = { "", "Januari", "Februari", "Maret", "April", "Mei", "Juni",
                               "Juli", "Agustus", "September", "Oktober", "November", "Desember" };
            return months[month];
        }

        private List<int> GetAvailableYears()
        {
            var currentYear = DateTime.Now.Year;
            return Enumerable.Range(currentYear - 2, 5).ToList();
        }

        private List<object> GetAvailableMonths()
        {
            return new List<object>
            {
                new { Value = 1, Name = "Januari" },
                new { Value = 2, Name = "Februari" },
                new { Value = 3, Name = "Maret" },
                new { Value = 4, Name = "April" },
                new { Value = 5, Name = "Mei" },
                new { Value = 6, Name = "Juni" },
                new { Value = 7, Name = "Juli" },
                new { Value = 8, Name = "Agustus" },
                new { Value = 9, Name = "September" },
                new { Value = 10, Name = "Oktober" },
                new { Value = 11, Name = "November" },
                new { Value = 12, Name = "Desember" }
            };
        }

        public async Task<IActionResult> Settings()
        {
            try
            {
                // Get all employees to determine positions/divisions
                var employeesResult = await _apiService.GetAsync<List<KaryawanResponse>>("api/karyawan");
                
                if (employeesResult?.Success == true && employeesResult.Data != null)
                {
                    var positions = employeesResult.Data
                        .Select(e => e.Posisi)
                        .Distinct()
                        .OrderBy(p => p)
                        .ToList();
                        
                    var divisions = employeesResult.Data
                        .Select(e => e.Divisi)
                        .Distinct()
                        .OrderBy(d => d)
                        .ToList();

                    ViewBag.Positions = positions;
                    ViewBag.Divisions = divisions;
                }

                // Mock settings data - in real app, this would come from database
                var settingsData = new
                {
                    PayrollSchedule = new
                    {
                        PayrollDay = 25,
                        CutOffDay = 22,
                        WorkingHoursPerDay = 8,
                        WorkingDaysPerMonth = 22
                    },
                    Allowances = new[]
                    {
                        new { Type = "Transport", Amount = 500000, IsActive = true },
                        new { Type = "Makan", Amount = 750000, IsActive = true },
                        new { Type = "Komunikasi", Amount = 200000, IsActive = false },
                        new { Type = "Kesehatan", Amount = 300000, IsActive = true }
                    },
                    Deductions = new[]
                    {
                        new { Type = "BPJS Ketenagakerjaan", Percentage = 2.0, Amount = (int?)null, IsActive = true },
                        new { Type = "BPJS Kesehatan", Percentage = 1.0, Amount = (int?)null, IsActive = true },
                        new { Type = "PPh 21", Percentage = 5.0, Amount = (int?)null, IsActive = true },
                        new { Type = "Potongan Lain", Percentage = 0.0, Amount = (int?)0, IsActive = false }
                    },
                    SalaryByPosition = new[]
                    {
                        new { Position = "Software Engineer", BaseSalary = 8000000 },
                        new { Position = "UI/UX Designer", BaseSalary = 6500000 },
                        new { Position = "Project Manager", BaseSalary = 12000000 },
                        new { Position = "QA Tester", BaseSalary = 5500000 },
                        new { Position = "DevOps Engineer", BaseSalary = 9000000 },
                        new { Position = "Business Analyst", BaseSalary = 7000000 }
                    }
                };

                return View(settingsData);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading settings: {ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSettings([FromBody] object settingsData)
        {
            try
            {
                // In real implementation, this would save to database via API
                // For now, just return success
                return Json(new { success = true, message = "Settings berhasil disimpan" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}