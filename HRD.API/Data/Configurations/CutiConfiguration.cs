// HRD.API/Data/Configurations/CutiConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRD.API.Models.Entities;

namespace HRD.API.Data.Configurations
{
    public class CutiConfiguration : IEntityTypeConfiguration<Cuti>
    {
        public void Configure(EntityTypeBuilder<Cuti> builder)
        {
            // Indexes for performance
            builder.HasIndex(e => e.IdKaryawan);
            builder.HasIndex(e => e.StatusPersetujuan);
            builder.HasIndex(e => e.TglMulai);

            // Computed column for JumlahHari (optional)
            // builder.Property(e => e.JumlahHari)
            //     .HasComputedColumnSql("DATEDIFF(day, [tgl_mulai], [tgl_selesai]) + 1");
        }
    }
}