// HRD.API/Models/DTOs/Request/CutiRequest.cs
using System.ComponentModel.DataAnnotations;

namespace HRD.API.Models.DTOs.Request
{
    public class CreateCutiRequest
    {
        [Required(ErrorMessage = "Jenis cuti wajib diisi")]
        [StringLength(30, ErrorMessage = "Jenis cuti maksimal 30 karakter")]
        public string JenisCuti { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tanggal mulai wajib diisi")]
        public DateTime TglMulai { get; set; }

        [Required(ErrorMessage = "Tanggal selesai wajib diisi")]
        public DateTime TglSelesai { get; set; }

        [StringLength(500, ErrorMessage = "Alasan maksimal 500 karakter")]
        public string? Alasan { get; set; }

        // Validate that end date is not before start date
        public bool IsValidDateRange()
        {
            return TglSelesai >= TglMulai;
        }
    }

    public class ApproveCutiRequest
    {
        [Required(ErrorMessage = "Status persetujuan wajib diisi")]
        public string StatusPersetujuan { get; set; } = string.Empty; // "Disetujui" atau "Ditolak"

        [StringLength(500, ErrorMessage = "Catatan HRD maksimal 500 karakter")]
        public string? CatatanHRD { get; set; }
    }

    public class EditCutiRequest
    {
        [Required(ErrorMessage = "Tanggal mulai wajib diisi")]
        public DateTime TglMulai { get; set; }

        [Required(ErrorMessage = "Tanggal selesai wajib diisi")]
        public DateTime TglSelesai { get; set; }

        // Validate that end date is not before start date
        public bool IsValidDateRange()
        {
            return TglSelesai >= TglMulai;
        }
    }
}
