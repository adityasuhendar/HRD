// HRD.Web/Services/AuthApiService.cs
using HRD.Web.Models.DTOs; 

namespace HRD.Web.Services
{
    public class AuthApiService
    {
        private readonly ApiService _apiService;

        public AuthApiService(ApiService apiService)
        {
            _apiService = apiService;
        }

        // ✅ FIXED: Change LoginRequest to LoginRequestDto and LoginResponse to LoginResponseDto
        public async Task<ApiResponse<LoginResponseDto>?> LoginAsync(LoginRequestDto request)
        {
            return await _apiService.PostAsync<LoginResponseDto>("api/auth/login", request);
        }

        public async Task<ApiResponse<string>?> LogoutAsync()
        {
            return await _apiService.PostAsync<string>("api/auth/logout", new { });
        }

        public async Task<ApiResponse<object>?> ValidateTokenAsync()
        {
            return await _apiService.GetAsync<object>("api/auth/validate");
        }
    }
}