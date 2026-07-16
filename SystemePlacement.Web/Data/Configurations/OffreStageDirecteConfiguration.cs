using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class OffreStageDirecteConfiguration : IEntityTypeConfiguration<OffreStageDirecte>
{
    public void Configure(EntityTypeBuilder<OffreStageDirecte> builder)
    {
        builder.ToTable("offres_stage_directes");

        builder.HasKey(o => o.IdOffreDirecte);

        builder.Property(o => o.IdOffreDirecte)
            .HasColumnName("id_offre_directe");

        builder.Property(o => o.IdEtudiant)
            .HasColumnName("id_etudiant")
            .IsRequired();

        builder.Property(o => o.IdEmployeur)
            .HasColumnName("id_employeur")
            .IsRequired();

        builder.Property(o => o.IdOffreStage)
            .HasColumnName("id_offre_stage");

        builder.Property(o => o.IdCandidature)
            .HasColumnName("id_candidature");

        builder.Property(o => o.IdDemandeStage)
            .HasColumnName("id_demande_stage");

        builder.Property(o => o.Conditions)
            .HasColumnName("conditions")
            .HasColumnType("TEXT")
            .IsRequired();

        builder.Property(o => o.DateDebutProposee)
            .HasColumnName("date_debut_proposee");

        builder.Property(o => o.DateFinProposee)
            .HasColumnName("date_fin_proposee");

        builder.Property(o => o.DateProposition)
            .HasColumnName("date_proposition")
            .IsRequired();

        builder.Property(o => o.Statut)
            .HasColumnName("statut")
            .HasConversion<string>()
            .HasMaxLength(30)
            .HasDefaultValue(StatutOffreStageDirecte.Envoyee)
            .IsRequired();

        builder.Property(o => o.ReponseEtudiant)
            .HasColumnName("reponse_etudiant");

        builder.Property(o => o.Commentaire)
            .HasColumnName("commentaire");

        builder.HasOne(o => o.Etudiant)
            .WithMany()
            .HasForeignKey(o => o.IdEtudiant)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Employeur)
            .WithMany()
            .HasForeignKey(o => o.IdEmployeur)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.OffreStage)
            .WithMany()
            .HasForeignKey(o => o.IdOffreStage)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.Candidature)
            .WithMany()
            .HasForeignKey(o => o.IdCandidature)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.DemandeStage)
            .WithMany()
            .HasForeignKey(o => o.IdDemandeStage)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(o => o.IdEtudiant);
        builder.HasIndex(o => o.IdEmployeur);
        builder.HasIndex(o => o.Statut);
    }
}
