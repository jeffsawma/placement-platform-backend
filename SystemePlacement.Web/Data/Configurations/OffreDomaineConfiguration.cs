using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class OffreDomaineConfiguration : IEntityTypeConfiguration<OffreDomaine>
{
    public void Configure(EntityTypeBuilder<OffreDomaine> builder)
    {
        builder.ToTable("OFFRE_DOMAINE");

        builder.HasKey(od => new { od.IdOffre, od.IdDomaine });

        builder.Property(od => od.IdOffre).HasColumnName("id_offre");
        builder.Property(od => od.IdDomaine).HasColumnName("id_domaine");

        builder.HasOne(od => od.Offre)
            .WithMany(o => o.OffreDomaines)
            .HasForeignKey(od => od.IdOffre)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(od => od.DomaineEtude)
            .WithMany()
            .HasForeignKey(od => od.IdDomaine)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
