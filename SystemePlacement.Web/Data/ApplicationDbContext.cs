using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    // Dev 1 - Authentification, utilisateurs et r�les

    // Dev 1 - Authentification, utilisateurs et roles

    // Dev 1 - Authentification, Utilisateurs et Roles

    public DbSet<Role> Roles { get; set; }
    public DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<Administrateur> Administrateurs { get; set; }
    public DbSet<Etudiant> Etudiants { get; set; }
    public DbSet<Employeur> Employeurs { get; set; }
    public DbSet<ResponsableStage> ResponsablesStage { get; set; }




    // Dev 1 - Sprint 2



    public DbSet<DemarcheSuivi> DemarchesSuivi => Set<DemarcheSuivi>();
    public DbSet<Stage> Stages => Set<Stage>();
    public DbSet<ConfirmationStage> ConfirmationsStage => Set<ConfirmationStage>();


    // Dev 2 - Param�trage, collèges et domaines d'études

    // Dev 2 - Parametrage, colleges et domaines d'etudes

    


    // Dev 2 - Collèges, Domaines d'études, Offres de stage directes
    public DbSet<College> Colleges { get; set; }
    public DbSet<DomaineEtude> DomainesEtudes { get; set; }
    public DbSet<OffreStageDirecte> OffresStageDirectes { get; set; }


    // Dev 3 - Offres

    // Dev 3 - Offres, Offre de domaines, Entreprises

    public DbSet<Offre> Offres { get; set; }
    public DbSet<OffreDomaine> OffreDomaines { get; set; }
    public DbSet<Entreprise> Entreprises { get; set; }


    // Dev 4 - Candidatures

    // Dev 4 - Candidatures, documents et demandes de stage

   



    // Dev 4 - Candidatures, Documents pour une candidature, Demandes de stage
    public DbSet<Candidature> Candidatures { get; set; }

    public DbSet<CandidatureDocument> CandidatureDocuments { get; set; }
    public DbSet<DemandeStage> DemandesStage { get; set; }

    // Notifications
    public DbSet<Notification> Notifications { get; set; }


    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
