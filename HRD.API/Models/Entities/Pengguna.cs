// HRD.API/Models/Entities/Pengguna.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRD.API.Models.Entities
{
    [Table("tb_pengguna")]
    public class Pengguna
    {
        [Key]
        [Column("id_pengguna")]
        public int IdPengguna { get; set; }

        [Required]
        [StringLength(50)]
        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("peran")]
        public string Peran { get; set; } = string.Empty; // "HRD" atau "Karyawan"

        [Column("aktif")]
        public bool Aktif { get; set; } = true;

        [Column("tgl_dibuat")]
        public DateTime TglDibuat { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual Karyawan? Karyawan { get; set; }
    }
}