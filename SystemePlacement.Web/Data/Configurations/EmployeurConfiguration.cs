using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class EmployeurConfiguration : IEntityTypeConfiguration<Employeur>
{
    public void Configure(EntityTypeBuilder<Employeur> builder)
    {
        // Profil employeur lié à un utilisateur modifié
        builder.ToTable("employeurs");

        builder.HasKey(e => e.IdEmployeur);

        builder.Property(e => e.IdEmployeur)
            .HasColumnName("id_employeur");

        builder.Property(e => e.IdUtilisateur)
            .HasColumnName("id_utilisateur")
            .IsRequired();

        builder.Property(e => e.Poste)
            .HasColumnName("poste")
            .HasMaxLength(100)
            .HasDefaultValue("")
            .IsRequired();

        builder.Property(e => e.Telephone)
            .HasColumnName("telephone")
            .HasMaxLength(20)
            .HasDefaultValue("")
            .IsRequired();

        builder.Property(e => e.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(255);

        builder.Property(e => e.Titre)
            .HasColumnName("titre")
            .HasMaxLength(150)
            .HasDefaultValue("")
            .IsRequired();

        // Un utilisateur peut avoir un seul profil employeur
        builder.HasOne(e => e.Utilisateur)
            .WithOne(u => u.Employeur)
            .HasForeignKey<Employeur>(e => e.IdUtilisateur)
            .OnDelete(DeleteBehavior.Cascade);

        // Un employeur peut avoir un seul profil d'entreprise
        builder.HasOne(e => e.Entreprise)
            .WithOne(e => e.Employeur)
            .HasForeignKey<Entreprise>(e => e.IdEmployeur)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.IdUtilisateur)
            .IsUnique();
    }
}
