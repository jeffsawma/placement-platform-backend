using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class OffreConfiguration : IEntityTypeConfiguration<Offre>
{
    public void Configure(EntityTypeBuilder<Offre> builder)
    {
        builder.ToTable("OFFRE");

        builder.HasKey(o => o.IdOffre);

        builder.Property(o => o.IdOffre)
            .HasColumnName("id_offre");

        builder.Property(o => o.Titre)
            .HasColumnName("titre")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.Description)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(o => o.Lieu)
            .HasColumnName("lieu")
            .HasMaxLength(150);

        builder.Property(o => o.DatePublication)
            .HasColumnName("date_publication")
            .IsRequired();

        builder.Property(o => o.DateExpiration)
            .HasColumnName("date_expiration");

        builder.Property(o => o.Statut)
            .HasColumnName("statut")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.TypeOffre)
            .HasColumnName("type_offre")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.IdEntreprise)
            .HasColumnName("id_entreprise")
            .IsRequired();

        builder.Property(o => o.NombrePostes)
            .HasColumnName("nombre_postes");

        builder.Property(o => o.Remunere)
            .HasColumnName("remunere");
    }
}
