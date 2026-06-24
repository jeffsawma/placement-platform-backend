using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;


namespace SystemePlacement.Web.Data.Configurations;

public class DomaineEtudeConfiguration : IEntityTypeConfiguration<DomaineEtude>
{
    public void Configure(EntityTypeBuilder<DomaineEtude> builder)
    {
        builder.ToTable("DOMAINE_ETUDE"); 

        builder.HasKey(d => d.IdDomaine); 

        builder.Property(d => d.IdDomaine)
            .HasColumnName("id_domaine");

        builder.Property(d => d.IdCollege) 
            .HasColumnName("id_college")
            .IsRequired();

        builder.Property(d => d.Nom)
            .HasColumnName("nom")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(d => d.Code)
            .HasColumnName("code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(d => d.AccepteStagiaires)
            .HasColumnName("accepte_stagiaires")
            .IsRequired();

        builder.Property(d => d.Actif)
            .HasColumnName("actif")
            .IsRequired();

        // Relationship
        builder.HasOne(d => d.College)
            .WithMany(c => c.DomaineEtudes)
            .HasForeignKey(d => d.IdCollege)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
