// HRD.API/Models/Entities/Cuti.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRD.API.Models.Entities
{
    [Table("tb_cuti")]
    public class Cuti
    {
        [Key]
        [Column("id_cuti")]
        public int IdCuti { get; set; }

        [ForeignKey("Karyawan")]
        [Column("id_karyawan")]
        public int IdKaryawan { get; set; }

        [Required]
        [StringLength(30)]
        [Column("jenis_cuti")]
        public string JenisCuti { get; set; } = string.Empty;

        [Column("tgl_mulai")]
        public DateTime TglMulai { get; set; }

        [Column("tgl_selesai")]
        public DateTime TglSelesai { get; set; }

        [Column("jumlah_hari")]
        public int JumlahHari { get; set; }

        [Column("alasan")]
        public string? Alasan { get; set; }

        [StringLength(20)]
        [Column("status_persetujuan")]
        public string StatusPersetujuan { get; set; } = "Pending";

        [Column("catatan_hrd")]
        public string? CatatanHRD { get; set; }

        [Column("tgl_dibuat")]
        public DateTime TglDibuat { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual Karyawan Karyawan { get; set; } = null!;
    }
}