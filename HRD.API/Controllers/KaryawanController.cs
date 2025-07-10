// HRD.API/Controllers/KaryawanController.cs
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response; // ADD THIS
using HRD.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // ADD THIS IMPORT

namespace HRD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Semua endpoint butuh authentication
    public class KaryawanController : ControllerBase
    {
        private readonly IKaryawanService _karyawanService;

        public KaryawanController(IKaryawanService karyawanService)
        {
            _karyawanService = karyawanService;
        }

        /// <summary>
        /// Get all employees with optional search and filter
        /// </summary>
        /// <param name="search">Search by name, NIK, email, or position</param>
        /// <param name="divisi">Filter by division</param>
        /// <returns>List of employees</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search = null, [FromQuery] string? divisi = null)
        {
            try
            {
                var result = await _karyawanService.GetAllAsync(search, divisi);

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
        /// Get employee by ID
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Employee details</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID karyawan tidak valid" });
                }

                var result = await _karyawanService.GetByIdAsync(id);

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
        /// Create new employee (HRD only)
        /// </summary>
        /// <param name="request">Employee data</param>
        /// <returns>Created employee</returns>
        [HttpPost]
        [Authorize(Roles = "HRD")] // Only HRD can create employees
        public async Task<IActionResult> Create([FromBody] CreateKaryawanRequest request)
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

                var result = await _karyawanService.CreateAsync(request);

                if (result.Success)
                {
                    return CreatedAtAction(
                        nameof(GetById),
                        new { id = result.Data!.IdKaryawan },
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
        /// Update employee data (HRD only)
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="request">Updated employee data</param>
        /// <returns>Updated employee</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "HRD")] // Only HRD can update employees
        public async Task<IActionResult> Update(int id, [FromBody] UpdateKaryawanRequest request)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID karyawan tidak valid" });
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

                var result = await _karyawanService.UpdateAsync(id, request);

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
        /// Delete employee (HRD only) - Soft delete recommended
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "HRD")] // Only HRD can delete employees
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID karyawan tidak valid" });
                }

                var result = await _karyawanService.DeleteAsync(id);

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
        /// Activate employee account (HRD only)
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success message</returns>
        [HttpPost("{id:int}/activate")]
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID karyawan tidak valid" });
                }

                var result = await _karyawanService.ActivateAsync(id);

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
        /// Deactivate employee account (HRD only)
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success message</returns>
        [HttpPost("{id:int}/deactivate")]
        [Authorize(Roles = "HRD")]
        public async Task<IActionResult> Deactivate(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { success = false, message = "ID karyawan tidak valid" });
                }

                var result = await _karyawanService.DeactivateAsync(id);

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
        /// Get employee profile (for logged in employee)
        /// </summary>
        /// <returns>Current user's employee profile</returns>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst("nameid")?.Value;

                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int penggunaId))
                {
                    return BadRequest(new { success = false, message = "User ID tidak valid" });
                }

                // Find employee by user ID
                var karyawan = await _karyawanService.GetAllAsync();
                var employeeProfile = karyawan.Data?.FirstOrDefault(k => k.IdKaryawan == penggunaId);

                if (employeeProfile == null)
                {
                    return NotFound(new { success = false, message = "Profil karyawan tidak ditemukan" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Profil berhasil diambil",
                    data = employeeProfile
                });
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
        /// Get divisions list for dropdown
        /// </summary>
        /// <returns>List of unique divisions</returns>
        [HttpGet("divisions")]
        public async Task<IActionResult> GetDivisions()
        {
            try
            {
                var employees = await _karyawanService.GetAllAsync();

                if (employees.Success && employees.Data != null)
                {
                    var divisions = employees.Data
                        .Select(k => k.Divisi)
                        .Distinct()
                        .OrderBy(d => d)
                        .ToList();

                    return Ok(new
                    {
                        success = true,
                        message = "Daftar divisi berhasil diambil",
                        data = divisions
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Tidak ada data divisi",
                    data = new List<string>()
                });
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
        [HttpGet("by-user/{userId:int}")]
        [Authorize]
        public async Task<IActionResult> GetEmployeeByUserId(int userId)
        {
            try
            {
                Console.WriteLine($"GetEmployeeByUserId: Requested userId: {userId}");

                // Security check - employees can only access their own data
                if (User.IsInRole("Karyawan"))
                {
                    // FIX: Use ClaimTypes.NameIdentifier instead of "nameid"
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    Console.WriteLine($"NameIdentifier claim: '{userIdClaim}'");

                    if (!int.TryParse(userIdClaim, out int currentUserId))
                    {
                        Console.WriteLine($"Failed to parse NameIdentifier: '{userIdClaim}'");
                        return BadRequest(new { success = false, message = "User ID tidak valid" });
                    }

                    Console.WriteLine($"Comparing: currentUserId={currentUserId}, requestedUserId={userId}");

                    if (currentUserId != userId)
                    {
                        Console.WriteLine($"Access denied: {currentUserId} != {userId}");
                        return Forbid();
                    }

                    Console.WriteLine("Security check passed for employee");
                }
                else
                {
                    Console.WriteLine("User is not Karyawan, allowing access (probably HRD)");
                }

                // Use service instead of direct context
                var result = await _karyawanService.GetByUserIdAsync(userId);

                Console.WriteLine($"Service result: Success={result.Success}");

                if (result.Success)
                {
                    return Ok(result);
                }

                return NotFound(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
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