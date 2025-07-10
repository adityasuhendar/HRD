// HRD.API/Services/Interfaces/IAuthService.cs
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response;

namespace HRD.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<string>> LogoutAsync(string token);
        Task<bool> ValidateTokenAsync(string token);
        string GenerateJwtToken(string username, string peran, int penggunaId);
    }
}