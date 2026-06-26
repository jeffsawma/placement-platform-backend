using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

/// <summary>
/// Configuration Table-Per-Hierarchy pour Offre / OffreEmploi / OffreStage.
/// Toutes les sous-classes partagent la table offres ; le discriminant est la
/// colonne type_offre.
/// </summary>
public class OffreConfiguration : IEntityTypeConfiguration<Offre>
{
    public void Configure(EntityTypeBuilder<Offre> builder)
    {
        builder.ToTable("offres");

        builder.HasKey(o => o.IdOffre);

        builder.Property(o => o.IdOffre)
            .HasColumnName("id_offre")
            .ValueGeneratedOnAdd();

        builder.Property(o => o.Titre)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("titre");

        builder.Property(o => o.Description)
            .IsRequired()
            .HasColumnType("longtext")
            .HasColumnName("description");

        builder.Property(o => o.Ville)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("ville");

        builder.Property(o => o.Adresse)
            .HasMaxLength(255)
            .HasColumnName("adresse");

        builder.Property(o => o.TypeOffre)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>()
            .HasColumnName("type_offre");

        builder.Property(o => o.Statut)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>()
            .HasDefaultValue(StatutOffre.Active)
            .HasColumnName("statut");

        builder.Property(o => o.DatePublication)
            .HasColumnName("date_publication");

        builder.Property(o => o.DateExpiration)
            .HasColumnName("date_expiration");

        builder.Property(o => o.IdEmployeur)
            .HasColumnName("id_employeur");

        builder.HasOne(o => o.Employeur)
            .WithMany()
            .HasForeignKey(o => o.IdEmployeur)
            .OnDelete(DeleteBehavior.Restrict);

        // TPH discriminant = colonne type_offre.
        // Offre est abstraite, donc seules les classes concretes ont une valeur.
        builder.HasDiscriminator(o => o.TypeOffre)
            .HasValue<OffreEmploi>(TypeOffre.Emploi)
            .HasValue<OffreStage>(TypeOffre.Stage);

        builder.HasIndex(o => o.IdEmployeur).HasDatabaseName("idx_offre_employeur");
        builder.HasIndex(o => o.Statut).HasDatabaseName("idx_offre_statut");
    }
}

public class OffreEmploiConfiguration : IEntityTypeConfiguration<OffreEmploi>
{
    public void Configure(EntityTypeBuilder<OffreEmploi> builder)
    {
        builder.Property(e => e.TypeContrat)
            .HasMaxLength(50)
            .HasColumnName("type_contrat");

        builder.Property(e => e.SalaireMin)
            .HasPrecision(10, 2)
            .HasColumnName("salaire_min");

        builder.Property(e => e.SalaireMax)
            .HasPrecision(10, 2)
            .HasColumnName("salaire_max");

        builder.Property(e => e.TeleTravail)
            .HasMaxLength(50)
            .HasColumnName("tele_travail");
    }
}

public class OffreStageConfiguration : IEntityTypeConfiguration<OffreStage>
{
    public void Configure(EntityTypeBuilder<OffreStage> builder)
    {
        builder.Property(s => s.DateDebutStage)
            .HasColumnName("date_debut_stage");

        builder.Property(s => s.DateFinStage)
            .HasColumnName("date_fin_stage");

        builder.Property(s => s.DureeHeuresParSemaine)
            .HasColumnName("duree_heures_semaine");

        builder.Property(s => s.Remuneration)
            .HasPrecision(10, 2)
            .HasColumnName("remuneration");

        builder.Property(s => s.Session)
            .HasMaxLength(50)
            .HasColumnName("session");
    }
}
