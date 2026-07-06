using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Dev 1 - Authentification, Utilisateurs et Roles
    public DbSet<Role> Roles { get; set; }
    public DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<Administrateur> Administrateurs { get; set; }
    public DbSet<Etudiant> Etudiants { get; set; }
    public DbSet<Employeur> Employeurs { get; set; }
    public DbSet<ResponsableStage> ResponsablesStage { get; set; }
    public DbSet<DemarcheSuivi> DemarchesSuivi => Set<DemarcheSuivi>();


    // Dev 2 - Collèges, Domaines d'études, Offres de stage directes
    public DbSet<College> Colleges { get; set; }
    public DbSet<DomaineEtude> DomainesEtudes { get; set; }
    public DbSet<OffreStageDirecte> OffresStageDirectes { get; set; }


    // Dev 3 - Offres, Offre de domaines, Entreprises
    public DbSet<Offre> Offres { get; set; }
    public DbSet<OffreDomaine> OffreDomaines { get; set; }
    public DbSet<Entreprise> Entreprises { get; set; }


    // Dev 4 - Candidatures, Documents pour une candidature, Demandes de stage
    public DbSet<Candidature> Candidatures { get; set; }
    public DbSet<CandidatureDocument> CandidatureDocuments { get; set; }
    public DbSet<DemandeStage> DemandesStage { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
