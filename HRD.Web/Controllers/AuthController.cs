// ===================================
// UPDATED: HRD.Web/Controllers/AuthController.cs
// ===================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using HRD.Web.Models.ViewModels;
using HRD.Web.Models.DTOs;
using HRD.Web.Services;

namespace HRD.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthApiService _authApiService;

        public AuthController(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginRequestDto = new LoginRequestDto
            {
                Username = model.Username,
                Password = model.Password
            };

            var result = await _authApiService.LoginAsync(loginRequestDto);

            if (result?.Success == true && result.Data != null)
            {
                Console.WriteLine($"Login successful for user: {result.Data.Username}, UserId: {result.Data.UserId}");

                // Store token in session
                HttpContext.Session.SetString("AuthToken", result.Data.Token);
                HttpContext.Session.SetString("Username", result.Data.Username);
                HttpContext.Session.SetString("Peran", result.Data.Peran);
                HttpContext.Session.SetString("NamaLengkap", result.Data.NamaLengkap);
                HttpContext.Session.SetString("UserId", result.Data.UserId.ToString()); // ADD THIS

                // Create claims for cookie authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.Data.Username),
                    new Claim(ClaimTypes.NameIdentifier, result.Data.UserId.ToString()), // ADD THIS!
                    new Claim(ClaimTypes.Role, result.Data.Peran),
                    new Claim("NamaLengkap", result.Data.NamaLengkap),
                    new Claim(ClaimTypes.Email, result.Data.Email)
                };

                Console.WriteLine("Claims created:");
                foreach (var claim in claims)
                {
                    Console.WriteLine($"  {claim.Type}: {claim.Value}");
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = result.Data.ExpiresAt,
                    IsPersistent = model.RememberMe
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                // Redirect based on role
                if (result.Data.Peran == "HRD")
                {
                    return RedirectToAction("HRDDashboard", "Home");
                }
                else
                {
                    return RedirectToAction("EmployeeDashboard", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", result?.Message ?? "Login gagal");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Call API logout
                await _authApiService.LogoutAsync();
            }
            catch
            {
                // Continue with local logout even if API call fails
            }

            // Clear session
            HttpContext.Session.Clear();

            // Sign out from cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}