// HRD.API/Services/Implementations/CutiService.cs
using Microsoft.EntityFrameworkCore;
using HRD.API.Data;
using HRD.API.Models.Entities;
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response;
using HRD.API.Services.Interfaces;

namespace HRD.API.Services.Implementations
{
    public class CutiService : ICutiService
    {
        private readonly HRDDbContext _context;

        public CutiService(HRDDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<CutiListResponse>>> GetAllAsync(string? status = null, int? karyawanId = null)
        {
            try
            {
                var query = _context.Cuti
                    .Include(c => c.Karyawan)
                    .AsQueryable();

                // Filter by status
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(c => c.StatusPersetujuan == status);
                }

                // Filter by employee (for employee's own requests)
                if (karyawanId.HasValue)
                {
                    query = query.Where(c => c.IdKaryawan == karyawanId.Value);
                }

                var cutiList = await query
                    .OrderByDescending(c => c.TglDibuat)
                    .Select(c => new CutiListResponse
                    {
                        IdCuti = c.IdCuti,
                        NamaKaryawan = c.Karyawan.NamaLengkap,
                        NIKKaryawan = c.Karyawan.NIK,
                        JenisCuti = c.JenisCuti,
                        TglMulai = c.TglMulai,
                        TglSelesai = c.TglSelesai,
                        JumlahHari = c.JumlahHari,
                        StatusPersetujuan = c.StatusPersetujuan,
                        TglDibuat = c.TglDibuat
                    })
                    .ToListAsync();

                return ApiResponse<List<CutiListResponse>>.SuccessResult(cutiList, "Data cuti berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CutiListResponse>>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CutiResponse>> GetByIdAsync(int id)
        {
            try
            {
                var cuti = await _context.Cuti
                    .Include(c => c.Karyawan)
                    .FirstOrDefaultAsync(c => c.IdCuti == id);

                if (cuti == null)
                {
                    return ApiResponse<CutiResponse>.ErrorResult("Data cuti tidak ditemukan");
                }

                // FIXED: Build response matching CutiResponse DTO structure (without TglDisetujui/DisetujuiOleh)
                var response = new CutiResponse
                {
                    IdCuti = cuti.IdCuti,
                    IdKaryawan = cuti.IdKaryawan,
                    NamaKaryawan = cuti.Karyawan.NamaLengkap,
                    NIKKaryawan = cuti.Karyawan.NIK,
                    DivisiKaryawan = cuti.Karyawan.Divisi,
                    JenisCuti = cuti.JenisCuti,
                    TglMulai = cuti.TglMulai,
                    TglSelesai = cuti.TglSelesai,
                    JumlahHari = cuti.JumlahHari,
                    Alasan = cuti.Alasan,
                    StatusPersetujuan = cuti.StatusPersetujuan,
                    CatatanHRD = cuti.CatatanHRD,
                    TglDibuat = cuti.TglDibuat

                    // REMOVED: These don't exist in CutiResponse DTO
                    // TglDisetujui = tglDisetujui,
                    // DisetujuiOleh = disetujuiOleh
                };

                return ApiResponse<CutiResponse>.SuccessResult(response, "Data cuti berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<CutiResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }
        public async Task<ApiResponse<CutiResponse>> CreateAsync(CreateCutiRequest request, int karyawanId)
        {
            try
            {
                // Validate date range
                if (!request.IsValidDateRange())
                {
                    return ApiResponse<CutiResponse>.ErrorResult("Tanggal selesai tidak boleh lebih awal dari tanggal mulai");
                }

                // Check if employee exists
                var karyawan = await _context.Karyawan.FindAsync(karyawanId);
                if (karyawan == null)
                {
                    return ApiResponse<CutiResponse>.ErrorResult("Karyawan tidak ditemukan");
                }

                // Check for overlapping leave requests
                var hasOverlap = await _context.Cuti
                    .Where(c => c.IdKaryawan == karyawanId &&
                               c.StatusPersetujuan == "Disetujui" &&
                               ((request.TglMulai >= c.TglMulai && request.TglMulai <= c.TglSelesai) ||
                                (request.TglSelesai >= c.TglMulai && request.TglSelesai <= c.TglSelesai) ||
                                (request.TglMulai <= c.TglMulai && request.TglSelesai >= c.TglSelesai)))
                    .AnyAsync();

                if (hasOverlap)
                {
                    return ApiResponse<CutiResponse>.ErrorResult("Sudah ada cuti yang disetujui pada rentang tanggal tersebut");
                }

                // Calculate working days (simple calculation - excludes weekends)
                int jumlahHari = CalculateWorkingDays(request.TglMulai, request.TglSelesai);

                var cuti = new Cuti
                {
                    IdKaryawan = karyawanId,
                    JenisCuti = request.JenisCuti,
                    TglMulai = request.TglMulai.Date,
                    TglSelesai = request.TglSelesai.Date,
                    JumlahHari = jumlahHari,
                    Alasan = request.Alasan,
                    StatusPersetujuan = "Pending",
                    TglDibuat = DateTime.Now
                };

                _context.Cuti.Add(cuti);
                await _context.SaveChangesAsync();

                // Return created leave request
                var response = new CutiResponse
                {
                    IdCuti = cuti.IdCuti,
                    IdKaryawan = cuti.IdKaryawan,
                    NamaKaryawan = karyawan.NamaLengkap,
                    NIKKaryawan = karyawan.NIK,
                    DivisiKaryawan = karyawan.Divisi,
                    JenisCuti = cuti.JenisCuti,
                    TglMulai = cuti.TglMulai,
                    TglSelesai = cuti.TglSelesai,
                    JumlahHari = cuti.JumlahHari,
                    Alasan = cuti.Alasan,
                    StatusPersetujuan = cuti.StatusPersetujuan,
                    CatatanHRD = cuti.CatatanHRD,
                    TglDibuat = cuti.TglDibuat
                };

                return ApiResponse<CutiResponse>.SuccessResult(response, "Pengajuan cuti berhasil dibuat");
            }
            catch (Exception ex)
            {
                return ApiResponse<CutiResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CutiResponse>> ApproveAsync(int id, ApproveCutiRequest request, int approvedBy)
        {
            try
            {
                Console.WriteLine($"[DEBUG] ApproveAsync called - ID: {id}, Status: {request.StatusPersetujuan}, ApprovedBy: {approvedBy}");

                var cuti = await _context.Cuti
                    .Include(c => c.Karyawan)
                    .FirstOrDefaultAsync(c => c.IdCuti == id);

                if (cuti == null)
                {
                    Console.WriteLine($"[DEBUG] Cuti ID {id} not found");
                    return ApiResponse<CutiResponse>.ErrorResult("Data cuti tidak ditemukan");
                }

                Console.WriteLine($"[DEBUG] Found cuti - Current status: {cuti.StatusPersetujuan}");

                if (cuti.StatusPersetujuan != "Pending")
                {
                    Console.WriteLine($"[DEBUG] Cuti already processed with status: {cuti.StatusPersetujuan}");
                    return ApiResponse<CutiResponse>.ErrorResult("Cuti sudah diproses sebelumnya");
                }

                if (request.StatusPersetujuan != "Disetujui" && request.StatusPersetujuan != "Ditolak")
                {
                    Console.WriteLine($"[DEBUG] Invalid status: {request.StatusPersetujuan}");
                    return ApiResponse<CutiResponse>.ErrorResult("Status persetujuan hanya boleh 'Disetujui' atau 'Ditolak'");
                }

                // Update leave request - ONLY fields that exist in database
                Console.WriteLine($"[DEBUG] Updating status from '{cuti.StatusPersetujuan}' to '{request.StatusPersetujuan}'");
                cuti.StatusPersetujuan = request.StatusPersetujuan;
                cuti.CatatanHRD = request.CatatanHRD;

                await _context.SaveChangesAsync();
                Console.WriteLine($"[DEBUG] Database updated successfully");

                // FIXED: Build response WITHOUT TglDisetujui and DisetujuiOleh since they don't exist in CutiResponse DTO
                var response = new CutiResponse
                {
                    IdCuti = cuti.IdCuti,
                    IdKaryawan = cuti.IdKaryawan,
                    NamaKaryawan = cuti.Karyawan.NamaLengkap,
                    NIKKaryawan = cuti.Karyawan.NIK,
                    DivisiKaryawan = cuti.Karyawan.Divisi,
                    JenisCuti = cuti.JenisCuti,
                    TglMulai = cuti.TglMulai,
                    TglSelesai = cuti.TglSelesai,
                    JumlahHari = cuti.JumlahHari,
                    Alasan = cuti.Alasan,
                    StatusPersetujuan = cuti.StatusPersetujuan,
                    CatatanHRD = cuti.CatatanHRD,
                    TglDibuat = cuti.TglDibuat

                    // REMOVED: These don't exist in CutiResponse DTO
                    // TglDisetujui = DateTime.Now,
                    // DisetujuiOleh = "HRD Admin"
                };

                var message = $"Cuti berhasil {request.StatusPersetujuan.ToLower()}";
                Console.WriteLine($"[DEBUG] Success: {message}");

                return ApiResponse<CutiResponse>.SuccessResult(response, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DEBUG] Exception in ApproveAsync: {ex.Message}");
                Console.WriteLine($"[DEBUG] Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
                return ApiResponse<CutiResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id, int karyawanId)
        {
            try
            {
                var cuti = await _context.Cuti.FirstOrDefaultAsync(c => c.IdCuti == id && c.IdKaryawan == karyawanId);

                if (cuti == null)
                {
                    return ApiResponse<string>.ErrorResult("Data cuti tidak ditemukan");
                }

                if (cuti.StatusPersetujuan != "Pending")
                {
                    return ApiResponse<string>.ErrorResult("Hanya cuti dengan status pending yang dapat dihapus");
                }

                _context.Cuti.Remove(cuti);
                await _context.SaveChangesAsync();

                return ApiResponse<string>.SuccessResult("", "Pengajuan cuti berhasil dihapus");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CutiListResponse>>> GetByKaryawanAsync(int karyawanId)
        {
            return await GetAllAsync(null, karyawanId);
        }

        public async Task<ApiResponse<CutiStatsResponse>> GetStatsAsync()
        {
            try
            {
                var today = DateTime.Today;

                var stats = new CutiStatsResponse
                {
                    TotalPengajuan = await _context.Cuti.CountAsync(),
                    MenungguPersetujuan = await _context.Cuti.CountAsync(c => c.StatusPersetujuan == "Pending"),
                    Disetujui = await _context.Cuti.CountAsync(c => c.StatusPersetujuan == "Disetujui"),
                    Ditolak = await _context.Cuti.CountAsync(c => c.StatusPersetujuan == "Ditolak"),
                    CutiHariBerjalan = await _context.Cuti.CountAsync(c =>
                        c.StatusPersetujuan == "Disetujui" &&
                        c.TglMulai <= today &&
                        c.TglSelesai >= today)
                };

                return ApiResponse<CutiStatsResponse>.SuccessResult(stats, "Statistik cuti berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<CutiStatsResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<string>>> GetJenisCutiAsync()
        {
            try
            {
                var jenisCuti = new List<string>
                {
                    "Tahunan",
                    "Sakit",
                    "Melahirkan",
                    "Menikah",
                    "Duka Cita",
                    "Izin Khusus"
                };

                return ApiResponse<List<string>>.SuccessResult(jenisCuti, "Jenis cuti berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<string>>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        private int CalculateWorkingDays(DateTime startDate, DateTime endDate)
        {
            int workingDays = 0;
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // Exclude weekends (Saturday = 6, Sunday = 0)
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }
            return workingDays;
        }
    }
}