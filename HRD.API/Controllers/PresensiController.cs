// HRD.API/Controllers/PresensiController.cs - COMPLETE VERSION
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using HRD.API.Data;
using HRD.API.Models.DTOs.Request;
using HRD.API.Services.Interfaces;

namespace HRD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class PresensiController : ControllerBase
    {
        private readonly IPresensiService _presensiService;
        private readonly HRDDbContext _context;

        public PresensiController(IPresensiService presensiService, HRDDbContext context)
        {
            _presensiService = presensiService;
            _context = context;
        }

        /// <summary>
        /// Clock In (Employee)
        /// </summary>
        /// <param name="request">Clock in data</param>
        /// <returns>Clock in response</returns>
        [HttpPost("clock-in")]
        public async Task<IActionResult> ClockIn([FromBody] ClockInRequest request)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int penggunaId))
                {
                    return BadRequest(new { success = false, message = "User ID tidak valid" });
                }

                // Find employee by user ID
                var employee = await _context.Karyawan
                    .FirstOrDefaultAsync(k => k.IdPengguna == penggunaId);

                if (employee == null)
                {
                    return BadRequest(new { success = false, message = "Data karyawan tidak ditemukan" });
                }

                var result = await _presensiService.ClockInAsync(request, employee.IdKaryawan);

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
        /// Clock Out (Employee)
        /// </summary>
        /// <param name="request">Clock out data</param>
        /// <returns>Clock out response</returns>
        [HttpPost("clock-out")]
        public async Task<IActionResult> ClockOut([FromBody] ClockOutRequest request)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int penggunaId))
                {
                    return BadRequest(new { success = false, message = "User ID tidak valid" });
                }

                // Find employee by user ID
                var employee = await _context.Karyawan
                    .FirstOrDefaultAsync(k => k.IdPengguna == penggunaId);

                if (employee == null)
                {
                    return BadRequest(new { success = false, message = "Data karyawan tidak ditemukan" });
                }

                var result = await _presensiService.ClockOutAsync(request, employee.IdKaryawan);

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
        /// Get all attendance records (HRD) or user's own records (Employee)
        /// </summary>
        /// <param name="startDate">Start date filter</param>
        /// <param name="endDate">End date filter</param>
        /// <returns>List of attendance records</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var userRole = User.FindFirst("peran")?.Value ??
                              User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                int? karyawanId = null;
                if (userRole == "Karyawan")
                {
                    if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int penggunaId))
                    {
                        return BadRequest(new { success = false, message = "User ID tidak valid" });
                    }

                    var employee = await _context.Karyawan
                        .FirstOrDefaultAsync(k => k.IdPengguna == penggunaId);

                    if (employee != null)
                    {
                        karyawanId = employee.IdKaryawan;
                    }
                }

                var result = await _presensiService.GetAllAsync(startDate, endDate, karyawanId);

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
        /// Get attendance record by ID
        /// </summary>
        /// <param name="id">Attendance record ID</param>
        /// <returns>Attendance record details</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID presensi tidak valid" });
                }

                var result = await _presensiService.GetByIdAsync(id);

                if (result.Success)
                {
                    return Ok(result);
                }

                return NotFound(result);
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
        /// Create manual attendance record (HRD only)
        /// </summary>
        /// <param name="request">Manual attendance data</param>
        /// <returns>Created attendance record</returns>
        [HttpPost("manual")]
        [Authorize(Roles = "HRD")] // Only HRD can create manual attendance
        public async Task<IActionResult> CreateManual([FromBody] ManualPresensiRequest request)
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

                var result = await _presensiService.CreateManualAsync(request);

                if (result.Success)
                {
                    return CreatedAtAction(
                        nameof(GetById),
                        new { id = result.Data!.IdPresensi },
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
        /// Update attendance record (HRD only)
        /// </summary>
        /// <param name="id">Attendance record ID</param>
        /// <param name="request">Updated attendance data</param>
        /// <returns>Updated attendance record</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "HRD")] // Only HRD can update attendance
        public async Task<IActionResult> Update(int id, [FromBody] ManualPresensiRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID presensi tidak valid" });
                }

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

                var result = await _presensiService.UpdateAsync(id, request);

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
        /// Delete attendance record (HRD only)
        /// </summary>
        /// <param name="id">Attendance record ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "HRD")] // Only HRD can delete attendance
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID presensi tidak valid" });
                }

                var result = await _presensiService.DeleteAsync(id);

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
        /// Get current user's attendance records
        /// </summary>
        /// <param name="startDate">Start date filter</param>
        /// <param name="endDate">End date filter</param>
        /// <returns>User's attendance records</returns>
        [HttpGet("my-attendance")]
        public async Task<IActionResult> GetMyAttendance([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int penggunaId))
                {
                    return BadRequest(new { success = false, message = "User ID tidak valid" });
                }

                var employee = await _context.Karyawan
                    .FirstOrDefaultAsync(k => k.IdPengguna == penggunaId);

                if (employee == null)
                {
                    return BadRequest(new { success = false, message = "Data karyawan tidak ditemukan" });
                }

                var result = await _presensiService.GetByKaryawanAsync(employee.IdKaryawan, startDate, endDate);

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
        /// Get today's attendance status for current user
        /// </summary>
        /// <returns>Today's attendance status</returns>
        [HttpGet("today")]
        public async Task<IActionResult> GetTodayStatus()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int penggunaId))
                {
                    return BadRequest(new { success = false, message = "User ID tidak valid" });
                }

                var employee = await _context.Karyawan
                    .FirstOrDefaultAsync(k => k.IdPengguna == penggunaId);

                if (employee == null)
                {
                    return BadRequest(new { success = false, message = "Data karyawan tidak ditemukan" });
                }

                var result = await _presensiService.GetTodayPresenceAsync(employee.IdKaryawan);

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
        /// Get attendance statistics (HRD only)
        /// </summary>
        /// <param name="startDate">Start date for statistics</param>
        /// <param name="endDate">End date for statistics</param>
        /// <returns>Attendance statistics</returns>
        [HttpGet("stats")]
        [Authorize(Roles = "HRD")] // Only HRD can view stats
        public async Task<IActionResult> GetStats([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var result = await _presensiService.GetStatsAsync(startDate, endDate);

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
        /// Get attendance summary for dashboard (HRD only)
        /// </summary>
        /// <returns>Today's attendance summary</returns>
        [HttpGet("today-summary")]
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> GetTodaySummary()
        {
            try
            {
                var today = DateTime.Today;
                var todayAttendance = await _context.Presensi
                    .Include(p => p.Karyawan)
                    .Where(p => p.Tanggal == today)
                    .ToListAsync();

                var totalEmployees = await _context.Karyawan
                    .Where(k => k.StatusKaryawan == "Aktif")
                    .CountAsync();

                var summary = new
                {
                    success = true,
                    message = "Ringkasan presensi hari ini berhasil diambil",
                    data = new
                    {
                        tanggal = today,
                        totalKaryawan = totalEmployees,
                        sudahClockIn = todayAttendance.Count,
                        belumClockIn = totalEmployees - todayAttendance.Count,
                        sudahClockOut = todayAttendance.Count(p => p.JamKeluar.HasValue),
                        belumClockOut = todayAttendance.Count(p => !p.JamKeluar.HasValue),
                        hadir = todayAttendance.Count(p => p.StatusPresensi == "Hadir"),
                        terlambat = todayAttendance.Count(p => p.StatusPresensi == "Terlambat"),
                        karyawanHadir = todayAttendance.Select(p => new
                        {
                            nama = p.Karyawan.NamaLengkap,
                            nik = p.Karyawan.NIK,
                            jamMasuk = p.JamMasuk,
                            jamKeluar = p.JamKeluar,
                            status = p.StatusPresensi
                        }).ToList()
                    }
                };

                return Ok(summary);
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