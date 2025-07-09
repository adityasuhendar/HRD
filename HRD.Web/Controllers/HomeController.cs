// HRD.Web/Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRD.Web.Models.ViewModels;
using HRD.Web.Services;
using HRD.Web.Models.DTOs; // ADD THIS LINE

namespace HRD.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApiService _apiService;

        public HomeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            // Redirect based on role
            if (User.IsInRole("HRD"))
            {
                return RedirectToAction("HRDDashboard");
            }
            else
            {
                return RedirectToAction("EmployeeDashboard");
            }
        }

        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> HRDDashboard()
        {
            var model = new DashboardViewModel
            {
                NamaKaryawan = HttpContext.Session.GetString("NamaLengkap") ?? "HRD",
                Peran = "HRD",
                TanggalHariIni = DateTime.Today
            };

            try
            {
                // Get pending leave requests
                var cutiResult = await _apiService.GetAsync<List<CutiResponse>>("api/cuti?status=Pending");
                if (cutiResult?.Success == true && cutiResult.Data != null)
                {
                    model.CutiMenungguList = cutiResult.Data;
                    model.CutiMenungguApproval = cutiResult.Data.Count;
                }

                // Get today's attendance summary  
                var todaySummaryResult = await _apiService.GetAsync<object>("api/presensi/today-summary");
                if (todaySummaryResult?.Success == true)
                {
                    // Parse the summary data
                    // This would need proper deserialization based on the API response structure
                }
            }
            catch (Exception ex)
            {
                // Log error and continue with empty data
                ViewBag.Error = $"Error loading dashboard data: {ex.Message}";
            }

            return View(model);
        }

        // HRD.Web/Controllers/HomeController.cs - EmployeeDashboard method
        [Authorize(Roles = "Karyawan")]
        public async Task<IActionResult> EmployeeDashboard()
        {
            var model = new DashboardViewModel
            {
                // ISSUE: This gets different values depending on source
                NamaKaryawan = HttpContext.Session.GetString("NamaLengkap") ?? "Karyawan", // ❌ Fallback to role
                Peran = "Karyawan",
                TanggalHariIni = DateTime.Today
            };

            // Sometimes API call succeeds, sometimes fails
            try
            {
                var todayAttendanceResult = await _apiService.GetAsync<PresensiResponse>("api/presensi/today");
                if (todayAttendanceResult?.Success == true)
                {
                    model.PresensiHariIni = todayAttendanceResult.Data;
                }

                var cutiResult = await _apiService.GetAsync<List<CutiResponse>>("api/cuti/my-leaves");
                if (cutiResult?.Success == true && cutiResult.Data != null)
                {
                    model.CutiTerbaru = cutiResult.Data.Take(5).ToList();
                    model.TotalCutiMenunggu = cutiResult.Data.Count(c => c.StatusPersetujuan == "Pending");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading dashboard data: {ex.Message}";
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}