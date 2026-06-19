using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class EtudiantConfiguration : IEntityTypeConfiguration<Etudiant>
{
    public void Configure(EntityTypeBuilder<Etudiant> builder)
    {
        // Profil minimal pour un utilisateur etudiant
        builder.ToTable("etudiants");

        builder.HasKey(e => e.IdEtudiant);

        builder.Property(e => e.IdEtudiant)
            .HasColumnName("id_etudiant");

        builder.Property(e => e.IdUtilisateur)
            .HasColumnName("id_utilisateur");

        // Un utilisateur peut avoir un seul profil etudiant
        builder.HasOne(e => e.Utilisateur)
            .WithOne(u => u.Etudiant)
            .HasForeignKey<Etudiant>(e => e.IdUtilisateur)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.IdUtilisateur)
            .IsUnique();
    }
}