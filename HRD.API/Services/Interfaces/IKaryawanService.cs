// HRD.API/Services/Interfaces/IKaryawanService.cs
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response;

namespace HRD.API.Services.Interfaces
{
    public interface IKaryawanService
    {
        Task<ApiResponse<List<KaryawanListResponse>>> GetAllAsync(string? search = null, string? divisi = null);
        Task<ApiResponse<KaryawanResponse>> GetByIdAsync(int id);
        Task<ApiResponse<KaryawanResponse>> CreateAsync(CreateKaryawanRequest request);
        Task<ApiResponse<KaryawanResponse>> UpdateAsync(int id, UpdateKaryawanRequest request);
        Task<ApiResponse<KaryawanResponse>> GetByUserIdAsync(int userId);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<string>> ActivateAsync(int id);
        Task<ApiResponse<string>> DeactivateAsync(int id);
    }
}