// HRD.API/Services/Implementations/PresensiService.cs
using Microsoft.EntityFrameworkCore;
using HRD.API.Data;
using HRD.API.Models.Entities;
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response;
using HRD.API.Services.Interfaces;

namespace HRD.API.Services.Implementations
{
    public class PresensiService : IPresensiService
    {
        private readonly HRDDbContext _context;

        public PresensiService(HRDDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<ClockInResponse>> ClockInAsync(ClockInRequest request, int karyawanId)
        {
            try
            {
                var today = DateTime.Today;

                // Check if already clocked in today
                var existingPresensi = await _context.Presensi
                    .FirstOrDefaultAsync(p => p.IdKaryawan == karyawanId && p.Tanggal == today);

                if (existingPresensi != null)
                {
                    return ApiResponse<ClockInResponse>.ErrorResult("Anda sudah melakukan clock in hari ini");
                }

                // Check if employee exists
                var karyawan = await _context.Karyawan.FindAsync(karyawanId);
                if (karyawan == null)
                {
                    return ApiResponse<ClockInResponse>.ErrorResult("Karyawan tidak ditemukan");
                }

                var now = DateTime.Now;
                var jamMasukStandar = new TimeSpan(9, 0, 0); // 09:00 AM
                var isTerlambat = now.TimeOfDay > jamMasukStandar;

                var presensi = new Presensi
                {
                    IdKaryawan = karyawanId,
                    Tanggal = today,
                    JamMasuk = now,
                    StatusPresensi = isTerlambat ? "Terlambat" : "Hadir",
                    MetodePresensi = request.MetodePresensi,
                    DeviceId = request.DeviceId,
                    Catatan = request.Catatan,
                    TglDibuat = now
                };

                _context.Presensi.Add(presensi);
                await _context.SaveChangesAsync();

                var response = new ClockInResponse
                {
                    IdPresensi = presensi.IdPresensi,
                    JamMasuk = presensi.JamMasuk.Value,
                    StatusPresensi = presensi.StatusPresensi,
                    IsTerlambat = isTerlambat,
                    Message = isTerlambat ?
                        $"Clock in berhasil - Terlambat {(now.TimeOfDay - jamMasukStandar).TotalMinutes:F0} menit" :
                        "Clock in berhasil - Tepat waktu"
                };

                return ApiResponse<ClockInResponse>.SuccessResult(response, response.Message);
            }
            catch (Exception ex)
            {
                return ApiResponse<ClockInResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ClockOutResponse>> ClockOutAsync(ClockOutRequest request, int karyawanId)
        {
            try
            {
                var today = DateTime.Today;

                // Find today's attendance record
                var presensi = await _context.Presensi
                    .FirstOrDefaultAsync(p => p.IdKaryawan == karyawanId && p.Tanggal == today);

                if (presensi == null)
                {
                    return ApiResponse<ClockOutResponse>.ErrorResult("Anda belum melakukan clock in hari ini");
                }

                if (presensi.JamKeluar.HasValue)
                {
                    return ApiResponse<ClockOutResponse>.ErrorResult("Anda sudah melakukan clock out hari ini");
                }

                var now = DateTime.Now;
                var totalJam = (decimal)(now - presensi.JamMasuk!.Value).TotalHours;
                var jamKerjaStandar = 8m; // 8 hours
                var isLembur = totalJam > jamKerjaStandar;

                // Update attendance record
                presensi.JamKeluar = now;
                presensi.TotalJam = Math.Round(totalJam, 2);

                if (!string.IsNullOrEmpty(request.Catatan))
                {
                    presensi.Catatan = string.IsNullOrEmpty(presensi.Catatan) ?
                        request.Catatan :
                        $"{presensi.Catatan}; {request.Catatan}";
                }

                await _context.SaveChangesAsync();

                var response = new ClockOutResponse
                {
                    IdPresensi = presensi.IdPresensi,
                    JamKeluar = presensi.JamKeluar.Value,
                    TotalJamKerja = presensi.TotalJam,
                    IsLembur = isLembur,
                    Message = isLembur ?
                        $"Clock out berhasil - Total {totalJam:F1} jam (Lembur {totalJam - jamKerjaStandar:F1} jam)" :
                        $"Clock out berhasil - Total {totalJam:F1} jam kerja"
                };

                return ApiResponse<ClockOutResponse>.SuccessResult(response, response.Message);
            }
            catch (Exception ex)
            {
                return ApiResponse<ClockOutResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<PresensiListResponse>>> GetAllAsync(DateTime? startDate = null, DateTime? endDate = null, int? karyawanId = null)
        {
            try
            {
                var query = _context.Presensi
                    .Include(p => p.Karyawan)
                    .AsQueryable();

                // Filter by date range
                if (startDate.HasValue)
                {
                    query = query.Where(p => p.Tanggal >= startDate.Value.Date);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(p => p.Tanggal <= endDate.Value.Date);
                }

                // Filter by employee
                if (karyawanId.HasValue)
                {
                    query = query.Where(p => p.IdKaryawan == karyawanId.Value);
                }

                var presensiList = await query
                    .OrderByDescending(p => p.Tanggal)
                    .ThenByDescending(p => p.JamMasuk)
                    .Select(p => new PresensiListResponse
                    {
                        IdPresensi = p.IdPresensi,
                        NamaKaryawan = p.Karyawan.NamaLengkap,
                        NIKKaryawan = p.Karyawan.NIK,
                        Tanggal = p.Tanggal,
                        JamMasuk = p.JamMasuk,
                        JamKeluar = p.JamKeluar,
                        TotalJam = p.TotalJam,
                        StatusPresensi = p.StatusPresensi,
                        MetodePresensi = p.MetodePresensi
                    })
                    .ToListAsync();

                return ApiResponse<List<PresensiListResponse>>.SuccessResult(presensiList, "Data presensi berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<PresensiListResponse>>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PresensiResponse>> GetByIdAsync(int id)
        {
            try
            {
                var presensi = await _context.Presensi
                    .Include(p => p.Karyawan)
                    .FirstOrDefaultAsync(p => p.IdPresensi == id);

                if (presensi == null)
                {
                    return ApiResponse<PresensiResponse>.ErrorResult("Data presensi tidak ditemukan");
                }

                var response = new PresensiResponse
                {
                    IdPresensi = presensi.IdPresensi,
                    IdKaryawan = presensi.IdKaryawan,
                    NamaKaryawan = presensi.Karyawan.NamaLengkap,
                    NIKKaryawan = presensi.Karyawan.NIK,
                    Tanggal = presensi.Tanggal,
                    JamMasuk = presensi.JamMasuk,
                    JamKeluar = presensi.JamKeluar,
                    TotalJam = presensi.TotalJam,
                    StatusPresensi = presensi.StatusPresensi,
                    MetodePresensi = presensi.MetodePresensi,
                    DeviceId = presensi.DeviceId,
                    Catatan = presensi.Catatan,
                    TglDibuat = presensi.TglDibuat
                };

                return ApiResponse<PresensiResponse>.SuccessResult(response, "Data presensi berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<PresensiResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PresensiResponse>> CreateManualAsync(ManualPresensiRequest request)
        {
            try
            {
                // Check if attendance already exists for this date and employee
                var existingPresensi = await _context.Presensi
                    .FirstOrDefaultAsync(p => p.IdKaryawan == request.IdKaryawan && p.Tanggal == request.Tanggal.Date);

                if (existingPresensi != null)
                {
                    return ApiResponse<PresensiResponse>.ErrorResult("Presensi untuk tanggal ini sudah ada");
                }

                // Check if employee exists
                var karyawan = await _context.Karyawan.FindAsync(request.IdKaryawan);
                if (karyawan == null)
                {
                    return ApiResponse<PresensiResponse>.ErrorResult("Karyawan tidak ditemukan");
                }

                var totalJam = 0m;
                if (request.JamKeluar.HasValue)
                {
                    totalJam = (decimal)(request.JamKeluar.Value - request.JamMasuk).TotalHours;
                }

                var presensi = new Presensi
                {
                    IdKaryawan = request.IdKaryawan,
                    Tanggal = request.Tanggal.Date,
                    JamMasuk = request.JamMasuk,
                    JamKeluar = request.JamKeluar,
                    TotalJam = Math.Round(totalJam, 2),
                    StatusPresensi = request.StatusPresensi,
                    MetodePresensi = "Manual",
                    Catatan = request.Catatan,
                    TglDibuat = DateTime.Now
                };

                _context.Presensi.Add(presensi);
                await _context.SaveChangesAsync();

                var response = new PresensiResponse
                {
                    IdPresensi = presensi.IdPresensi,
                    IdKaryawan = presensi.IdKaryawan,
                    NamaKaryawan = karyawan.NamaLengkap,
                    NIKKaryawan = karyawan.NIK,
                    Tanggal = presensi.Tanggal,
                    JamMasuk = presensi.JamMasuk,
                    JamKeluar = presensi.JamKeluar,
                    TotalJam = presensi.TotalJam,
                    StatusPresensi = presensi.StatusPresensi,
                    MetodePresensi = presensi.MetodePresensi,
                    DeviceId = presensi.DeviceId,
                    Catatan = presensi.Catatan,
                    TglDibuat = presensi.TglDibuat
                };

                return ApiResponse<PresensiResponse>.SuccessResult(response, "Presensi manual berhasil dibuat");
            }
            catch (Exception ex)
            {
                return ApiResponse<PresensiResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PresensiResponse>> UpdateAsync(int id, ManualPresensiRequest request)
        {
            try
            {
                var presensi = await _context.Presensi
                    .Include(p => p.Karyawan)
                    .FirstOrDefaultAsync(p => p.IdPresensi == id);

                if (presensi == null)
                {
                    return ApiResponse<PresensiResponse>.ErrorResult("Data presensi tidak ditemukan");
                }

                var totalJam = 0m;
                if (request.JamKeluar.HasValue)
                {
                    totalJam = (decimal)(request.JamKeluar.Value - request.JamMasuk).TotalHours;
                }

                // Update presensi data
                presensi.Tanggal = request.Tanggal.Date;
                presensi.JamMasuk = request.JamMasuk;
                presensi.JamKeluar = request.JamKeluar;
                presensi.TotalJam = Math.Round(totalJam, 2);
                presensi.StatusPresensi = request.StatusPresensi;
                presensi.Catatan = request.Catatan;

                await _context.SaveChangesAsync();

                var response = new PresensiResponse
                {
                    IdPresensi = presensi.IdPresensi,
                    IdKaryawan = presensi.IdKaryawan,
                    NamaKaryawan = presensi.Karyawan.NamaLengkap,
                    NIKKaryawan = presensi.Karyawan.NIK,
                    Tanggal = presensi.Tanggal,
                    JamMasuk = presensi.JamMasuk,
                    JamKeluar = presensi.JamKeluar,
                    TotalJam = presensi.TotalJam,
                    StatusPresensi = presensi.StatusPresensi,
                    MetodePresensi = presensi.MetodePresensi,
                    DeviceId = presensi.DeviceId,
                    Catatan = presensi.Catatan,
                    TglDibuat = presensi.TglDibuat
                };

                return ApiResponse<PresensiResponse>.SuccessResult(response, "Data presensi berhasil diperbarui");
            }
            catch (Exception ex)
            {
                return ApiResponse<PresensiResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            try
            {
                var presensi = await _context.Presensi.FindAsync(id);

                if (presensi == null)
                {
                    return ApiResponse<string>.ErrorResult("Data presensi tidak ditemukan");
                }

                _context.Presensi.Remove(presensi);
                await _context.SaveChangesAsync();

                return ApiResponse<string>.SuccessResult("", "Data presensi berhasil dihapus");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<PresensiListResponse>>> GetByKaryawanAsync(int karyawanId, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await GetAllAsync(startDate, endDate, karyawanId);
        }

        public async Task<ApiResponse<PresensiStatsResponse>> GetStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30); // Default last 30 days
                var end = endDate ?? DateTime.Today;

                var presensiData = await _context.Presensi
                    .Include(p => p.Karyawan)
                    .Where(p => p.Tanggal >= start && p.Tanggal <= end)
                    .ToListAsync();

                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                var stats = new PresensiStatsResponse
                {
                    TotalHadirBulanIni = presensiData.Count(p => p.Tanggal >= thisMonth && p.StatusPresensi == "Hadir"),
                    TotalTerlambatBulanIni = presensiData.Count(p => p.Tanggal >= thisMonth && p.StatusPresensi == "Terlambat"),
                    TotalAlphaBulanIni = presensiData.Count(p => p.Tanggal >= thisMonth && p.StatusPresensi == "Alpha"),

                    // FIX: Cast double to decimal explicitly
                    RataRataJamKerja = (decimal)(presensiData.Where(p => p.TotalJam > 0).Average(p => (double?)p.TotalJam) ?? 0),

                    KaryawanHadirHariIni = presensiData.Count(p => p.Tanggal == today),
                    KaryawanBelumClockOut = presensiData.Count(p => p.Tanggal == today && !p.JamKeluar.HasValue),
                    StatistikHarian = presensiData
                         .GroupBy(p => p.Tanggal)
                         .Select(g => new PresensiHarianStats
                         {
                             Tanggal = g.Key,
                             JumlahHadir = g.Count(p => p.StatusPresensi == "Hadir"),
                             JumlahTerlambat = g.Count(p => p.StatusPresensi == "Terlambat"),
                             JumlahAlpha = g.Count(p => p.StatusPresensi == "Alpha")
                         })
                         .OrderBy(s => s.Tanggal)
                         .ToList()
                };

                return ApiResponse<PresensiStatsResponse>.SuccessResult(stats, "Statistik presensi berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<PresensiStatsResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PresensiResponse?>> GetTodayPresenceAsync(int karyawanId)
        {
            try
            {
                var today = DateTime.Today;
                var presensi = await _context.Presensi
                    .Include(p => p.Karyawan)
                    .FirstOrDefaultAsync(p => p.IdKaryawan == karyawanId && p.Tanggal == today);

                if (presensi == null)
                {
                    return ApiResponse<PresensiResponse?>.SuccessResult(null, "Belum ada presensi hari ini");
                }

                var response = new PresensiResponse
                {
                    IdPresensi = presensi.IdPresensi,
                    IdKaryawan = presensi.IdKaryawan,
                    NamaKaryawan = presensi.Karyawan.NamaLengkap,
                    NIKKaryawan = presensi.Karyawan.NIK,
                    Tanggal = presensi.Tanggal,
                    JamMasuk = presensi.JamMasuk,
                    JamKeluar = presensi.JamKeluar,
                    TotalJam = presensi.TotalJam,
                    StatusPresensi = presensi.StatusPresensi,
                    MetodePresensi = presensi.MetodePresensi,
                    DeviceId = presensi.DeviceId,
                    Catatan = presensi.Catatan,
                    TglDibuat = presensi.TglDibuat
                };

                return ApiResponse<PresensiResponse?>.SuccessResult(response, "Presensi hari ini berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<PresensiResponse?>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }
    }
}