using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class DomaineEtudeConfiguration : IEntityTypeConfiguration<DomaineEtude>
{
    public void Configure(EntityTypeBuilder<DomaineEtude> builder)
    {
        builder.ToTable("domaine_etudes");

        builder.HasKey(d => d.IdDomaine);

        builder.Property(d => d.IdDomaine)
            .HasColumnName("id_domaine");

        builder.Property(d => d.Nom)
            .HasColumnName("nom")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(d => d.Code)
            .HasColumnName("code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(d => d.Actif)
            .HasColumnName("actif")
            .HasDefaultValue(true);

        // Un seul domaine global avec ce nom.
        builder.HasIndex(d => d.Nom)
            .IsUnique();

        // Un seul code global pour eviter les doublons comme INFO deux fois.
        builder.HasIndex(d => d.Code)
            .IsUnique();
    }
}