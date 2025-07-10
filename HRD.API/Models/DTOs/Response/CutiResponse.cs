// HRD.API/Models/DTOs/Response/CutiResponse.cs
namespace HRD.API.Models.DTOs.Response
{
    public class CutiResponse
    {
        public int IdCuti { get; set; }
        public int IdKaryawan { get; set; }
        public string NamaKaryawan { get; set; } = string.Empty;
        public string NIKKaryawan { get; set; } = string.Empty;
        public string DivisiKaryawan { get; set; } = string.Empty;
        public string JenisCuti { get; set; } = string.Empty;
        public DateTime TglMulai { get; set; }
        public DateTime TglSelesai { get; set; }
        public int JumlahHari { get; set; }
        public string? Alasan { get; set; }
        public string StatusPersetujuan { get; set; } = string.Empty;
        public string? CatatanHRD { get; set; }
        public DateTime TglDibuat { get; set; }
        public DateTime? TglDisetujui { get; set; }
        public string? DisetujuiOleh { get; set; }
    }

    public class CutiListResponse
    {
        public int IdCuti { get; set; }
        public string NamaKaryawan { get; set; } = string.Empty;
        public string NIKKaryawan { get; set; } = string.Empty;
        public string JenisCuti { get; set; } = string.Empty;
        public DateTime TglMulai { get; set; }
        public DateTime TglSelesai { get; set; }
        public int JumlahHari { get; set; }
        public string StatusPersetujuan { get; set; } = string.Empty;
        public DateTime TglDibuat { get; set; }
    }

    public class CutiStatsResponse
    {
        public int TotalPengajuan { get; set; }
        public int MenungguPersetujuan { get; set; }
        public int Disetujui { get; set; }
        public int Ditolak { get; set; }
        public int CutiHariBerjalan { get; set; } // Karyawan yang sedang cuti hari ini
    }
}