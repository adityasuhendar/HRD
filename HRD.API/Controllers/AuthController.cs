// HRD.API/Controllers/AuthController.cs - Updated with better token handling
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HRD.API.Models.DTOs.Request;
using HRD.API.Services.Interfaces;

namespace HRD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new { success = false, message = "Data tidak valid", errors });
            }

            var result = await _authService.LoginAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("logout")]
        [Authorize] // Require authentication
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Get token from Authorization header
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authHeader))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Authorization header tidak ditemukan",
                        hint = "Pastikan mengirim header: Authorization: Bearer <token>"
                    });
                }

                // Extract token (remove "Bearer " prefix)
                var token = authHeader.StartsWith("Bearer ")
                    ? authHeader.Substring("Bearer ".Length).Trim()
                    : authHeader.Trim();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Token tidak ditemukan dalam Authorization header",
                        hint = "Format: Authorization: Bearer <your-token>"
                    });
                }

                var result = await _authService.LogoutAsync(token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan saat logout",
                    error = ex.Message
                });
            }
        }

        [HttpGet("validate")]
        [Authorize] // Require authentication
        public async Task<IActionResult> ValidateToken()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authHeader))
                {
                    return BadRequest(new { success = false, message = "Authorization header tidak ditemukan" });
                }

                var token = authHeader.StartsWith("Bearer ")
                    ? authHeader.Substring("Bearer ".Length).Trim()
                    : authHeader.Trim();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { success = false, message = "Token tidak ditemukan" });
                }

                var isValid = await _authService.ValidateTokenAsync(token);

                if (isValid)
                {
                    // Get current user info from claims
                    var username = User.FindFirst("username")?.Value ?? User.Identity?.Name;
                    var peran = User.FindFirst("peran")?.Value ?? User.FindFirst("role")?.Value;

                    return Ok(new
                    {
                        success = true,
                        message = "Token valid",
                        user = new
                        {
                            username = username,
                            peran = peran
                        }
                    });
                }

                return Unauthorized(new { success = false, message = "Token tidak valid" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan saat validasi token",
                    error = ex.Message
                });
            }
        }

        [HttpGet("me")]
        [Authorize] // Get current user info
        public IActionResult GetCurrentUser()
        {
            try
            {
                var username = User.FindFirst("username")?.Value ?? User.Identity?.Name;
                var peran = User.FindFirst("peran")?.Value ?? User.FindFirst("role")?.Value;
                var userId = User.FindFirst("nameid")?.Value;

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        username = username,
                        peran = peran,
                        userId = userId
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Terjadi kesalahan",
                    error = ex.Message
                });
            }
        }
    }
}