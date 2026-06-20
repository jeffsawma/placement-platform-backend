using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class CollegeConfiguration : IEntityTypeConfiguration<College>
{
    public void Configure(EntityTypeBuilder<College> builder)
    {
        builder.ToTable("COLLEGE"); // Table name

        builder.HasKey(c => c.IdCollege); // PK

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
            .IsRequired();
    }
}
