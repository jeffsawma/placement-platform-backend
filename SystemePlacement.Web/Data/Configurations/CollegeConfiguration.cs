using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class CollegeConfiguration : IEntityTypeConfiguration<College>
{
    public void Configure(EntityTypeBuilder<College> builder)
    {
        builder.ToTable("colleges");

        builder.HasKey(c => c.IdCollege);

        builder.Property(c => c.IdCollege)
            .HasColumnName("id_college");

        builder.Property(c => c.Nom)
            .HasColumnName("nom")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Ville)
            .HasColumnName("ville")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Actif)
            .HasColumnName("actif")
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasIndex(c => c.Nom)
            .IsUnique();
    }
}
