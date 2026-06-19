using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class AdministrateurConfiguration : IEntityTypeConfiguration<Administrateur>
{
    public void Configure(EntityTypeBuilder<Administrateur> builder)
    {
        // Profil supplementaire pour un utilisateur administrateur
        builder.ToTable("administrateurs");

        builder.HasKey(a => a.IdAdministrateur);

        builder.Property(a => a.IdAdministrateur)
            .HasColumnName("id_administrateur");

        builder.Property(a => a.NiveauAcces)
            .HasColumnName("niveau_acces")
            .HasMaxLength(50)
            .HasDefaultValue("Standard");

        builder.Property(a => a.IdUtilisateur)
            .HasColumnName("id_utilisateur");

        // Un utilisateur peut avoir un seul profil administrateur
        builder.HasOne(a => a.Utilisateur)
            .WithOne(u => u.Administrateur)
            .HasForeignKey<Administrateur>(a => a.IdUtilisateur)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.IdUtilisateur)
            .IsUnique();
    }
}