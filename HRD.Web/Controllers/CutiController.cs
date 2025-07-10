// HRD.Web/Controllers/CutiController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRD.Web.Models.DTOs;
using HRD.Web.Services;
using HRD.Web.Models.DTOs; // ADD THIS LINE

namespace HRD.Web.Controllers
{
    [Authorize]
    public class CutiController : Controller
    {
        private readonly ApiService _apiService;

        public CutiController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Employee: View own leave requests
        [Authorize(Roles = "Karyawan")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _apiService.GetAsync<List<CutiResponse>>("api/cuti/my-leaves");
                if (result?.Success == true)
                {
                    return View(result.Data ?? new List<CutiResponse>());
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Gagal memuat data cuti";
                    return View(new List<CutiResponse>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<CutiResponse>());
            }
        }

        // Employee: Create leave request
        [Authorize(Roles = "Karyawan")]
        public async Task<IActionResult> Create()
        {
            try
            {
                // Get leave types
                var leaveTypesResult = await _apiService.GetAsync<List<string>>("api/cuti/jenis-cuti");
                if (leaveTypesResult?.Success == true)
                {
                    ViewBag.JenisCuti = leaveTypesResult.Data ?? new List<string>();
                }
            }
            catch
            {
                ViewBag.JenisCuti = new List<string> { "Tahunan", "Sakit", "Melahirkan", "Menikah", "Duka Cita", "Izin Khusus" };
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Karyawan")]
        public async Task<IActionResult> Create(CreateCutiRequest model)
        {
            if (!ModelState.IsValid)
            {
                // Reload leave types
                try
                {
                    var leaveTypesResult = await _apiService.GetAsync<List<string>>("api/cuti/jenis-cuti");
                    ViewBag.JenisCuti = leaveTypesResult?.Data ?? new List<string> { "Tahunan", "Sakit", "Melahirkan", "Menikah", "Duka Cita", "Izin Khusus" };
                }
                catch
                {
                    ViewBag.JenisCuti = new List<string> { "Tahunan", "Sakit", "Melahirkan", "Menikah", "Duka Cita", "Izin Khusus" };
                }
                return View(model);
            }

            try
            {
                var result = await _apiService.PostAsync<CutiResponse>("api/cuti", model);
                if (result?.Success == true)
                {
                    TempData["SuccessMessage"] = "Pengajuan cuti berhasil dibuat";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Gagal membuat pengajuan cuti";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }
        }

        // HRD: Manage leave requests
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> Manage()
        {
            try
            {
                var result = await _apiService.GetAsync<List<CutiResponse>>("api/cuti");
                if (result?.Success == true)
                {
                    return View(result.Data ?? new List<CutiResponse>());
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Gagal memuat data cuti";
                    return View(new List<CutiResponse>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<CutiResponse>());
            }
        }

        // HRD: Approve/Reject leave
        [HttpPost]
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> Approve(int id, ApproveCutiRequest model)
        {
            try
            {
                var result = await _apiService.PutAsync<CutiResponse>($"api/cuti/{id}/approve", model);
                if (result?.Success == true)
                {
                    return Json(new { success = true, message = result.Message });
                }
                else
                {
                    return Json(new { success = false, message = result?.Message ?? "Gagal memproses persetujuan" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // HRD: Quick approve
        [HttpPost]
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> QuickApprove(int id)
        {
            try
            {
                var result = await _apiService.PostAsync<CutiResponse>($"api/cuti/{id}/quick-approve", new { });
                if (result?.Success == true)
                {
                    return Json(new { success = true, message = result.Message });
                }
                else
                {
                    return Json(new { success = false, message = result?.Message ?? "Gagal menyetujui cuti" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // HRD: Quick reject
        [HttpPost]
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> QuickReject(int id, string reason = "")
        {
            try
            {
                var result = await _apiService.PostAsync<CutiResponse>($"api/cuti/{id}/quick-reject", reason);
                if (result?.Success == true)
                {
                    return Json(new { success = true, message = result.Message });
                }
                else
                {
                    return Json(new { success = false, message = result?.Message ?? "Gagal menolak cuti" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // View details
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var result = await _apiService.GetAsync<CutiResponse>($"api/cuti/{id}");
                if (result?.Success == true && result.Data != null)
                {
                    return View(result.Data);
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Data cuti tidak ditemukan";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View();
            }
        }
    }
}
