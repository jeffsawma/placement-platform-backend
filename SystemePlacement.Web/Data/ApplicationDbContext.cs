using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Dev 1 - Authentification, utilisateurs et roles
    public DbSet<Role> Roles { get; set; }
    public DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<Administrateur> Administrateurs { get; set; }
    public DbSet<Etudiant> Etudiants { get; set; }
    public DbSet<Employeur> Employeurs { get; set; }
    public DbSet<ResponsableStage> ResponsablesStage { get; set; }

    // Dev 1 - Sprint 2 : suivi et confirmations
    public DbSet<DemarcheSuivi> DemarchesSuivi => Set<DemarcheSuivi>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<ConfirmationStage> ConfirmationsStage => Set<ConfirmationStage>();

    // Dev 2 - Colleges et domaines d'etudes
    public DbSet<College> Colleges { get; set; }
    public DbSet<DomaineEtude> DomainesEtudes { get; set; }

    // Liaison plusieurs-a-plusieurs entre colleges et domaines
    public DbSet<CollegeDomaine> CollegeDomaines => Set<CollegeDomaine>();

    // Dev 2 - Offres de stage directes
    public DbSet<OffreStageDirecte> OffresStageDirectes { get; set; }

    // Dev 3 - Offres, domaines d'offres et entreprises
    public DbSet<Offre> Offres { get; set; }
    public DbSet<OffreDomaine> OffreDomaines { get; set; }
    public DbSet<Entreprise> Entreprises { get; set; }

    // Dev 4 - Candidatures, documents et demandes de stage
    public DbSet<Candidature> Candidatures { get; set; }
    public DbSet<CandidatureDocument> CandidatureDocuments { get; set; }
    public DbSet<DemandeStage> DemandesStage { get; set; }

    // Recommandations et notifications
    public DbSet<Recommandation> Recommandations { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Applique automatiquement toutes les classes de configuration EF.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Lie Stage.IdEtudiant avec Etudiant.IdEtudiant.
        modelBuilder.Entity<Stage>()
            .HasOne(s => s.Etudiant)
            .WithMany()
            .HasForeignKey(s => s.IdEtudiant)
            .OnDelete(DeleteBehavior.Restrict);

        // Lie Stage.IdOffre avec Offre.IdOffre.
        modelBuilder.Entity<Stage>()
            .HasOne(s => s.Offre)
            .WithMany()
            .HasForeignKey(s => s.IdOffre)
            .OnDelete(DeleteBehavior.SetNull);

        // Lie ConfirmationStage.IdStage avec Stage.IdStage.
        modelBuilder.Entity<ConfirmationStage>()
            .HasOne(c => c.Stage)
            .WithMany(s => s.Confirmations)
            .HasForeignKey(c => c.IdStage)
            .OnDelete(DeleteBehavior.Cascade);

        // Lie ConfirmationStage.IdUtilisateur avec Utilisateur.IdUtilisateur.
        modelBuilder.Entity<ConfirmationStage>()
            .HasOne(c => c.Utilisateur)
            .WithMany()
            .HasForeignKey(c => c.IdUtilisateur)
            .OnDelete(DeleteBehavior.Restrict);
    }
}