// HRD.API/Models/Entities/Karyawan.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRD.API.Models.Entities
{
    [Table("tb_karyawan")]
    public class Karyawan
    {
        [Key]
        [Column("id_karyawan")]
        public int IdKaryawan { get; set; }

        [ForeignKey("Pengguna")]
        [Column("id_pengguna")]
        public int IdPengguna { get; set; }

        [Required]
        [StringLength(20)]
        [Column("nik")]
        public string NIK { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("nama_lengkap")]
        public string NamaLengkap { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("email_kantor")]
        public string EmailKantor { get; set; } = string.Empty;

        [StringLength(20)]
        [Column("no_telepon")]
        public string? NoTelepon { get; set; }

        [Required]
        [StringLength(50)]
        [Column("posisi")]
        public string Posisi { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("divisi")]
        public string Divisi { get; set; } = string.Empty;

        [Column("tgl_bergabung")]
        public DateTime TglBergabung { get; set; }

        [Column("gaji_pokok", TypeName = "decimal(12,2)")]
        public decimal GajiPokok { get; set; }

        [StringLength(20)]
        [Column("status_karyawan")]
        public string StatusKaryawan { get; set; } = "Aktif";

        [Column("tgl_dibuat")]
        public DateTime TglDibuat { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual Pengguna Pengguna { get; set; } = null!;
        public virtual ICollection<Cuti> DaftarCuti { get; set; } = new List<Cuti>();
        public virtual ICollection<Presensi> DaftarPresensi { get; set; } = new List<Presensi>();
        public virtual ICollection<Gaji> DaftarGaji { get; set; } = new List<Gaji>();
    }
}