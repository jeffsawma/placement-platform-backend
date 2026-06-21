using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class CandidatureConfiguration : IEntityTypeConfiguration<Candidature>
{
    public void Configure(EntityTypeBuilder<Candidature> builder)
    {
        builder.ToTable("CANDIDATURE");

        builder.HasKey(c => c.IdCandidature);

        builder.Property(c => c.IdCandidature)
            .HasColumnName("id_candidature");

        builder.Property(c => c.IdOffre)
            .HasColumnName("id_offre")
            .IsRequired();

        builder.Property(c => c.IdEtudiant)
            .HasColumnName("id_etudiant")
            .IsRequired();

        builder.Property(c => c.DateCandidature)
            .HasColumnName("date_candidature")
            .IsRequired();

        builder.Property(c => c.Statut)
            .HasColumnName("statut")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.CvUrl)
            .HasColumnName("cv_url")
            .HasMaxLength(255);

        builder.Property(c => c.LettreMotivation)
            .HasColumnName("lettre_motivation");

        builder.HasIndex(c => new { c.IdOffre, c.IdEtudiant })
            .IsUnique();

        builder.HasOne(c => c.Etudiant)
            .WithMany()
            .HasForeignKey(c => c.IdEtudiant)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
