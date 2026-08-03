using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class CollegeDomaineConfiguration : IEntityTypeConfiguration<CollegeDomaine>
{
    public void Configure(EntityTypeBuilder<CollegeDomaine> builder)
    {
        builder.ToTable("college_domaines");

        builder.HasKey(cd => cd.IdCollegeDomaine);

        builder.Property(cd => cd.IdCollegeDomaine)
            .HasColumnName("id_college_domaine");

        builder.Property(cd => cd.IdCollege)
            .HasColumnName("id_college");

        builder.Property(cd => cd.IdDomaine)
            .HasColumnName("id_domaine");

        builder.Property(cd => cd.AccepteStagiaires)
            .HasColumnName("accepte_stagiaires")
            .HasDefaultValue(true);

        builder.Property(cd => cd.Actif)
            .HasColumnName("actif")
            .HasDefaultValue(true);

        // Empêche le meme college d'etre lie deux fois au meme domaine.
        builder.HasIndex(cd => new { cd.IdCollege, cd.IdDomaine })
            .IsUnique();

        builder.HasOne(cd => cd.College)
            .WithMany(c => c.CollegeDomaines)
            .HasForeignKey(cd => cd.IdCollege)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cd => cd.DomaineEtude)
            .WithMany(d => d.CollegeDomaines)
            .HasForeignKey(cd => cd.IdDomaine)
            .OnDelete(DeleteBehavior.Restrict);
    }
}