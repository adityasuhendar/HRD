// HRD.API/Models/DTOs/Request/PayrollRequest.cs
using System.ComponentModel.DataAnnotations;

namespace HRD.API.Models.DTOs.Request
{
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
        [Required(ErrorMessage = "ID Gaji wajib diisi")]
        public int IdGaji { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tunjangan harus >= 0")]
        public decimal Tunjangan { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Potongan harus >= 0")]
        public decimal Potongan { get; set; }

        [Range(0, 300, ErrorMessage = "Total jam kerja harus antara 0-300")]
        public int TotalJamKerja { get; set; }

        public string? Keterangan { get; set; }
    }

    public class ProcessPaymentRequest
    {
        [Required(ErrorMessage = "ID Gaji wajib diisi")]
        public List<int> IdGajiList { get; set; } = new();
    }
}