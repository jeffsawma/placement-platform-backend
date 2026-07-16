using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class CandidatureDocumentConfiguration : IEntityTypeConfiguration<CandidatureDocument>
{
    public void Configure(EntityTypeBuilder<CandidatureDocument> builder)
    {
        builder.ToTable("candidature_documents");

        builder.HasKey(d => d.IdDocument);

        builder.Property(d => d.IdDocument)
            .HasColumnName("id_document");

        builder.Property(d => d.IdCandidature)
            .HasColumnName("id_candidature")
            .IsRequired();

        builder.Property(d => d.TypeDocument)
            .HasColumnName("type_document")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(d => d.NomFichier)
            .HasColumnName("nom_fichier")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.CheminFichier)
            .HasColumnName("chemin_fichier")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(d => d.ContentType)
            .HasColumnName("content_type")
            .HasMaxLength(100);

        builder.Property(d => d.TailleFichier)
            .HasColumnName("taille_fichier")
            .IsRequired();

        builder.Property(d => d.DateUpload)
            .HasColumnName("date_upload")
            .IsRequired();

        builder.HasOne(d => d.Candidature)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.IdCandidature)
            .OnDelete(DeleteBehavior.Cascade);
    }
}