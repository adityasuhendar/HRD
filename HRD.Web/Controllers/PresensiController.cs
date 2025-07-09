// HRD.Web/Controllers/PresensiController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRD.Web.Models.DTOs;
using HRD.Web.Services;
using HRD.Web.Models.DTOs; // ADD THIS LINE

namespace HRD.Web.Controllers
{
    [Authorize]
    public class PresensiController : Controller
    {
        private readonly ApiService _apiService;

        public PresensiController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Employee: View own attendance
        [Authorize(Roles = "Karyawan")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _apiService.GetAsync<List<PresensiResponse>>("api/presensi/my-attendance");
                if (result?.Success == true)
                {
                    return View(result.Data ?? new List<PresensiResponse>());
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Gagal memuat data presensi";
                    return View(new List<PresensiResponse>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<PresensiResponse>());
            }
        }

        // Employee: Clock in page
        [Authorize(Roles = "Karyawan")]
        public async Task<IActionResult> ClockIn()
        {
            try
            {
                // Check if already clocked in today
                var todayResult = await _apiService.GetAsync<PresensiResponse>("api/presensi/today");
                if (todayResult?.Success == true && todayResult.Data != null)
                {
                    ViewBag.AlreadyClocked = true;
                    ViewBag.TodayAttendance = todayResult.Data;
                }
            }
            catch
            {
                // Continue if error checking today's attendance
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Karyawan")]
        public async Task<IActionResult> ClockIn(ClockInRequest model)
        {
            try
            {
                var result = await _apiService.PostAsync<ClockInResponse>("api/presensi/clock-in", model);
                if (result?.Success == true)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Gagal melakukan clock in";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(model);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Karyawan")]
        public async Task<IActionResult> ClockOut(ClockOutRequest model)
        {
            try
            {
                var result = await _apiService.PostAsync<ClockOutResponse>("api/presensi/clock-out", model);
                if (result?.Success == true)
                {
                    TempData["SuccessMessage"] = result.Message;
                    return Json(new { success = true, message = result.Message });
                }
                else
                {
                    return Json(new { success = false, message = result?.Message ?? "Gagal melakukan clock out" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // HRD: View all attendance
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> Manage()
        {
            try
            {
                var result = await _apiService.GetAsync<List<PresensiResponse>>("api/presensi");
                if (result?.Success == true)
                {
                    return View(result.Data ?? new List<PresensiResponse>());
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Gagal memuat data presensi";
                    return View(new List<PresensiResponse>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<PresensiResponse>());
            }
        }

        // HRD: Today's summary
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> TodaySummary()
        {
            try
            {
                var result = await _apiService.GetAsync<object>("api/presensi/today-summary");
                if (result?.Success == true)
                {
                    return View(result.Data);
                }
                else
                {
                    ViewBag.Error = result?.Message ?? "Gagal memuat ringkasan hari ini";
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