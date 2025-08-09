// HRD.API/Services/Implementations/AuthService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRD.API.Data;
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response;
using HRD.API.Services.Interfaces;

namespace HRD.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly HRDDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(HRDDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                // Find user
                var pengguna = await _context.Pengguna
                    .Include(p => p.Karyawan)
                    .FirstOrDefaultAsync(p => p.Username == request.Username && p.Aktif);

                if (pengguna == null)
                {
                    return ApiResponse<LoginResponse>.ErrorResult("Username tidak ditemukan");
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, pengguna.PasswordHash))
                {
                    return ApiResponse<LoginResponse>.ErrorResult("Password salah");
                }

                // Generate JWT token
                var token = GenerateJwtToken(pengguna.Username, pengguna.Peran, pengguna.IdPengguna);
                var expiresAt = DateTime.UtcNow.AddHours(24);

                // UPDATED: Include UserId in response
                var response = new LoginResponse
                {
                    Token = token,
                    Username = pengguna.Username,
                    Email = pengguna.Email,
                    Peran = pengguna.Peran,
                    NamaLengkap = pengguna.Karyawan?.NamaLengkap ?? "Administrator",
                    ExpiresAt = expiresAt,
                    UserId = pengguna.IdPengguna // ADD THIS LINE!
                };

                Console.WriteLine($"AuthService Login: Created response for user {pengguna.Username} with UserId {pengguna.IdPengguna}");

                return ApiResponse<LoginResponse>.SuccessResult(response, "Login berhasil");
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> LogoutAsync(string token)
        {
            // For now, just return success
            // In production, you might want to blacklist the token
            await Task.CompletedTask;
            return ApiResponse<string>.SuccessResult("", "Logout berhasil");
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtSettings = _configuration.GetSection("JWT");
                var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GenerateJwtToken(string username, string peran, int penggunaId)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, peran),
                    new Claim(ClaimTypes.NameIdentifier, penggunaId.ToString()),
                    new Claim("username", username),
                    new Claim("peran", peran)
                }),
                Expires = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpiryInHours"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
