using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class RecommandationConfiguration : IEntityTypeConfiguration<Recommandation>
{
    public void Configure(EntityTypeBuilder<Recommandation> builder)
    {
        builder.ToTable("RECOMMANDATION");

        builder.HasKey(r => r.IdRecommandation);

        builder.Property(r => r.IdRecommandation)
            .HasColumnName("id_recommandation")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.IdEtudiant)
            .HasColumnName("id_etudiant");

        builder.Property(r => r.IdAuteur)
            .HasColumnName("id_auteur");

        builder.Property(r => r.Commentaire)
            .IsRequired()
            .HasMaxLength(2000)
            .HasColumnName("commentaire");

        builder.Property(r => r.CheminLettreRecommandation)
            .HasMaxLength(500)
            .HasColumnName("chemin_lettre");

        builder.Property(r => r.NomFichierLettre)
            .HasMaxLength(255)
            .HasColumnName("nom_fichier_lettre");

        builder.Property(r => r.ContentTypeLettre)
            .HasMaxLength(100)
            .HasColumnName("content_type_lettre");

        builder.Property(r => r.DateCreation)
            .HasColumnName("date_creation");

        builder.HasOne(r => r.Etudiant)
            .WithMany()
            .HasForeignKey(r => r.IdEtudiant)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Auteur)
            .WithMany()
            .HasForeignKey(r => r.IdAuteur)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.IdEtudiant)
            .HasDatabaseName("idx_recommandation_etudiant");
    }
}
