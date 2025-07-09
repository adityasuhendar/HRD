// HRD.API/Models/Entities/Gaji.cs
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRD.API.Models.Entities
{
    [Table("tb_gaji")]
    public class Gaji
    {
        [Key]
        [Column("id_gaji")]
        public int IdGaji { get; set; }

        [Column("id_karyawan")]
        public int IdKaryawan { get; set; }

        [Column("bulan")]
        public int Bulan { get; set; }

        [Column("tahun")]
        public int Tahun { get; set; }

        [Column("gaji_pokok")]
        [Precision(18, 2)]
        public decimal GajiPokok { get; set; }

        [Column("tunjangan")]
        [Precision(18, 2)]
        public decimal Tunjangan { get; set; }

        [Column("potongan")]
        [Precision(18, 2)]
        public decimal Potongan { get; set; }

        [Column("total_jam_kerja")]
        public int TotalJamKerja { get; set; }

        [Column("total_gaji")]
        [Precision(18, 2)]
        public decimal TotalGaji { get; set; }

        [Column("status_bayar")]
        [StringLength(20)]
        public string StatusBayar { get; set; } = "Belum Bayar";

        [Column("tgl_dibuat")]
        public DateTime TglDibuat { get; set; }

        [Column("keterangan")]
        [StringLength(500)]
        public string? Keterangan { get; set; }

        // Navigation property
        public virtual Karyawan Karyawan { get; set; } = null!;
    }
}