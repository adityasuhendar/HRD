// HRD.API/Services/Interfaces/IPayrollService.cs
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response;

namespace HRD.API.Services.Interfaces
{
    public interface IPayrollService
    {
        Task<ApiResponse<PayrollSummaryResponse>> GetPayrollSummaryAsync(int bulan, int tahun);
        Task<ApiResponse<List<PayrollResponse>>> GetPayrollByMonthAsync(int bulan, int tahun);
        Task<ApiResponse<PayrollResponse>> GetPayrollByIdAsync(int idGaji);
        Task<ApiResponse<PayrollSummaryResponse>> GenerateMonthlyPayrollAsync(GeneratePayrollRequest request);
        Task<ApiResponse<PayrollResponse>> UpdatePayrollAsync(int idGaji, UpdatePayrollRequest request);
        Task<ApiResponse<string>> ProcessPaymentAsync(ProcessPaymentRequest request);
        Task<ApiResponse<List<PayrollResponse>>> GetEmployeePayrollHistoryAsync(int idKaryawan, int? tahun = null);
        Task<ApiResponse<string>> DeletePayrollAsync(int idGaji);
    }
}