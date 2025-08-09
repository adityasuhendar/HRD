// HRD.API/Services/Interfaces/ICutiService.cs
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response;

namespace HRD.API.Services.Interfaces
{
    public interface ICutiService
    {
        Task<ApiResponse<List<CutiListResponse>>> GetAllAsync(string? status = null, int? karyawanId = null);
        Task<ApiResponse<CutiResponse>> GetByIdAsync(int id);
        Task<ApiResponse<CutiResponse>> CreateAsync(CreateCutiRequest request, int karyawanId);
        Task<ApiResponse<CutiResponse>> ApproveAsync(int id, ApproveCutiRequest request, int approvedBy);
        Task<ApiResponse<string>> DeleteAsync(int id, int karyawanId);
        Task<ApiResponse<List<CutiListResponse>>> GetByKaryawanAsync(int karyawanId);
        Task<ApiResponse<CutiStatsResponse>> GetStatsAsync();
        Task<ApiResponse<List<string>>> GetJenisCutiAsync();
    }
}