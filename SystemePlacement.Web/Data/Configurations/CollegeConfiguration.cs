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

        builder.Property(c => c.CouleurPrimaire)
            .HasColumnName("couleur_primaire")
            .HasMaxLength(20)
            .HasDefaultValue("#009fda")
            .IsRequired();

        builder.Property(c => c.CouleurPrimaireFoncee)
            .HasColumnName("couleur_primaire_foncee")
            .HasMaxLength(20)
            .HasDefaultValue("#003f7d")
            .IsRequired();

        builder.Property(c => c.CouleurSecondaire)
            .HasColumnName("couleur_secondaire")
            .HasMaxLength(20)
            .HasDefaultValue("#0053a1")
            .IsRequired();

        builder.Property(c => c.CouleurAccent)
            .HasColumnName("couleur_accent")
            .HasMaxLength(20)
            .HasDefaultValue("#69be28")
            .IsRequired();

        builder.Property(c => c.CouleurFond)
            .HasColumnName("couleur_fond")
            .HasMaxLength(20)
            .HasDefaultValue("#f4f7fb")
            .IsRequired();

        builder.Property(c => c.CouleurTexte)
            .HasColumnName("couleur_texte")
            .HasMaxLength(20)
            .HasDefaultValue("#172033")
            .IsRequired();

        builder.Property(c => c.LogoUrl)
            .HasColumnName("logo_url")
            .HasMaxLength(500);

        builder.HasIndex(c => c.Nom)
            .IsUnique();
    }
}