// ===================================
// UPDATED: HRD.Web/Controllers/EmployeeController.cs - Simple Version
// ===================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRD.Web.Models.DTOs;
using HRD.Web.Services;
using System.Security.Claims;

namespace HRD.Web.Controllers
{
    [Authorize(Roles = "Karyawan")]
    public class EmployeeController : Controller
    {
        private readonly ApiService _apiService;

        public EmployeeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Dashboard for employees
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var currentUserId = GetCurrentUserId(); // BACK TO SYNC
                if (currentUserId == 0)
                {
                    ViewBag.Error = "User ID tidak valid - tidak dapat mengidentifikasi pengguna";
                    return View();
                }

                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                // Get current user's employee data
                var employeeResult = await _apiService.GetAsync<KaryawanResponse>($"api/karyawan/by-user/{currentUserId}");

                if (employeeResult?.Success == true && employeeResult.Data != null)
                {
                    ViewBag.Employee = employeeResult.Data;

                    // Get current month payslip
                    var payslipResult = await _apiService.GetAsync<PayrollResponse>($"api/payroll/employee/{employeeResult.Data.IdKaryawan}/{currentMonth}/{currentYear}");
                    ViewBag.CurrentPayslip = payslipResult?.Data;

                    return View();
                }
                else
                {
                    ViewBag.Error = "Data karyawan tidak ditemukan";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View();
            }
        }

