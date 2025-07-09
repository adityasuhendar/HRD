// HRD.Web/Models/DTOs/CutiModels.cs
namespace HRD.Web.Models.DTOs
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

    public class CreateCutiRequest
    {
        public string JenisCuti { get; set; } = string.Empty;
        public DateTime TglMulai { get; set; }
        public DateTime TglSelesai { get; set; }
        public string? Alasan { get; set; }
    }

    public class ApproveCutiRequest
    {
        public string StatusPersetujuan { get; set; } = string.Empty;
        public string? CatatanHRD { get; set; }
    }
}
