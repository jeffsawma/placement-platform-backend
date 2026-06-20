using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Dev 1 - Authentification, utilisateurs et rôles
    public DbSet<Role> Roles { get; set; }

    public DbSet<Utilisateur> Utilisateurs { get; set; }

    public DbSet<Administrateur> Administrateurs { get; set; }

    public DbSet<Etudiant> Etudiants { get; set; }

    public DbSet<Employeur> Employeurs { get; set; }

    public DbSet<ResponsableStage> ResponsablesStage { get; set; }

    // Dev 2 - Paramétrage, collèges et domaines d'études
    public DbSet<College> Colleges { get; set; }

    public DbSet<DomaineEtude> DomainesEtudes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
