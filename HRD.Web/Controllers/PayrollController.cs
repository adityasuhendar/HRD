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
    }
}