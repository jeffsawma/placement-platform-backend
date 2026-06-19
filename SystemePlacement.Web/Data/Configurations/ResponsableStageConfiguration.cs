using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class ResponsableStageConfiguration : IEntityTypeConfiguration<ResponsableStage>
{
    public void Configure(EntityTypeBuilder<ResponsableStage> builder)
    {
        // Profil minimal pour un responsable de stage
        builder.ToTable("responsables_stage");

        builder.HasKey(r => r.IdResponsable);

        builder.Property(r => r.IdResponsable)
            .HasColumnName("id_responsable");

        builder.Property(r => r.IdUtilisateur)
            .HasColumnName("id_utilisateur");

        // Un utilisateur peut avoir un seul profil responsable de stage
        builder.HasOne(r => r.Utilisateur)
            .WithOne(u => u.ResponsableStage)
            .HasForeignKey<ResponsableStage>(r => r.IdUtilisateur)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.IdUtilisateur)
            .IsUnique();
    }
}