using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data.Configurations;

public class UtilisateurConfiguration : IEntityTypeConfiguration<Utilisateur>
{
    public void Configure(EntityTypeBuilder<Utilisateur> builder)
    {
        // Table principale pour tous les comptes
        builder.ToTable("utilisateurs");

        builder.HasKey(u => u.IdUtilisateur);

        builder.Property(u => u.IdUtilisateur)
            .HasColumnName("id_utilisateur");

        builder.Property(u => u.Prenom)
            .HasColumnName("prenom")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Nom)
            .HasColumnName("nom")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Courriel)
            .HasColumnName("courriel")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.NomUtilisateur)
            .HasColumnName("nom_utilisateur")
            .HasMaxLength(100)
            .IsRequired();

        // On ne sauvegarde jamais le mot de passe en clair
        builder.Property(u => u.MotDePasseHash)
            .HasColumnName("mot_de_passe_hash")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.Langue)
            .HasColumnName("langue")
            .HasMaxLength(10)
            .HasDefaultValue("fr");

        builder.Property(u => u.Actif)
            .HasColumnName("actif")
            .HasDefaultValue(true);

        builder.Property(u => u.DateCreation)
            .HasColumnName("date_creation");

        builder.Property(u => u.DateModification)
            .HasColumnName("date_modification");

        builder.Property(u => u.DerniereConnexion)
            .HasColumnName("derniere_connexion");

        builder.Property(u => u.IdRole)
            .HasColumnName("id_role");

        builder.Property(u => u.IdCollege)
            .HasColumnName("id_college");

        // Courriel et nom d'utilisateur uniques
        builder.HasIndex(u => u.Courriel).IsUnique();
        builder.HasIndex(u => u.NomUtilisateur).IsUnique();

        // Plusieurs utilisateurs peuvent avoir le meme role
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Utilisateurs)
            .HasForeignKey(u => u.IdRole)
            .OnDelete(DeleteBehavior.Restrict);

        // Un utilisateur peut etre rattache a un college
        builder.HasOne(u => u.College)
            .WithMany(c => c.Utilisateurs)
            .HasForeignKey(u => u.IdCollege)
            .OnDelete(DeleteBehavior.SetNull);
    }
}