using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class EmployeurConfiguration : IEntityTypeConfiguration<Employeur>
{
    public void Configure(EntityTypeBuilder<Employeur> builder)
    {
        // Profil minimal pour un utilisateur employeur
        builder.ToTable("employeurs");

        builder.HasKey(e => e.IdEmployeur);

        builder.Property(e => e.IdEmployeur)
            .HasColumnName("id_employeur");

        builder.Property(e => e.IdUtilisateur)
            .HasColumnName("id_utilisateur");

        // Un utilisateur peut avoir un seul profil employeur
        builder.HasOne(e => e.Utilisateur)
            .WithOne(u => u.Employeur)
            .HasForeignKey<Employeur>(e => e.IdUtilisateur)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.IdUtilisateur)
            .IsUnique();
    }
}