// HRD.API/Data/Configurations/KaryawanConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRD.API.Models.Entities;

namespace HRD.API.Data.Configurations
{
    public class KaryawanConfiguration : IEntityTypeConfiguration<Karyawan>
    {
        public void Configure(EntityTypeBuilder<Karyawan> builder)
        {
            // Unique constraints
            builder.HasIndex(e => e.NIK).IsUnique();
            builder.HasIndex(e => e.EmailKantor).IsUnique();

            // Relationships
            builder.HasOne(e => e.Pengguna)
                .WithOne(p => p.Karyawan)
                .HasForeignKey<Karyawan>(e => e.IdPengguna)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.DaftarCuti)
                .WithOne(c => c.Karyawan)
                .HasForeignKey(c => c.IdKaryawan)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.DaftarPresensi)
                .WithOne(p => p.Karyawan)
                .HasForeignKey(p => p.IdKaryawan)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.DaftarGaji)
                .WithOne(g => g.Karyawan)
                .HasForeignKey(g => g.IdKaryawan)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}