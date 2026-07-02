using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class DemandeStageConfiguration : IEntityTypeConfiguration<DemandeStage>
{
    public void Configure(EntityTypeBuilder<DemandeStage> builder)
    {
        builder.ToTable("demandes_stage");

        builder.HasKey(d => d.IdDemandeStage);

        builder.Property(d => d.IdDemandeStage)
            .HasColumnName("id_demande_stage");

        builder.Property(d => d.IdEtudiant)
            .HasColumnName("id_etudiant")
            .IsRequired();

        builder.Property(d => d.IdDomaine)
            .HasColumnName("id_domaine")
            .IsRequired();

        builder.Property(d => d.Description)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(d => d.PeriodeSouhaitee)
            .HasColumnName("periode_souhaitee")
            .HasMaxLength(200);

        builder.Property(d => d.Competences)
            .HasColumnName("competences");

        builder.Property(d => d.Statut)
            .HasColumnName("statut")
            .HasConversion<string>()
            .HasMaxLength(30)
            .HasDefaultValue(StatutDemandeStage.Ouverte)
            .IsRequired();

        builder.Property(d => d.DateCreation)
            .HasColumnName("date_creation")
            .IsRequired();

        builder.HasOne(d => d.Etudiant)
            .WithMany()
            .HasForeignKey(d => d.IdEtudiant)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.DomaineEtude)
            .WithMany()
            .HasForeignKey(d => d.IdDomaine)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
