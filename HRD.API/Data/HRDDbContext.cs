// HRD.API/Data/HRDDbContext.cs
using Microsoft.EntityFrameworkCore;
using HRD.API.Models.Entities;

namespace HRD.API.Data
{
    public class HRDDbContext : DbContext
    {
        public HRDDbContext(DbContextOptions<HRDDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Pengguna> Pengguna { get; set; }
        public DbSet<Karyawan> Karyawan { get; set; }
        public DbSet<Cuti> Cuti { get; set; }
        public DbSet<Presensi> Presensi { get; set; }
        public DbSet<Gaji> Gaji { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HRDDbContext).Assembly);

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed default HRD user
            modelBuilder.Entity<Pengguna>().HasData(
                new Pengguna
                {
                    IdPengguna = 1,
                    Username = "admin",
                    Email = "hrd@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // Hash password
                    Peran = "HRD",
                    Aktif = true,
                    TglDibuat = DateTime.Now
                }
            );

            // Seed sample employee user
            modelBuilder.Entity<Pengguna>().HasData(
                new Pengguna
                {
                    IdPengguna = 2,
                    Username = "john.doe",
                    Email = "john@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Peran = "Karyawan",
                    Aktif = true,
                    TglDibuat = DateTime.Now
                }
            );

            // Seed sample employee
            modelBuilder.Entity<Karyawan>().HasData(
                new Karyawan
                {
                    IdKaryawan = 1,
                    IdPengguna = 2,
                    NIK = "EMP001",
                    NamaLengkap = "John Doe",
                    EmailKantor = "john@company.com",
                    NoTelepon = "081234567890",
                    Posisi = "Software Developer",
                    Divisi = "Development",
                    TglBergabung = new DateTime(2024, 1, 15),
                    GajiPokok = 8000000,
                    StatusKaryawan = "Aktif",
                    TglDibuat = DateTime.Now
                }
            );
            modelBuilder.Entity<Gaji>(entity =>
            {
                entity.HasKey(e => e.IdGaji);

                entity.HasOne(e => e.Karyawan)
                      .WithMany()
                      .HasForeignKey(e => e.IdKaryawan)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => new { e.IdKaryawan, e.Bulan, e.Tahun })
                      .IsUnique()
                      .HasDatabaseName("IX_Gaji_Karyawan_Bulan_Tahun");
            });
        }
    }
}
