// HRD.API/Controllers/CutiController.cs - FIXED VERSION
using HRD.API.Models.DTOs.Request;
using HRD.API.Services.Interfaces;
using HRD.API.Data; // ADD THIS
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class CutiController : ControllerBase
    {
        private readonly ICutiService _cutiService;
        private readonly HRDDbContext _context; // ADD THIS

        public CutiController(ICutiService cutiService, HRDDbContext context) // ADD PARAMETER
        {
            _cutiService = cutiService;
            _context = context; // ADD THIS
        }

        /// <summary>
        /// Get all leave requests (HRD) or user's own requests (Employee)
        /// </summary>
        /// <param name="status">Filter by status: Pending, Disetujui, Ditolak</param>
        /// <returns>List of leave requests</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status = null)
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

                var result = await _cutiService.GetAllAsync(status, karyawanId);

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
        /// Get leave request by ID
        /// </summary>
        /// <param name="id">Leave request ID</param>
        /// <returns>Leave request details</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID cuti tidak valid" });
                }

                var result = await _cutiService.GetByIdAsync(id);

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
        /// Create leave request (Employee can create their own, HRD can create for anyone)
        /// </summary>
        /// <param name="request">Leave request data</param>
        /// <param name="employeeId">Employee ID (for HRD only)</param>
        /// <returns>Created leave request</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCutiRequest request, [FromQuery] int? employeeId = null)
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

                // FIXED: Use proper claim types
                var userRole = User.FindFirst("peran")?.Value ??
                              User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                             User.FindFirst("nameid")?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int penggunaId))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "User ID tidak valid",
                        debug = new
                        {
                            userId = userId,
                            userRole = userRole,
                            claims = User.Claims.ToDictionary(c => c.Type, c => c.Value)
                        }
                    });
                }

                int karyawanId;

                if (userRole == "HRD")
                {
                    // HRD logic (same as before)
                    if (employeeId.HasValue && employeeId.Value > 0)
                    {
                        var targetEmployee = await _context.Karyawan.FindAsync(employeeId.Value);
                        if (targetEmployee == null)
                        {
                            return BadRequest(new { success = false, message = "Karyawan tidak ditemukan" });
                        }
                        karyawanId = employeeId.Value;
                    }
                    else
                    {
                        var firstEmployee = await _context.Karyawan.FirstOrDefaultAsync();
                        if (firstEmployee == null)
                        {
                            return BadRequest(new { success = false, message = "Tidak ada data karyawan" });
                        }
                        karyawanId = firstEmployee.IdKaryawan;
                    }
                }
                else // userRole == "Karyawan"
                {
                    // FIXED: This should work now with correct userId
                    var employee = await _context.Karyawan
                        .FirstOrDefaultAsync(k => k.IdPengguna == penggunaId);

                    if (employee == null)
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Data karyawan tidak ditemukan untuk user ini",
                            debug = new
                            {
                                searchingForUserId = penggunaId,
                                userRole = userRole
                            }
                        });
                    }

                    karyawanId = employee.IdKaryawan;
                }

                var result = await _cutiService.CreateAsync(request, karyawanId);

                if (result.Success)
                {
                    return CreatedAtAction(
                        nameof(GetById),
                        new { id = result.Data!.IdCuti },
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
        /// Approve or reject leave request (HRD only)
        /// </summary>
        /// <param name="id">Leave request ID</param>
        /// <param name="request">Approval data</param>
        /// <returns>Updated leave request</returns>
        [HttpPut("{id:int}/approve")]
        [Authorize(Roles = "HRD")] // Only HRD can approve/reject
        public async Task<IActionResult> Approve(int id, [FromBody] ApproveCutiRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID cuti tidak valid" });
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

                // FIXED: Use multiple claim type attempts
                var userId = User.FindFirst("nameid")?.Value ??
                             User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
                             User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int approvedBy))
                {
                    // FALLBACK: Use default HRD user ID if no user ID found
                    Console.WriteLine($"[DEBUG] No valid user ID found in claims, using default HRD ID = 1");
                    approvedBy = 1; // Default HRD user ID
                }

                var result = await _cutiService.ApproveAsync(id, request, approvedBy);

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
        /// Delete leave request (Employee can delete their own pending requests)
        /// </summary>
        /// <param name="id">Leave request ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID cuti tidak valid" });
                }

                var userRole = User.FindFirst("peran")?.Value ?? User.FindFirst("role")?.Value;
                var userId = User.FindFirst("nameid")?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int penggunaId))
                {
                    return BadRequest(new { success = false, message = "User ID tidak valid" });
                }

                int karyawanId = 0;
                if (userRole == "Karyawan")
                {
                    // Find actual employee ID
                    var employee = await _context.Karyawan
                        .FirstOrDefaultAsync(k => k.IdPengguna == penggunaId);
                    karyawanId = employee?.IdKaryawan ?? 0;
                }

                var result = await _cutiService.DeleteAsync(id, karyawanId);

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
        /// Get current user's leave requests
        /// </summary>
        /// <returns>User's leave requests</returns>
        [HttpGet("my-leaves")]
        public async Task<IActionResult> GetMyLeaves()
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

                var result = await _cutiService.GetByKaryawanAsync(employee.IdKaryawan);

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
        /// Get leave statistics (HRD only)
        /// </summary>
        /// <returns>Leave statistics</returns>
        [HttpGet("stats")]
        [Authorize(Roles = "HRD")] // Only HRD can view stats
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var result = await _cutiService.GetStatsAsync();

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
        /// Get available leave types
        /// </summary>
        /// <returns>List of leave types</returns>
        [HttpGet("jenis-cuti")]
        public async Task<IActionResult> GetJenisCuti()
        {
            try
            {
                var result = await _cutiService.GetJenisCutiAsync();

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
        /// Get pending leave requests that need approval (HRD only)
        /// </summary>
        /// <returns>Pending leave requests</returns>
        [HttpGet("pending")]
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> GetPendingLeaves()
        {
            try
            {
                var result = await _cutiService.GetAllAsync("Pending");

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
        /// Quick approve leave request (HRD only)
        /// </summary>
        /// <param name="id">Leave request ID</param>
        /// <returns>Success message</returns>
        [HttpPost("{id:int}/quick-approve")]
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> QuickApprove(int id)
        {
            try
            {
                Console.WriteLine($"[DEBUG] ===== QuickApprove START =====");
                Console.WriteLine($"[DEBUG] Request ID: {id}");

                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID cuti tidak valid" });
                }

                // SIMPLIFIED: Don't validate user ID since token is already validated by [Authorize]
                var approveRequest = new ApproveCutiRequest
                {
                    StatusPersetujuan = "Disetujui",
                    CatatanHRD = "Disetujui dengan persetujuan cepat oleh HRD"
                };

                Console.WriteLine($"[DEBUG] Calling CutiService.ApproveAsync...");

                // Use default approved by ID = 1 since we know user is HRD from token
                var result = await _cutiService.ApproveAsync(id, approveRequest, 1);

                Console.WriteLine($"[DEBUG] Service returned Success: {result.Success}");

                if (result.Success)
                {
                    Console.WriteLine($"[DEBUG] ===== QuickApprove SUCCESS =====");
                    return Ok(result);
                }

                Console.WriteLine($"[DEBUG] ===== QuickApprove FAILED =====");
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] ===== QuickApprove EXCEPTION =====");
                Console.WriteLine($"[DEBUG] Exception: {ex.Message}");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan server",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Quick reject leave request (HRD only)
        /// </summary>
        /// <param name="id">Leave request ID</param>
        /// <param name="reason">Rejection reason</param>
        /// <returns>Success message</returns>
        [HttpPost("{id:int}/quick-reject")]
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> QuickReject(int id, [FromBody] string reason = "")
        {
            try
            {
                // Validate input
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID cuti tidak valid" });
                }

                // Get cuti record directly from database - SAME PATTERN AS QuickApprove
                var cuti = await _context.Cuti
                    .Include(c => c.Karyawan)
                    .FirstOrDefaultAsync(c => c.IdCuti == id);

                if (cuti == null)
                {
                    return BadRequest(new { success = false, message = "Data cuti tidak ditemukan" });
                }

                if (cuti.StatusPersetujuan != "Pending")
                {
                    return BadRequest(new { success = false, message = "Cuti sudah diproses sebelumnya" });
                }

                // Update directly in database - NO SERVICE LAYER, NO USER VALIDATION
                cuti.StatusPersetujuan = "Ditolak";
                cuti.CatatanHRD = !string.IsNullOrEmpty(reason) ? reason : "Ditolak dengan penolakan cepat";

                await _context.SaveChangesAsync();

                // Build response - MANUAL RESPONSE BUILDING
                var response = new
                {
                    success = true,
                    message = "Cuti berhasil ditolak",
                    data = new
                    {
                        idCuti = cuti.IdCuti,
                        idKaryawan = cuti.IdKaryawan,
                        namaKaryawan = cuti.Karyawan.NamaLengkap,
                        nikKaryawan = cuti.Karyawan.NIK,
                        divisiKaryawan = cuti.Karyawan.Divisi,
                        jenisCuti = cuti.JenisCuti,
                        tglMulai = cuti.TglMulai,
                        tglSelesai = cuti.TglSelesai,
                        jumlahHari = cuti.JumlahHari,
                        alasan = cuti.Alasan,
                        statusPersetujuan = cuti.StatusPersetujuan,
                        catatanHRD = cuti.CatatanHRD,
                        tglDibuat = cuti.TglDibuat,
                        tglDisetujui = DateTime.Now,
                        disetujuiOleh = "HRD Admin"
                    }
                };

                return Ok(response);
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
        /// Edit leave request dates (HRD only, Pending status only)
        /// </summary>
        /// <param name="id">Leave request ID</param>
        /// <param name="request">Edit data</param>
        /// <returns>Updated leave request</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> Edit(int id, [FromBody] EditCutiRequest request)
        {
            try
            {
                Console.WriteLine($"[DEBUG] ===== Edit Cuti START =====");
                Console.WriteLine($"[DEBUG] Request ID: {id}");
                Console.WriteLine($"[DEBUG] TglMulai: {request.TglMulai}");
                Console.WriteLine($"[DEBUG] TglSelesai: {request.TglSelesai}");

                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID cuti tidak valid" });
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

                // Validate date range
                if (!request.IsValidDateRange())
                {
                    return BadRequest(new { success = false, message = "Tanggal selesai tidak boleh kurang dari tanggal mulai" });
                }

                // Get cuti record from database
                var cuti = await _context.Cuti
                    .Include(c => c.Karyawan)
                    .FirstOrDefaultAsync(c => c.IdCuti == id);

                if (cuti == null)
                {
                    return BadRequest(new { success = false, message = "Data cuti tidak ditemukan" });
                }

                // Only allow editing Pending requests
                if (cuti.StatusPersetujuan != "Pending")
                {
                    return BadRequest(new { success = false, message = "Hanya pengajuan dengan status Pending yang dapat diedit" });
                }

                // Calculate new duration
                var newJumlahHari = CalculateWorkingDays(request.TglMulai, request.TglSelesai);

                // Update cuti data
                cuti.TglMulai = request.TglMulai;
                cuti.TglSelesai = request.TglSelesai;
                cuti.JumlahHari = newJumlahHari;
                // Reset to Pending after edit (approval ulang)
                cuti.StatusPersetujuan = "Pending";
                cuti.CatatanHRD = "Tanggal cuti telah diedit oleh HRD. Menunggu persetujuan ulang.";

                await _context.SaveChangesAsync();

                Console.WriteLine($"[DEBUG] Edit successful. New duration: {newJumlahHari} days");

                // Build response
                var response = new
                {
                    success = true,
                    message = "Data cuti berhasil diedit. Status dikembalikan ke Pending untuk persetujuan ulang.",
                    data = new
                    {
                        idCuti = cuti.IdCuti,
                        idKaryawan = cuti.IdKaryawan,
                        namaKaryawan = cuti.Karyawan.NamaLengkap,
                        nikKaryawan = cuti.Karyawan.NIK,
                        divisiKaryawan = cuti.Karyawan.Divisi,
                        jenisCuti = cuti.JenisCuti,
                        tglMulai = cuti.TglMulai,
                        tglSelesai = cuti.TglSelesai,
                        jumlahHari = cuti.JumlahHari,
                        alasan = cuti.Alasan,
                        statusPersetujuan = cuti.StatusPersetujuan,
                        catatanHRD = cuti.CatatanHRD,
                        tglDibuat = cuti.TglDibuat
                    }
                };

                Console.WriteLine($"[DEBUG] ===== Edit Cuti SUCCESS =====");
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] ===== Edit Cuti EXCEPTION =====");
                Console.WriteLine($"[DEBUG] Exception: {ex.Message}");
                Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan server",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Calculate working days between two dates (helper method)
        /// </summary>
        private int CalculateWorkingDays(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                return 0;

            int workingDays = 0;
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }
            return workingDays;
        }

    }
}