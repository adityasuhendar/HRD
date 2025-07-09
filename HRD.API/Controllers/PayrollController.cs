// ===================================
// FIX: PayrollController Authorization
// ===================================

using HRD.API.Data;
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response;
using HRD.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // REMOVE CLASS-LEVEL AUTHORIZATION - Let methods handle their own auth
    public class PayrollController : ControllerBase
    {
        private readonly IPayrollService _payrollService;
        private readonly HRDDbContext _context;

        public PayrollController(IPayrollService payrollService, HRDDbContext context)
        {
            _payrollService = payrollService;
            _context = context;
        }

        /// <summary>
        /// Get payroll summary for specific month (HRD only)
        /// </summary>
        [HttpGet("{bulan:int}/{tahun:int}")]
        [Authorize(Roles = "HRD")] // HRD only
        public async Task<IActionResult> GetPayrollSummary(int bulan, int tahun)
        {
            try
            {
                if (bulan < 1 || bulan > 12)
                {
                    return BadRequest(new { success = false, message = "Bulan harus antara 1-12" });
                }

                var result = await _payrollService.GetPayrollSummaryAsync(bulan, tahun);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan server",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Generate monthly payroll for all active employees (HRD only)
        /// </summary>
        [HttpPost("generate")]
        [Authorize(Roles = "HRD")] // HRD only
        public async Task<IActionResult> GeneratePayroll([FromBody] GeneratePayrollRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = "Data tidak valid",
                        errors = errors
                    });
                }

                var result = await _payrollService.GenerateMonthlyPayrollAsync(request);

                if (result.Success)
                {
                    return CreatedAtAction(
                        nameof(GetPayrollSummary),
                        new { bulan = request.Bulan, tahun = request.Tahun },
                        result
                    );
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan server",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Update individual payroll record (HRD only)
        /// </summary>
        [HttpPut("{idGaji:int}")]
        [Authorize(Roles = "HRD")] // HRD only
        public async Task<IActionResult> UpdatePayroll(int idGaji, [FromBody] UpdatePayrollRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = "Data tidak valid",
                        errors = errors
                    });
                }

                var result = await _payrollService.UpdatePayrollAsync(idGaji, request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan server",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Process payment for selected payroll records (HRD only)
        /// </summary>
        [HttpPost("process-payment")]
        [Authorize(Roles = "HRD")] // HRD only
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            try
            {
                if (!ModelState.IsValid || !request.IdGajiList.Any())
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Pilih minimal satu payroll untuk diproses"
                    });
                }

                var result = await _payrollService.ProcessPaymentAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan server",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get employee payslip for specific month (Both HRD and Employee)
        /// </summary>
        [HttpGet("employee/{idKaryawan:int}/{bulan:int}/{tahun:int}")]
        [Authorize] // Allow both HRD and Karyawan
        public async Task<IActionResult> GetEmployeePayslip(int idKaryawan, int bulan, int tahun)
        {
            try
            {
                Console.WriteLine($"GetEmployeePayslip called: idKaryawan={idKaryawan}, bulan={bulan}, tahun={tahun}");
                Console.WriteLine($"User role: {User.FindFirst("role")?.Value}");

                // Security check - employees can only access their own payslip
                if (User.IsInRole("Karyawan"))
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    Console.WriteLine($"Employee security check: userIdClaim='{userIdClaim}'");

                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        // Get employee data to verify ownership
                        var employee = await _context.Karyawan
                            .FirstOrDefaultAsync(k => k.IdPengguna == userId);

                        Console.WriteLine($"Found employee: {employee?.NamaLengkap}, IdKaryawan={employee?.IdKaryawan}");

                        if (employee == null || employee.IdKaryawan != idKaryawan)
                        {
                            Console.WriteLine($"Access denied: employee.IdKaryawan={employee?.IdKaryawan} != requested={idKaryawan}");
                            return Forbid();
                        }

                        Console.WriteLine("Employee security check passed");
                    }
                    else
                    {
                        Console.WriteLine("Failed to parse user ID from claims");
                        return Forbid();
                    }
                }
                else
                {
                    Console.WriteLine("User is not Karyawan role, allowing access");
                }

                Console.WriteLine($"Looking for payroll: IdKaryawan={idKaryawan}, Bulan={bulan}, Tahun={tahun}");

                var payroll = await _context.Gaji
                    .Include(g => g.Karyawan)
                    .FirstOrDefaultAsync(g => g.IdKaryawan == idKaryawan && g.Bulan == bulan && g.Tahun == tahun);

                if (payroll == null)
                {
                    Console.WriteLine("Payroll not found in database");
                    return NotFound(new { success = false, message = "Slip gaji tidak ditemukan" });
                }

                Console.WriteLine($"Found payroll: IdGaji={payroll.IdGaji}, TotalGaji={payroll.TotalGaji}, Status={payroll.StatusBayar}");

                var response = new PayrollResponse
                {
                    IdGaji = payroll.IdGaji,
                    IdKaryawan = payroll.IdKaryawan,
                    NIK = payroll.Karyawan.NIK,
                    NamaLengkap = payroll.Karyawan.NamaLengkap,
                    Posisi = payroll.Karyawan.Posisi,
                    Divisi = payroll.Karyawan.Divisi,
                    Bulan = payroll.Bulan,
                    Tahun = payroll.Tahun,
                    GajiPokok = payroll.GajiPokok,
                    Tunjangan = payroll.Tunjangan,
                    Potongan = payroll.Potongan,
                    TotalJamKerja = payroll.TotalJamKerja,
                    TotalGaji = payroll.TotalGaji,
                    StatusBayar = payroll.StatusBayar,
                    TglDibuat = payroll.TglDibuat,
                    Keterangan = payroll.Keterangan
                };

                Console.WriteLine("Returning successful payroll response");
                return Ok(new { success = true, data = response, message = "Slip gaji berhasil diambil" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetEmployeePayslip Exception: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan server",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get employee payslip history (Both HRD and Employee)
        /// </summary>
        [HttpGet("employee/{idKaryawan:int}/history")]
        [Authorize] // Allow both HRD and Karyawan
        public async Task<IActionResult> GetEmployeePayslipHistory(int idKaryawan, int? tahun = null)
        {
            try
            {
                // Security check for employee role
                if (User.IsInRole("Karyawan"))
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        var employee = await _context.Karyawan
                            .FirstOrDefaultAsync(k => k.IdPengguna == userId);

                        if (employee == null || employee.IdKaryawan != idKaryawan)
                        {
                            return Forbid();
                        }
                    }
                    else
                    {
                        return Forbid();
                    }
                }

                var query = _context.Gaji
                    .Include(g => g.Karyawan)
                    .Where(g => g.IdKaryawan == idKaryawan);

                if (tahun.HasValue)
                {
                    query = query.Where(g => g.Tahun == tahun.Value);
                }

                var payrolls = await query
                    .OrderByDescending(g => g.Tahun)
                    .ThenByDescending(g => g.Bulan)
                    .Select(g => new PayrollResponse
                    {
                        IdGaji = g.IdGaji,
                        IdKaryawan = g.IdKaryawan,
                        NIK = g.Karyawan.NIK,
                        NamaLengkap = g.Karyawan.NamaLengkap,
                        Posisi = g.Karyawan.Posisi,
                        Divisi = g.Karyawan.Divisi,
                        Bulan = g.Bulan,
                        Tahun = g.Tahun,
                        GajiPokok = g.GajiPokok,
                        Tunjangan = g.Tunjangan,
                        Potongan = g.Potongan,
                        TotalJamKerja = g.TotalJamKerja,
                        TotalGaji = g.TotalGaji,
                        StatusBayar = g.StatusBayar,
                        TglDibuat = g.TglDibuat,
                        Keterangan = g.Keterangan
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = payrolls, message = "Riwayat slip gaji berhasil diambil" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan server",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get payroll by ID (Both HRD and Employee)
        /// </summary>
        [HttpGet("{idGaji:int}")]
        [Authorize] // Allow both HRD and Karyawan
        public async Task<IActionResult> GetPayrollById(int idGaji)
        {
            try
            {
                var payroll = await _context.Gaji
                    .Include(g => g.Karyawan)
                    .FirstOrDefaultAsync(g => g.IdGaji == idGaji);

                if (payroll == null)
                {
                    return NotFound(new { success = false, message = "Data payroll tidak ditemukan" });
                }

                // Security check for employee role
                if (User.IsInRole("Karyawan"))
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out int userId))
                    {
                        var employee = await _context.Karyawan
                            .FirstOrDefaultAsync(k => k.IdPengguna == userId);

                        if (employee == null || employee.IdKaryawan != payroll.IdKaryawan)
                        {
                            return Forbid();
                        }
                    }
                    else
                    {
                        return Forbid();
                    }
                }

                var response = new PayrollResponse
                {
                    IdGaji = payroll.IdGaji,
                    IdKaryawan = payroll.IdKaryawan,
                    NIK = payroll.Karyawan.NIK,
                    NamaLengkap = payroll.Karyawan.NamaLengkap,
                    Posisi = payroll.Karyawan.Posisi,
                    Divisi = payroll.Karyawan.Divisi,
                    Bulan = payroll.Bulan,
                    Tahun = payroll.Tahun,
                    GajiPokok = payroll.GajiPokok,
                    Tunjangan = payroll.Tunjangan,
                    Potongan = payroll.Potongan,
                    TotalJamKerja = payroll.TotalJamKerja,
                    TotalGaji = payroll.TotalGaji,
                    StatusBayar = payroll.StatusBayar,
                    TglDibuat = payroll.TglDibuat,
                    Keterangan = payroll.Keterangan
                };

                return Ok(new { success = true, data = response, message = "Data payroll berhasil diambil" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan server",
                    error = ex.Message
                });
            }
        }
    }
}