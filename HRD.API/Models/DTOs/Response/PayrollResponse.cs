// HRD.API/Models/DTOs/Response/PayrollResponse.cs
namespace HRD.API.Models.DTOs.Response
{
    public class PayrollResponse
    {
        public int IdGaji { get; set; }
        public int IdKaryawan { get; set; }
        public string NIK { get; set; } = string.Empty;
        public string NamaLengkap { get; set; } = string.Empty;
        public string Posisi { get; set; } = string.Empty;
        public string Divisi { get; set; } = string.Empty;
        public int Bulan { get; set; }
        public int Tahun { get; set; }
        public decimal GajiPokok { get; set; }
        public decimal Tunjangan { get; set; }
        public decimal Potongan { get; set; }
        public int TotalJamKerja { get; set; }
        public decimal TotalGaji { get; set; }
        public string StatusBayar { get; set; } = string.Empty;
        public DateTime TglDibuat { get; set; }
        public string? Keterangan { get; set; }
    }

    public class PayrollSummaryResponse
    {
        public int Bulan { get; set; }
        public int Tahun { get; set; }
        public int JumlahKaryawan { get; set; }
        public decimal TotalGajiPokok { get; set; }
        public decimal TotalTunjangan { get; set; }
        public decimal TotalPotongan { get; set; }
        public decimal TotalPayroll { get; set; }
        public int SudahBayar { get; set; }
        public int BelumBayar { get; set; }
        public List<PayrollResponse> DetailPayroll { get; set; } = new();
    }
}