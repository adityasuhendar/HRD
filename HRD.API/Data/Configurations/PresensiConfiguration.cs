// HRD.API/Data/Configurations/PresensiConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRD.API.Models.Entities;

namespace HRD.API.Data.Configurations
{
    public class PresensiConfiguration : IEntityTypeConfiguration<Presensi>
    {
        public void Configure(EntityTypeBuilder<Presensi> builder)
        {
            // Indexes for performance
            builder.HasIndex(e => new { e.IdKaryawan, e.Tanggal })
                .IsUnique(); // One attendance record per employee per day

            builder.HasIndex(e => e.Tanggal);
            builder.HasIndex(e => e.MetodePresensi);

            // Default values
            builder.Property(e => e.TotalJam).HasDefaultValue(0);
            builder.Property(e => e.StatusPresensi).HasDefaultValue("Hadir");
            builder.Property(e => e.MetodePresensi).HasDefaultValue("Web");
        }
    }
}
