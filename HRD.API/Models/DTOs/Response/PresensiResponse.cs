// HRD.API/Models/DTOs/Response/PresensiResponse.cs
namespace HRD.API.Models.DTOs.Response
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

    public class PresensiListResponse
    {
        public int IdPresensi { get; set; }
        public string NamaKaryawan { get; set; } = string.Empty;
        public string NIKKaryawan { get; set; } = string.Empty;
        public DateTime Tanggal { get; set; }
        public DateTime? JamMasuk { get; set; }
        public DateTime? JamKeluar { get; set; }
        public decimal TotalJam { get; set; }
        public string StatusPresensi { get; set; } = string.Empty;
        public string MetodePresensi { get; set; } = string.Empty;
    }

    public class PresensiStatsResponse
    {
        public int TotalHadirBulanIni { get; set; }
        public int TotalTerlambatBulanIni { get; set; }
        public int TotalAlphaBulanIni { get; set; }
        public decimal RataRataJamKerja { get; set; }
        public int KaryawanHadirHariIni { get; set; }
        public int KaryawanBelumClockOut { get; set; }
        public List<PresensiHarianStats> StatistikHarian { get; set; } = new();
    }

    public class PresensiHarianStats
    {
        public DateTime Tanggal { get; set; }
        public int JumlahHadir { get; set; }
        public int JumlahTerlambat { get; set; }
        public int JumlahAlpha { get; set; }
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