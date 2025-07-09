// HRD.API/Models/DTOs/Response/KaryawanResponse.cs
namespace HRD.API.Models.DTOs.Response
{
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

    public class KaryawanListResponse
    {
        public int IdKaryawan { get; set; }
        public string NIK { get; set; } = string.Empty;
        public string NamaLengkap { get; set; } = string.Empty;
        public string EmailKantor { get; set; } = string.Empty;
        public string Posisi { get; set; } = string.Empty;
        public string Divisi { get; set; } = string.Empty;
        public DateTime TglBergabung { get; set; }
        public decimal GajiPokok { get; set; } // ← ENSURE THIS EXISTS

        public string StatusKaryawan { get; set; } = string.Empty;
        public bool Aktif { get; set; }
    }
}