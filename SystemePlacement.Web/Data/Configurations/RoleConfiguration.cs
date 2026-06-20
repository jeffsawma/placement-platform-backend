using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Nom de la table dans MySQL
        builder.ToTable("roles");

        // Cle primaire
        builder.HasKey(r => r.IdRole);

        builder.Property(r => r.IdRole)
            .HasColumnName("id_role");

        builder.Property(r => r.NomRole)
            .HasColumnName("nom_role")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(255);

        builder.Property(r => r.Actif)
            .HasColumnName("actif")
            .HasDefaultValue(true);

        // Un nom de role doit etre unique
        builder.HasIndex(r => r.NomRole)
            .IsUnique();
    }
}