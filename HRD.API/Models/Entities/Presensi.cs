// HRD.API/Models/Entities/Presensi.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRD.API.Models.Entities
{
    [Table("tb_presensi")]
    public class Presensi
    {
        [Key]
        [Column("id_presensi")]
        public int IdPresensi { get; set; }

        [ForeignKey("Karyawan")]
        [Column("id_karyawan")]
        public int IdKaryawan { get; set; }

        [Column("tanggal")]
        public DateTime Tanggal { get; set; }

        [Column("jam_masuk")]
        public DateTime? JamMasuk { get; set; }

        [Column("jam_keluar")]
        public DateTime? JamKeluar { get; set; }

        [Column("total_jam", TypeName = "decimal(4,2)")]
        public decimal TotalJam { get; set; } = 0;

        [StringLength(20)]
        [Column("status_presensi")]
        public string StatusPresensi { get; set; } = "Hadir";

        [StringLength(20)]
        [Column("metode_presensi")]
        public string MetodePresensi { get; set; } = "Web";

        [StringLength(50)]
        [Column("device_id")]
        public string? DeviceId { get; set; }

        [StringLength(255)]
        [Column("catatan")]
        public string? Catatan { get; set; }

        [Column("tgl_dibuat")]
        public DateTime TglDibuat { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual Karyawan Karyawan { get; set; } = null!;
    }
}