// HRD.API/Data/Configurations/PenggunaConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRD.API.Models.Entities;

namespace HRD.API.Data.Configurations
{
    public class PenggunaConfiguration : IEntityTypeConfiguration<Pengguna>
    {
        public void Configure(EntityTypeBuilder<Pengguna> builder)
        {
            // Unique constraints
            builder.HasIndex(e => e.Username).IsUnique();
            builder.HasIndex(e => e.Email).IsUnique();

            // Check constraints (will be applied in migration)
            builder.Property(e => e.Peran)
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }
}
