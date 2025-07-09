// HRD.Web/Models/DTOs/PresensiModels.cs
namespace HRD.Web.Models.DTOs
{
    public class PresensiResponse
    {
        public int IdPresensi { get; set; }
        public int IdKaryawan { get; set; }
        public string NamaKaryawan { get; set; } = string.Empty;
        public string NIKKaryawan { get; set; } = string.Empty;
        public DateTime Tanggal { get; set; }
        public DateTime? JamMasuk { get; set; }
        public DateTime? JamKeluar { get; set; }
        public decimal TotalJam { get; set; }
        public string StatusPresensi { get; set; } = string.Empty;
        public string MetodePresensi { get; set; } = string.Empty;
        public string? DeviceId { get; set; }
        public string? Catatan { get; set; }
        public DateTime TglDibuat { get; set; }
    }

    public class ClockInRequest
    {
        public string? Catatan { get; set; }
        public string? DeviceId { get; set; }
        public string MetodePresensi { get; set; } = "Web";
    }

    public class ClockOutRequest
    {
        public string? Catatan { get; set; }
    }

    public class ClockInResponse
    {
        public int IdPresensi { get; set; }
        public DateTime JamMasuk { get; set; }
        public string StatusPresensi { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsTerlambat { get; set; }
    }

    public class ClockOutResponse
    {
        public int IdPresensi { get; set; }
        public DateTime JamKeluar { get; set; }
        public decimal TotalJamKerja { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsLembur { get; set; }
    }
}