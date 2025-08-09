// HRD.Web/Models/DTOs/PayrollModels.cs
using System.ComponentModel.DataAnnotations;

namespace HRD.Web.Models.DTOs
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

    public class GeneratePayrollRequest
    {
        [Required(ErrorMessage = "Bulan wajib diisi")]
        [Range(1, 12, ErrorMessage = "Bulan harus antara 1-12")]
        public int Bulan { get; set; }

        [Required(ErrorMessage = "Tahun wajib diisi")]
        [Range(2020, 2030, ErrorMessage = "Tahun harus antara 2020-2030")]
        public int Tahun { get; set; }
    }

    public class UpdatePayrollRequest
    {
        [Range(0, double.MaxValue, ErrorMessage = "Tunjangan harus >= 0")]
        public decimal Tunjangan { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Potongan harus >= 0")]
        public decimal Potongan { get; set; }

        [Range(0, 300, ErrorMessage = "Total jam kerja harus antara 0-300")]
        public int TotalJamKerja { get; set; }

        public string? Keterangan { get; set; }
    }
}
