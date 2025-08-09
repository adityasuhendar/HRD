// HRD.API/Models/DTOs/Request/KaryawanRequest.cs
using System.ComponentModel.DataAnnotations;

namespace HRD.API.Models.DTOs.Request
{
    public class CreateKaryawanRequest
    {
        [Required(ErrorMessage = "Username wajib diisi")]
        [StringLength(50, ErrorMessage = "Username maksimal 50 karakter")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "NIK wajib diisi")]
        [StringLength(20, ErrorMessage = "NIK maksimal 20 karakter")]
        public string NIK { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama lengkap wajib diisi")]
        [StringLength(100, ErrorMessage = "Nama lengkap maksimal 100 karakter")]
        public string NamaLengkap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email kantor wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email kantor tidak valid")]
        public string EmailKantor { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "No telepon maksimal 20 karakter")]
        public string? NoTelepon { get; set; }

        [Required(ErrorMessage = "Posisi wajib diisi")]
        [StringLength(50, ErrorMessage = "Posisi maksimal 50 karakter")]
        public string Posisi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Divisi wajib diisi")]
        [StringLength(50, ErrorMessage = "Divisi maksimal 50 karakter")]
        public string Divisi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tanggal bergabung wajib diisi")]
        public DateTime TglBergabung { get; set; }

        [Required(ErrorMessage = "Gaji pokok wajib diisi")]
        [Range(0, double.MaxValue, ErrorMessage = "Gaji pokok harus lebih dari 0")]
        public decimal GajiPokok { get; set; }
    }

    public class UpdateKaryawanRequest
    {
        [Required(ErrorMessage = "NIK wajib diisi")]
        [StringLength(20, ErrorMessage = "NIK maksimal 20 karakter")]
        public string NIK { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama lengkap wajib diisi")]
        [StringLength(100, ErrorMessage = "Nama lengkap maksimal 100 karakter")]
        public string NamaLengkap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email kantor wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email kantor tidak valid")]
        public string EmailKantor { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "No telepon maksimal 20 karakter")]
        public string? NoTelepon { get; set; }

        [Required(ErrorMessage = "Posisi wajib diisi")]
        [StringLength(50, ErrorMessage = "Posisi maksimal 50 karakter")]
        public string Posisi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Divisi wajib diisi")]
        [StringLength(50, ErrorMessage = "Divisi maksimal 50 karakter")]
        public string Divisi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tanggal bergabung wajib diisi")]
        public DateTime TglBergabung { get; set; }

        [Required(ErrorMessage = "Gaji pokok wajib diisi")]
        [Range(0, double.MaxValue, ErrorMessage = "Gaji pokok harus lebih dari 0")]
        public decimal GajiPokok { get; set; }

        [Required(ErrorMessage = "Status karyawan wajib diisi")]
        public string StatusKaryawan { get; set; } = "Aktif";
    }
}