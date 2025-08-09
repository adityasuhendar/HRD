// HRD.API/Services/Interfaces/IPresensiService.cs
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response;

namespace HRD.API.Services.Interfaces
{
    public interface IPresensiService
    {
        Task<ApiResponse<ClockInResponse>> ClockInAsync(ClockInRequest request, int karyawanId);
        Task<ApiResponse<ClockOutResponse>> ClockOutAsync(ClockOutRequest request, int karyawanId);
        Task<ApiResponse<List<PresensiListResponse>>> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, int? karyawanId = null);
        Task<ApiResponse<PresensiResponse>> GetByIdAsync(int id);
        Task<ApiResponse<PresensiResponse>> CreateManualAsync(ManualPresensiRequest request);
        Task<ApiResponse<PresensiResponse>> UpdateAsync(int id, ManualPresensiRequest request);
        Task<ApiResponse<string>> DeleteAsync(int id);
        Task<ApiResponse<List<PresensiListResponse>>> GetByKaryawanAsync(int karyawanId, DateTime? startDate = null, DateTime? endDate = null);
        Task<ApiResponse<PresensiStatsResponse>> GetStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<ApiResponse<PresensiResponse?>> GetTodayPresenceAsync(int karyawanId);
    }
}