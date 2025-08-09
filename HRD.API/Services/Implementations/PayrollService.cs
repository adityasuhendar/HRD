// HRD.API/Services/Implementations/PayrollService.cs
using Microsoft.EntityFrameworkCore;
using HRD.API.Data;
using HRD.API.Models.Entities;
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response;
using HRD.API.Services.Interfaces;

namespace HRD.API.Services.Implementations
{
    public class PayrollService : IPayrollService
    {
        private readonly HRDDbContext _context;

        public PayrollService(HRDDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<PayrollSummaryResponse>> GetPayrollSummaryAsync(int bulan, int tahun)
        {
            try
            {
                var payrolls = await _context.Gaji
                    .Include(g => g.Karyawan)
                    .Where(g => g.Bulan == bulan && g.Tahun == tahun)
                    .ToListAsync();

                var summary = new PayrollSummaryResponse
                {
                    Bulan = bulan,
                    Tahun = tahun,
                    JumlahKaryawan = payrolls.Count,
                    TotalGajiPokok = payrolls.Sum(p => p.GajiPokok),
                    TotalTunjangan = payrolls.Sum(p => p.Tunjangan),
                    TotalPotongan = payrolls.Sum(p => p.Potongan),
                    TotalPayroll = payrolls.Sum(p => p.TotalGaji),
                    SudahBayar = payrolls.Count(p => p.StatusBayar == "Sudah Bayar"),
                    BelumBayar = payrolls.Count(p => p.StatusBayar == "Belum Bayar"),
                    DetailPayroll = payrolls.Select(p => new PayrollResponse
                    {
                        IdGaji = p.IdGaji,
                        IdKaryawan = p.IdKaryawan,
                        NIK = p.Karyawan.NIK,
                        NamaLengkap = p.Karyawan.NamaLengkap,
                        Posisi = p.Karyawan.Posisi,
                        Divisi = p.Karyawan.Divisi,
                        Bulan = p.Bulan,
                        Tahun = p.Tahun,
                        GajiPokok = p.GajiPokok,
                        Tunjangan = p.Tunjangan,
                        Potongan = p.Potongan,
                        TotalJamKerja = p.TotalJamKerja,
                        TotalGaji = p.TotalGaji,
                        StatusBayar = p.StatusBayar,
                        TglDibuat = p.TglDibuat,
                        Keterangan = p.Keterangan
                    }).OrderBy(p => p.NIK).ToList()
                };

                return ApiResponse<PayrollSummaryResponse>.SuccessResult(summary, "Data payroll berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<PayrollSummaryResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PayrollSummaryResponse>> GenerateMonthlyPayrollAsync(GeneratePayrollRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check if payroll already exists for this month
                var existingPayroll = await _context.Gaji
                    .AnyAsync(g => g.Bulan == request.Bulan && g.Tahun == request.Tahun);

                if (existingPayroll)
                {
                    return ApiResponse<PayrollSummaryResponse>.ErrorResult($"Payroll untuk {GetMonthName(request.Bulan)} {request.Tahun} sudah ada");
                }

                // Get all active employees
                var activeEmployees = await _context.Karyawan
                    .Where(k => k.StatusKaryawan == "Aktif")
                    .ToListAsync();

                if (!activeEmployees.Any())
                {
                    return ApiResponse<PayrollSummaryResponse>.ErrorResult("Tidak ada karyawan aktif");
                }

                // Calculate standard working hours for the month
                var standardWorkingHours = CalculateWorkingHours(request.Bulan, request.Tahun);

                // Generate payroll records
                var payrollRecords = new List<Gaji>();
                foreach (var employee in activeEmployees)
                {
                    var payroll = new Gaji
                    {
                        IdKaryawan = employee.IdKaryawan,
                        Bulan = request.Bulan,
                        Tahun = request.Tahun,
                        GajiPokok = employee.GajiPokok,
                        Tunjangan = CalculateDefaultAllowance(employee.GajiPokok),
                        Potongan = CalculateDefaultDeduction(employee.GajiPokok),
                        TotalJamKerja = standardWorkingHours,
                        StatusBayar = "Belum Bayar",
                        TglDibuat = DateTime.Now,
                        Keterangan = "Generated automatically"
                    };

                    // Calculate total salary
                    payroll.TotalGaji = payroll.GajiPokok + payroll.Tunjangan - payroll.Potongan;

                    payrollRecords.Add(payroll);
                }

                _context.Gaji.AddRange(payrollRecords);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Return summary
                return await GetPayrollSummaryAsync(request.Bulan, request.Tahun);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<PayrollSummaryResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PayrollResponse>> UpdatePayrollAsync(int idGaji, UpdatePayrollRequest request)
        {
            try
            {
                var payroll = await _context.Gaji
                    .Include(g => g.Karyawan)
                    .FirstOrDefaultAsync(g => g.IdGaji == idGaji);

                if (payroll == null)
                {
                    return ApiResponse<PayrollResponse>.ErrorResult("Data payroll tidak ditemukan");
                }

                if (payroll.StatusBayar == "Sudah Bayar")
                {
                    return ApiResponse<PayrollResponse>.ErrorResult("Tidak dapat mengubah payroll yang sudah dibayar");
                }

                // Update fields
                payroll.Tunjangan = request.Tunjangan;
                payroll.Potongan = request.Potongan;
                payroll.TotalJamKerja = request.TotalJamKerja;
                payroll.Keterangan = request.Keterangan;

                // Recalculate total salary
                payroll.TotalGaji = payroll.GajiPokok + payroll.Tunjangan - payroll.Potongan;

                await _context.SaveChangesAsync();

                var response = new PayrollResponse
                {
                    IdGaji = payroll.IdGaji,
                    IdKaryawan = payroll.IdKaryawan,
                    NIK = payroll.Karyawan.NIK,
                    NamaLengkap = payroll.Karyawan.NamaLengkap,
                    Posisi = payroll.Karyawan.Posisi,
                    Divisi = payroll.Karyawan.Divisi,
                    Bulan = payroll.Bulan,
                    Tahun = payroll.Tahun,
                    GajiPokok = payroll.GajiPokok,
                    Tunjangan = payroll.Tunjangan,
                    Potongan = payroll.Potongan,
                    TotalJamKerja = payroll.TotalJamKerja,
                    TotalGaji = payroll.TotalGaji,
                    StatusBayar = payroll.StatusBayar,
                    TglDibuat = payroll.TglDibuat,
                    Keterangan = payroll.Keterangan
                };

                return ApiResponse<PayrollResponse>.SuccessResult(response, "Data payroll berhasil diperbarui");
            }
            catch (Exception ex)
            {
                return ApiResponse<PayrollResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> ProcessPaymentAsync(ProcessPaymentRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var payrolls = await _context.Gaji
                    .Where(g => request.IdGajiList.Contains(g.IdGaji))
                    .ToListAsync();

                if (!payrolls.Any())
                {
                    return ApiResponse<string>.ErrorResult("Data payroll tidak ditemukan");
                }

                // Check if any payroll is already paid
                var alreadyPaid = payrolls.Where(p => p.StatusBayar == "Sudah Bayar").ToList();
                if (alreadyPaid.Any())
                {
                    return ApiResponse<string>.ErrorResult($"{alreadyPaid.Count} payroll sudah dibayar sebelumnya");
                }

                // Update status to paid
                foreach (var payroll in payrolls)
                {
                    payroll.StatusBayar = "Sudah Bayar";
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse<string>.SuccessResult("", $"Berhasil memproses pembayaran untuk {payrolls.Count} karyawan");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<string>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        // Helper methods
        private decimal CalculateDefaultAllowance(decimal gajiPokok)
        {
            // 10% dari gaji pokok sebagai tunjangan default
            return Math.Round(gajiPokok * 0.10m, 0);
        }

        private decimal CalculateDefaultDeduction(decimal gajiPokok)
        {
            // 5% dari gaji pokok sebagai potongan default (pajak)
            return Math.Round(gajiPokok * 0.05m, 0);
        }

        private int CalculateWorkingHours(int bulan, int tahun)
        {
            // Asumsi 8 jam per hari, 22 hari kerja per bulan
            return 22 * 8; // 176 hours
        }

        private string GetMonthName(int bulan)
        {
            string[] months = { "", "Januari", "Februari", "Maret", "April", "Mei", "Juni",
                               "Juli", "Agustus", "September", "Oktober", "November", "Desember" };
            return months[bulan];
        }

        // Implement other interface methods...
        public async Task<ApiResponse<List<PayrollResponse>>> GetPayrollByMonthAsync(int bulan, int tahun)
        {
            // Implementation here...
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<PayrollResponse>> GetPayrollByIdAsync(int idGaji)
        {
            // Implementation here...
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<List<PayrollResponse>>> GetEmployeePayrollHistoryAsync(int idKaryawan, int? tahun = null)
        {
            // Implementation here...
            throw new NotImplementedException();
        }

        public async Task<ApiResponse<string>> DeletePayrollAsync(int idGaji)
        {
            // Implementation here...
            throw new NotImplementedException();
        }
    }
}