// HRD.API/Data/Configurations/GajiConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRD.API.Models.Entities;

namespace HRD.API.Data.Configurations
{
    public class GajiConfiguration : IEntityTypeConfiguration<Gaji>
    {
        public void Configure(EntityTypeBuilder<Gaji> builder)
        {
            // Indexes for performance
            builder.HasIndex(e => new { e.IdKaryawan, e.Bulan, e.Tahun })
                .IsUnique(); // One payroll per employee per month

            builder.HasIndex(e => new { e.Bulan, e.Tahun });
            builder.HasIndex(e => e.StatusBayar);

            // Default values
            builder.Property(e => e.Tunjangan).HasDefaultValue(0);
            builder.Property(e => e.Potongan).HasDefaultValue(0);
            builder.Property(e => e.TotalJamKerja).HasDefaultValue(0);
            builder.Property(e => e.StatusBayar).HasDefaultValue("Belum Dibayar");
        }
    }
}
