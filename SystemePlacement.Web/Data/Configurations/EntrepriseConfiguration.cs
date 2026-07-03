using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class EntrepriseConfiguration : IEntityTypeConfiguration<Entreprise>
{
    public void Configure(EntityTypeBuilder<Entreprise> builder)
    {
        // Profil entreprise lié à un employeur
        builder.ToTable("entreprises");

        builder.HasKey(e => e.IdEntreprise);

        builder.Property(e => e.IdEntreprise)
            .HasColumnName("id_entreprise");

        builder.Property(e => e.IdEmployeur)
            .HasColumnName("id_employeur")
            .IsRequired();

        builder.Property(e => e.Nom)
            .HasColumnName("nom")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.Secteur)
            .HasColumnName("secteur")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Adresse)
            .HasColumnName("adresse")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.SiteWeb)
            .HasColumnName("site_web")
            .HasMaxLength(255);

        builder.Property(e => e.Description)
            .HasColumnName("description")
            .HasColumnType("TEXT");

        builder.Property(e => e.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(255);

        // Une entreprise peut avoir un seul profil d'employeur
        builder.HasOne(e => e.Employeur)
            .WithOne(e => e.Entreprise)
            .HasForeignKey<Entreprise>(e => e.IdEmployeur)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.IdEmployeur)
            .IsUnique();
    }
}
