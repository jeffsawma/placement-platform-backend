using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class CandidatureConfiguration : IEntityTypeConfiguration<Candidature>
{
    public void Configure(EntityTypeBuilder<Candidature> builder)
    {
        builder.ToTable("CANDIDATURE");

        builder.HasKey(c => c.IdCandidature);

        builder.Property(c => c.IdCandidature)
            .HasColumnName("id_candidature")
            .UseIdentityColumn();

        builder.Property(c => c.IdOffre)
            .HasColumnName("id_offre");

        builder.Property(c => c.IdEtudiant)
            .HasColumnName("id_etudiant");

        builder.Property(c => c.Statut)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion<string>()
            .HasDefaultValue(StatutCandidature.EnAttente)
            .HasColumnName("statut");

        builder.Property(c => c.DateCandidature)
            .HasColumnName("date_candidature");

        builder.Property(c => c.MessageMotivation)
            .HasColumnType("longtext")
            .HasColumnName("message_motivation");

        builder.HasOne(c => c.Offre)
            .WithMany(o => o.Candidatures)
            .HasForeignKey(c => c.IdOffre)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Etudiant)
            .WithMany()
            .HasForeignKey(c => c.IdEtudiant)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.IdOffre).HasDatabaseName("idx_candidature_offre");
        builder.HasIndex(c => c.IdEtudiant).HasDatabaseName("idx_candidature_etudiant");
        builder.HasIndex(c => new { c.IdOffre, c.IdEtudiant }).IsUnique();
    }
}

public class CandidatureDocumentConfiguration : IEntityTypeConfiguration<CandidatureDocument>
{
    public void Configure(EntityTypeBuilder<CandidatureDocument> builder)
    {
        builder.ToTable("CANDIDATURE_DOCUMENT");

        builder.HasKey(d => d.IdDocument);

        builder.Property(d => d.IdDocument)
            .HasColumnName("id_document")
            .UseIdentityColumn();

        builder.Property(d => d.IdCandidature)
            .HasColumnName("id_candidature");

        builder.Property(d => d.TypeDocument)
            .IsRequired()
            .HasMaxLength(30)
            .HasConversion<string>()
            .HasColumnName("type_document");

        builder.Property(d => d.NomFichier)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("nom_fichier");

        builder.Property(d => d.CheminFichier)
            .IsRequired()
            .HasMaxLength(500)
            .HasColumnName("chemin_fichier");

        builder.Property(d => d.ContentType)
            .HasMaxLength(100)
            .HasColumnName("content_type");

        builder.Property(d => d.TailleFichier)
            .HasColumnName("taille_fichier");

        builder.Property(d => d.DateUpload)
            .HasColumnName("date_upload");

        builder.HasOne(d => d.Candidature)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.IdCandidature)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
