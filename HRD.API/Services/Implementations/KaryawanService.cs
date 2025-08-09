// HRD.API/Services/Implementations/KaryawanService.cs
using Microsoft.EntityFrameworkCore;
using HRD.API.Data;
using HRD.API.Models.Entities;
using HRD.API.Models.DTOs.Request;
using HRD.API.Models.DTOs.Response;
using HRD.API.Services.Interfaces;

namespace HRD.API.Services.Implementations
{
    public class KaryawanService : IKaryawanService
    {
        private readonly HRDDbContext _context;

        public KaryawanService(HRDDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<List<KaryawanListResponse>>> GetAllAsync(string? search = null, string? divisi = null)
        {
            try
            {
                var query = _context.Karyawan
                    .Include(k => k.Pengguna)
                    .AsQueryable();

                // Filter by search (nama, nik, email)
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(k =>
                        k.NamaLengkap.Contains(search) ||
                        k.NIK.Contains(search) ||
                        k.EmailKantor.Contains(search) ||
                        k.Posisi.Contains(search));
                }

                // Filter by divisi
                if (!string.IsNullOrEmpty(divisi))
                {
                    query = query.Where(k => k.Divisi == divisi);
                }

                var karyawan = await query
                    .OrderBy(k => k.NamaLengkap)
                    .Select(k => new KaryawanListResponse
                    {
                        IdKaryawan = k.IdKaryawan,
                        NIK = k.NIK,
                        NamaLengkap = k.NamaLengkap,
                        EmailKantor = k.EmailKantor,
                        Posisi = k.Posisi,
                        Divisi = k.Divisi,
                        TglBergabung = k.TglBergabung,
                        GajiPokok = k.GajiPokok, // ← ADD THIS MISSING LINE
                        StatusKaryawan = k.StatusKaryawan,
                        Aktif = k.Pengguna.Aktif
                    })
                    .ToListAsync();

                return ApiResponse<List<KaryawanListResponse>>.SuccessResult(karyawan, "Data karyawan berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<KaryawanListResponse>>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<KaryawanResponse>> GetByIdAsync(int id)
        {
            try
            {
                var karyawan = await _context.Karyawan
                    .Include(k => k.Pengguna)
                    .FirstOrDefaultAsync(k => k.IdKaryawan == id);

                if (karyawan == null)
                {
                    return ApiResponse<KaryawanResponse>.ErrorResult("Karyawan tidak ditemukan");
                }

                var response = new KaryawanResponse
                {
                    IdKaryawan = karyawan.IdKaryawan,
                    IdPengguna = karyawan.IdPengguna,
                    NIK = karyawan.NIK,
                    NamaLengkap = karyawan.NamaLengkap,
                    Username = karyawan.Pengguna.Username,
                    Email = karyawan.Pengguna.Email,
                    EmailKantor = karyawan.EmailKantor,
                    NoTelepon = karyawan.NoTelepon,
                    Posisi = karyawan.Posisi,
                    Divisi = karyawan.Divisi,
                    TglBergabung = karyawan.TglBergabung,
                    GajiPokok = karyawan.GajiPokok,
                    StatusKaryawan = karyawan.StatusKaryawan,
                    TglDibuat = karyawan.TglDibuat,
                    Aktif = karyawan.Pengguna.Aktif
                };

                return ApiResponse<KaryawanResponse>.SuccessResult(response, "Data karyawan berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<KaryawanResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<KaryawanResponse>> CreateAsync(CreateKaryawanRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check username exists
                if (await _context.Pengguna.AnyAsync(p => p.Username == request.Username))
                {
                    return ApiResponse<KaryawanResponse>.ErrorResult("Username sudah digunakan");
                }

                // Check email exists
                if (await _context.Pengguna.AnyAsync(p => p.Email == request.Email))
                {
                    return ApiResponse<KaryawanResponse>.ErrorResult("Email sudah digunakan");
                }

                // Check NIK exists
                if (await _context.Karyawan.AnyAsync(k => k.NIK == request.NIK))
                {
                    return ApiResponse<KaryawanResponse>.ErrorResult("NIK sudah digunakan");
                }

                // Check email kantor exists
                if (await _context.Karyawan.AnyAsync(k => k.EmailKantor == request.EmailKantor))
                {
                    return ApiResponse<KaryawanResponse>.ErrorResult("Email kantor sudah digunakan");
                }

                // Create user account
                var pengguna = new Pengguna
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Peran = "Karyawan",
                    Aktif = true,
                    TglDibuat = DateTime.Now
                };

                _context.Pengguna.Add(pengguna);
                await _context.SaveChangesAsync();

                // Create employee
                var karyawan = new Karyawan
                {
                    IdPengguna = pengguna.IdPengguna,
                    NIK = request.NIK,
                    NamaLengkap = request.NamaLengkap,
                    EmailKantor = request.EmailKantor,
                    NoTelepon = request.NoTelepon,
                    Posisi = request.Posisi,
                    Divisi = request.Divisi,
                    TglBergabung = request.TglBergabung,
                    GajiPokok = request.GajiPokok,
                    StatusKaryawan = "Aktif",
                    TglDibuat = DateTime.Now
                };

                _context.Karyawan.Add(karyawan);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Return created employee data
                var response = new KaryawanResponse
                {
                    IdKaryawan = karyawan.IdKaryawan,
                    IdPengguna = karyawan.IdPengguna,
                    NIK = karyawan.NIK,
                    NamaLengkap = karyawan.NamaLengkap,
                    Username = pengguna.Username,
                    Email = pengguna.Email,
                    EmailKantor = karyawan.EmailKantor,
                    NoTelepon = karyawan.NoTelepon,
                    Posisi = karyawan.Posisi,
                    Divisi = karyawan.Divisi,
                    TglBergabung = karyawan.TglBergabung,
                    GajiPokok = karyawan.GajiPokok,
                    StatusKaryawan = karyawan.StatusKaryawan,
                    TglDibuat = karyawan.TglDibuat,
                    Aktif = pengguna.Aktif
                };

                return ApiResponse<KaryawanResponse>.SuccessResult(response, "Karyawan berhasil ditambahkan");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<KaryawanResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<KaryawanResponse>> UpdateAsync(int id, UpdateKaryawanRequest request)
        {
            try
            {
                var karyawan = await _context.Karyawan
                    .Include(k => k.Pengguna)
                    .FirstOrDefaultAsync(k => k.IdKaryawan == id);

                if (karyawan == null)
                {
                    return ApiResponse<KaryawanResponse>.ErrorResult("Karyawan tidak ditemukan");
                }

                // Check NIK exists (exclude current)
                if (await _context.Karyawan.AnyAsync(k => k.NIK == request.NIK && k.IdKaryawan != id))
                {
                    return ApiResponse<KaryawanResponse>.ErrorResult("NIK sudah digunakan");
                }

                // Check email kantor exists (exclude current)
                if (await _context.Karyawan.AnyAsync(k => k.EmailKantor == request.EmailKantor && k.IdKaryawan != id))
                {
                    return ApiResponse<KaryawanResponse>.ErrorResult("Email kantor sudah digunakan");
                }

                // Update employee data
                karyawan.NIK = request.NIK;
                karyawan.NamaLengkap = request.NamaLengkap;
                karyawan.EmailKantor = request.EmailKantor;
                karyawan.NoTelepon = request.NoTelepon;
                karyawan.Posisi = request.Posisi;
                karyawan.Divisi = request.Divisi;
                karyawan.TglBergabung = request.TglBergabung;
                karyawan.GajiPokok = request.GajiPokok;
                karyawan.StatusKaryawan = request.StatusKaryawan;

                await _context.SaveChangesAsync();

                var response = new KaryawanResponse
                {
                    IdKaryawan = karyawan.IdKaryawan,
                    IdPengguna = karyawan.IdPengguna,
                    NIK = karyawan.NIK,
                    NamaLengkap = karyawan.NamaLengkap,
                    Username = karyawan.Pengguna.Username,
                    Email = karyawan.Pengguna.Email,
                    EmailKantor = karyawan.EmailKantor,
                    NoTelepon = karyawan.NoTelepon,
                    Posisi = karyawan.Posisi,
                    Divisi = karyawan.Divisi,
                    TglBergabung = karyawan.TglBergabung,
                    GajiPokok = karyawan.GajiPokok,
                    StatusKaryawan = karyawan.StatusKaryawan,
                    TglDibuat = karyawan.TglDibuat,
                    Aktif = karyawan.Pengguna.Aktif
                };

                return ApiResponse<KaryawanResponse>.SuccessResult(response, "Data karyawan berhasil diperbarui");
            }
            catch (Exception ex)
            {
                return ApiResponse<KaryawanResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var karyawan = await _context.Karyawan
                    .Include(k => k.Pengguna)
                    .FirstOrDefaultAsync(k => k.IdKaryawan == id);

                if (karyawan == null)
                {
                    return ApiResponse<string>.ErrorResult("Karyawan tidak ditemukan");
                }

                // Check if employee has related data (cuti, presensi, gaji)
                var hasRelatedData = await _context.Cuti.AnyAsync(c => c.IdKaryawan == id) ||
                                   await _context.Presensi.AnyAsync(p => p.IdKaryawan == id) ||
                                   await _context.Gaji.AnyAsync(g => g.IdKaryawan == id);

                if (hasRelatedData)
                {
                    return ApiResponse<string>.ErrorResult("Tidak dapat menghapus karyawan yang memiliki data terkait (cuti, presensi, atau gaji)");
                }

                // Delete employee and user
                _context.Karyawan.Remove(karyawan);
                _context.Pengguna.Remove(karyawan.Pengguna);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse<string>.SuccessResult("", "Karyawan berhasil dihapus");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<string>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> ActivateAsync(int id)
        {
            try
            {
                var karyawan = await _context.Karyawan
                    .Include(k => k.Pengguna)
                    .FirstOrDefaultAsync(k => k.IdKaryawan == id);

                if (karyawan == null)
                {
                    return ApiResponse<string>.ErrorResult("Karyawan tidak ditemukan");
                }

                karyawan.Pengguna.Aktif = true;
                karyawan.StatusKaryawan = "Aktif";

                await _context.SaveChangesAsync();

                return ApiResponse<string>.SuccessResult("", "Karyawan berhasil diaktifkan");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> DeactivateAsync(int id)
        {
            try
            {
                var karyawan = await _context.Karyawan
                    .Include(k => k.Pengguna)
                    .FirstOrDefaultAsync(k => k.IdKaryawan == id);

                if (karyawan == null)
                {
                    return ApiResponse<string>.ErrorResult("Karyawan tidak ditemukan");
                }

                karyawan.Pengguna.Aktif = false;
                karyawan.StatusKaryawan = "Nonaktif";

                await _context.SaveChangesAsync();

                return ApiResponse<string>.SuccessResult("", "Karyawan berhasil dinonaktifkan");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }
        public async Task<ApiResponse<KaryawanResponse>> GetByUserIdAsync(int userId)
        {
            try
            {
                var karyawan = await _context.Karyawan
                    .Include(k => k.Pengguna)
                    .FirstOrDefaultAsync(k => k.IdPengguna == userId);

                if (karyawan == null)
                {
                    return ApiResponse<KaryawanResponse>.ErrorResult("Data karyawan tidak ditemukan");
                }

                var response = new KaryawanResponse
                {
                    IdKaryawan = karyawan.IdKaryawan,
                    IdPengguna = karyawan.IdPengguna,
                    NIK = karyawan.NIK,
                    NamaLengkap = karyawan.NamaLengkap,
                    Username = karyawan.Pengguna.Username,
                    Email = karyawan.Pengguna.Email,
                    EmailKantor = karyawan.EmailKantor,
                    NoTelepon = karyawan.NoTelepon,
                    Posisi = karyawan.Posisi,
                    Divisi = karyawan.Divisi,
                    TglBergabung = karyawan.TglBergabung,
                    GajiPokok = karyawan.GajiPokok,
                    StatusKaryawan = karyawan.StatusKaryawan,
                    TglDibuat = karyawan.TglDibuat,
                    Aktif = karyawan.Pengguna.Aktif
                };

                return ApiResponse<KaryawanResponse>.SuccessResult(response, "Data karyawan berhasil diambil");
            }
            catch (Exception ex)
            {
                return ApiResponse<KaryawanResponse>.ErrorResult($"Terjadi kesalahan: {ex.Message}");
            }
        }
    }
}