// HRD.Web/Models/DTOs/KaryawanModels.cs - Enhanced with Validation

using System.ComponentModel.DataAnnotations;

namespace HRD.Web.Models.DTOs
{
    public class CreateKaryawanRequest
    {
        [Required(ErrorMessage = "Username wajib diisi")]
        [StringLength(50, ErrorMessage = "Username maksimal 50 karakter")]
        [RegularExpression(@"^[a-zA-Z0-9._]+$", ErrorMessage = "Username hanya boleh mengandung huruf, angka, titik, dan underscore")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email personal wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        [StringLength(100, ErrorMessage = "Email maksimal 100 karakter")]
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
        [StringLength(100, ErrorMessage = "Email kantor maksimal 100 karakter")]
        public string EmailKantor { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Nomor telepon maksimal 20 karakter")]
        [RegularExpression(@"^[0-9\-\+\s\(\)]+$", ErrorMessage = "Format nomor telepon tidak valid")]
        public string? NoTelepon { get; set; }

        [Required(ErrorMessage = "Posisi wajib dipilih")]
        [StringLength(50, ErrorMessage = "Posisi maksimal 50 karakter")]
        public string Posisi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Divisi wajib dipilih")]
        [StringLength(50, ErrorMessage = "Divisi maksimal 50 karakter")]
        public string Divisi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tanggal bergabung wajib diisi")]
        [DataType(DataType.Date)]
        public DateTime TglBergabung { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Gaji pokok wajib diisi")]
        [Range(1000000, 100000000, ErrorMessage = "Gaji pokok harus antara Rp 1.000.000 - Rp 100.000.000")]
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
        [StringLength(100, ErrorMessage = "Email kantor maksimal 100 karakter")]
        public string EmailKantor { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Nomor telepon maksimal 20 karakter")]
        public string? NoTelepon { get; set; }

        [Required(ErrorMessage = "Posisi wajib dipilih")]
        [StringLength(50, ErrorMessage = "Posisi maksimal 50 karakter")]
        public string Posisi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Divisi wajib dipilih")]
        [StringLength(50, ErrorMessage = "Divisi maksimal 50 karakter")]
        public string Divisi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tanggal bergabung wajib diisi")]
        [DataType(DataType.Date)]
        public DateTime TglBergabung { get; set; }

        [Required(ErrorMessage = "Gaji pokok wajib diisi")]
        [Range(1000000, 100000000, ErrorMessage = "Gaji pokok harus antara Rp 1.000.000 - Rp 100.000.000")]
        public decimal GajiPokok { get; set; }

        [Required(ErrorMessage = "Status karyawan wajib dipilih")]
        public string StatusKaryawan { get; set; } = "Aktif";
    }

    public class KaryawanResponse
    {
        public int IdKaryawan { get; set; }
        public int IdPengguna { get; set; }
        public string NIK { get; set; } = string.Empty;
        public string NamaLengkap { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string EmailKantor { get; set; } = string.Empty;
        public string? NoTelepon { get; set; }
        public string Posisi { get; set; } = string.Empty;
        public string Divisi { get; set; } = string.Empty;
        public DateTime TglBergabung { get; set; }
        public decimal GajiPokok { get; set; }
        public string StatusKaryawan { get; set; } = string.Empty;
        public DateTime TglDibuat { get; set; }
        public bool Aktif { get; set; }
    }
}