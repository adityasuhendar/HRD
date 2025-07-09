// HRD.API/Models/DTOs/Request/PresensiRequest.cs
using System.ComponentModel.DataAnnotations;

namespace HRD.API.Models.DTOs.Request
{
    public class ClockInRequest
    {
        public string? Catatan { get; set; }

        [StringLength(50)]
        public string? DeviceId { get; set; }

        public string MetodePresensi { get; set; } = "Web";
    }

    public class ClockOutRequest
    {
        public string? Catatan { get; set; }
    }

    public class ManualPresensiRequest
    {
        [Required(ErrorMessage = "ID Karyawan wajib diisi")]
        public int IdKaryawan { get; set; }

        [Required(ErrorMessage = "Tanggal wajib diisi")]
        public DateTime Tanggal { get; set; }

        [Required(ErrorMessage = "Jam masuk wajib diisi")]
        public DateTime JamMasuk { get; set; }

        public DateTime? JamKeluar { get; set; }

        [StringLength(20)]
        public string StatusPresensi { get; set; } = "Hadir";

        [StringLength(255)]
        public string? Catatan { get; set; }
    }
}