        // Current month payslip
        public async Task<IActionResult> CurrentPayslip()
        {
            try
            {
                Console.WriteLine("=== CurrentPayslip Method Started ===");

                var currentUserId = GetCurrentUserId(); // BACK TO SYNC
                Console.WriteLine($"GetCurrentUserId returned: {currentUserId}");

                if (currentUserId == 0)
                {
                    Console.WriteLine("ERROR: currentUserId is 0, stopping execution");
                    ViewBag.Error = "User ID tidak valid - tidak dapat mengidentifikasi pengguna";
                    return View();
                }

                var currentMonth = DateTime.Now.Month;
                var currentYear = DateTime.Now.Year;

                Console.WriteLine($"Calling API: api/karyawan/by-user/{currentUserId}");

                // Get employee data first
                var employeeResult = await _apiService.GetAsync<KaryawanResponse>($"api/karyawan/by-user/{currentUserId}");

                Console.WriteLine($"Employee API Response: Success={employeeResult?.Success}, Message='{employeeResult?.Message}'");

                if (employeeResult?.Success == true && employeeResult.Data != null)
                {
                    Console.WriteLine($"Found employee: {employeeResult.Data.NamaLengkap} (ID: {employeeResult.Data.IdKaryawan})");
                    ViewBag.Employee = employeeResult.Data;

                    Console.WriteLine($"Calling Payroll API: api/payroll/employee/{employeeResult.Data.IdKaryawan}/{currentMonth}/{currentYear}");

                    // Get current month payslip
                    var payslipResult = await _apiService.GetAsync<PayrollResponse>($"api/payroll/employee/{employeeResult.Data.IdKaryawan}/{currentMonth}/{currentYear}");

                    Console.WriteLine($"Payroll API Response: Success={payslipResult?.Success}, Message='{payslipResult?.Message}'");

                    if (payslipResult?.Success == true && payslipResult.Data != null)
                    {
                        Console.WriteLine($"Found payslip: Total Gaji = {payslipResult.Data.TotalGaji}");
                        ViewBag.MonthName = GetMonthName(currentMonth);
                        ViewBag.Year = currentYear;
                        return View(payslipResult.Data);
                    }
                    else
                    {
                        Console.WriteLine("No payslip found for current month");
                        ViewBag.Message = "Slip gaji bulan ini belum tersedia";
                        ViewBag.MonthName = GetMonthName(currentMonth);
                        ViewBag.Year = currentYear;
                        return View();
                    }
                }
                else
                {
                    Console.WriteLine("Employee not found or API error");
                    ViewBag.Error = employeeResult?.Message ?? "Data karyawan tidak ditemukan";
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CurrentPayslip Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                ViewBag.Error = $"Error: {ex.Message}";
                return View();
            }
        }

        // Payslip history
        public async Task<IActionResult> PayslipHistory(int? tahun = null)
        {
            try
            {
                var currentUserId = GetCurrentUserId(); // BACK TO SYNC
                if (currentUserId == 0)
                {
                    ViewBag.Error = "User ID tidak valid - tidak dapat mengidentifikasi pengguna";
                    return View(new List<PayrollResponse>());
                }

                var targetYear = tahun ?? DateTime.Now.Year;

                // Get employee data first
                var employeeResult = await _apiService.GetAsync<KaryawanResponse>($"api/karyawan/by-user/{currentUserId}");

                if (employeeResult?.Success == true && employeeResult.Data != null)
                {
                    ViewBag.Employee = employeeResult.Data;

                    // Get payslip history
                    var historyResult = await _apiService.GetAsync<List<PayrollResponse>>($"api/payroll/employee/{employeeResult.Data.IdKaryawan}/history?tahun={targetYear}");

                    ViewBag.Year = targetYear;
                    ViewBag.Years = GetAvailableYears();

                    return View(historyResult?.Data ?? new List<PayrollResponse>());
                }
                else
                {
                    ViewBag.Error = "Data karyawan tidak ditemukan";
                    return View(new List<PayrollResponse>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<PayrollResponse>());
            }
        }

        // Download payslip PDF
        public async Task<IActionResult> DownloadPayslip(int idGaji)
        {
            try
            {
                var currentUserId = GetCurrentUserId(); // BACK TO SYNC
                if (currentUserId == 0)
                {
                    TempData["ErrorMessage"] = "User ID tidak valid";
                    return RedirectToAction("PayslipHistory");
                }

                // Get payslip data
                var result = await _apiService.GetAsync<PayrollResponse>($"api/payroll/{idGaji}");

                if (result?.Success == true && result.Data != null)
                {
                    // Simple security check - ensure user can only download their own payslip
                    var employeeResult = await _apiService.GetAsync<KaryawanResponse>($"api/karyawan/by-user/{currentUserId}");

                    if (employeeResult?.Success == true &&
                        employeeResult.Data != null &&
                        employeeResult.Data.IdKaryawan == result.Data.IdKaryawan)
                    {
                        // For now, redirect to a detailed view (PDF generation can be added later)
                        return RedirectToAction("PayslipDetail", new { idGaji = idGaji });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Anda tidak memiliki akses ke slip gaji ini";
                        return RedirectToAction("PayslipHistory");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Slip gaji tidak ditemukan";
                    return RedirectToAction("PayslipHistory");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("PayslipHistory");
            }
        }

        // Payslip detail view
        public async Task<IActionResult> PayslipDetail(int idGaji)
        {
            try
            {
                var currentUserId = GetCurrentUserId(); // BACK TO SYNC
                if (currentUserId == 0)
                {
                    TempData["ErrorMessage"] = "User ID tidak valid";
                    return RedirectToAction("PayslipHistory");
                }

                // Get payslip data
                var result = await _apiService.GetAsync<PayrollResponse>($"api/payroll/{idGaji}");

                if (result?.Success == true && result.Data != null)
                {
                    // Security check
                    var employeeResult = await _apiService.GetAsync<KaryawanResponse>($"api/karyawan/by-user/{currentUserId}");

                    if (employeeResult?.Success == true &&
                        employeeResult.Data != null &&
                        employeeResult.Data.IdKaryawan == result.Data.IdKaryawan)
                    {
                        ViewBag.Employee = employeeResult.Data;
                        ViewBag.MonthName = GetMonthName(result.Data.Bulan);
                        return View(result.Data);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Anda tidak memiliki akses ke slip gaji ini";
                        return RedirectToAction("PayslipHistory");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Slip gaji tidak ditemukan";
                    return RedirectToAction("PayslipHistory");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return RedirectToAction("PayslipHistory");
            }
        }

        // SIMPLE: Get user ID from claims - works for ALL users automatically!
        private int GetCurrentUserId()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"NameIdentifier claim: '{userIdClaim}'");

                if (int.TryParse(userIdClaim, out int userId))
                {
                    Console.WriteLine($"Successfully parsed user ID: {userId}");
                    return userId;
                }

                Console.WriteLine("Failed to parse user ID from claims");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user ID from claims: {ex.Message}");
                return 0;
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

        // <summary>
        /// Salary History with Charts and Analytics
        /// </summary>
        public async Task<IActionResult> SalaryHistory(int? tahun = null)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                if (currentUserId == 0)
                {
                    ViewBag.Error = "User ID tidak valid - tidak dapat mengidentifikasi pengguna";
                    return View(new List<PayrollResponse>());
                }

                var targetYear = tahun ?? DateTime.Now.Year;
                var previousYear = targetYear - 1;

                // Get employee data
                var employeeResult = await _apiService.GetAsync<KaryawanResponse>($"api/karyawan/by-user/{currentUserId}");

                if (employeeResult?.Success == true && employeeResult.Data != null)
                {
                    ViewBag.Employee = employeeResult.Data;

                    // Get current year payslip history
                    var currentYearResult = await _apiService.GetAsync<List<PayrollResponse>>($"api/payroll/employee/{employeeResult.Data.IdKaryawan}/history?tahun={targetYear}");

                    // Get previous year for comparison
                    var previousYearResult = await _apiService.GetAsync<List<PayrollResponse>>($"api/payroll/employee/{employeeResult.Data.IdKaryawan}/history?tahun={previousYear}");

                    // Get all available years for dropdown
                    var allHistoryResult = await _apiService.GetAsync<List<PayrollResponse>>($"api/payroll/employee/{employeeResult.Data.IdKaryawan}/history");

                    ViewBag.Year = targetYear;
                    ViewBag.PreviousYear = previousYear;
                    ViewBag.Years = GetAvailableYears();

                    // Calculate analytics
                    var currentYearData = currentYearResult?.Data ?? new List<PayrollResponse>();
                    var previousYearData = previousYearResult?.Data ?? new List<PayrollResponse>();
                    var allData = allHistoryResult?.Data ?? new List<PayrollResponse>();

                    // Analytics calculations
                    ViewBag.Analytics = new
                    {
                        CurrentYear = new
                        {
                            TotalPayslips = currentYearData.Count,
                            TotalSalary = currentYearData.Sum(p => p.TotalGaji),
                            AverageSalary = currentYearData.Any() ? currentYearData.Average(p => p.TotalGaji) : 0,
                            HighestSalary = currentYearData.Any() ? currentYearData.Max(p => p.TotalGaji) : 0,
                            LowestSalary = currentYearData.Any() ? currentYearData.Min(p => p.TotalGaji) : 0,
                            TotalAllowances = currentYearData.Sum(p => p.Tunjangan),
                            TotalDeductions = currentYearData.Sum(p => p.Potongan)
                        },
                        PreviousYear = new
                        {
                            TotalSalary = previousYearData.Sum(p => p.TotalGaji),
                            AverageSalary = previousYearData.Any() ? previousYearData.Average(p => p.TotalGaji) : 0
                        },
                        AllTime = new
                        {
                            TotalEarnings = allData.Sum(p => p.TotalGaji),
                            TotalMonths = allData.Count,
                            FirstPayslip = allData.OrderBy(p => p.Tahun).ThenBy(p => p.Bulan).FirstOrDefault()?.TglDibuat,
                            LatestPayslip = allData.OrderByDescending(p => p.Tahun).ThenByDescending(p => p.Bulan).FirstOrDefault()?.TglDibuat
                        }
                    };

                    return View(currentYearData);
                }
                else
                {
                    ViewBag.Error = "Data karyawan tidak ditemukan";
                    return View(new List<PayrollResponse>());
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error: {ex.Message}";
                return View(new List<PayrollResponse>());
            }
        }
    }
}