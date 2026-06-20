using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) 
    {
    }

    public DbSet<College> Colleges { get; set; } // DbSet pour les collèges
    public DbSet<DomaineEtude> DomainesEtudes { get; set; } //  DbSet pour les domaines d'étude

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
