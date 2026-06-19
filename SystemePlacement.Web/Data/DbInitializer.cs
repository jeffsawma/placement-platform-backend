using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Cree la base si elle n'existe pas.
        await context.Database.EnsureCreatedAsync();

        await SeedRolesAsync(context);
        await SeedAdminAsync(context);
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        var roles = new List<Role>
        {
            new Role
            {
                IdRole = (int)RoleUtilisateur.Administrateur,
                NomRole = "Administrateur",
                Description = "Acces complet a la gestion de la plateforme.",
                Actif = true
            },
            new Role
            {
                IdRole = (int)RoleUtilisateur.Etudiant,
                NomRole = "Etudiant",
                Description = "Utilisateur pouvant rechercher des offres, postuler et formuler des demandes de stage.",
                Actif = true
            },
            new Role
            {
                IdRole = (int)RoleUtilisateur.Employeur,
                NomRole = "Employeur",
                Description = "Utilisateur pouvant publier des offres et consulter les candidatures.",
                Actif = true
            },
            new Role
            {
                IdRole = (int)RoleUtilisateur.ResponsableStage,
                NomRole = "ResponsableStage",
                Description = "Utilisateur pouvant suivre les etudiants, confirmer les stages et faire des recommandations.",
                Actif = true
            }
        };

        foreach (var role in roles)
        {
            var roleExiste = await context.Roles
                .AnyAsync(r => r.IdRole == role.IdRole || r.NomRole == role.NomRole);

            if (!roleExiste)
            {
                await context.Roles.AddAsync(role);
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminAsync(ApplicationDbContext context)
    {
        // Evite de creer l'admin plusieurs fois.
        var adminExiste = await context.Utilisateurs
            .AnyAsync(u => u.NomUtilisateur == "admin");

        if (adminExiste)
        {
            return;
        }

        var utilisateurAdmin = new Utilisateur
        {
            Prenom = "Admin",
            Nom = "Systeme",
            Courriel = "admin@systemeplacement.local",
            NomUtilisateur = "admin",
            MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Langue = "fr",
            Actif = true,
            DateCreation = DateTime.UtcNow,
            IdRole = (int)RoleUtilisateur.Administrateur
        };

        await context.Utilisateurs.AddAsync(utilisateurAdmin);
        await context.SaveChangesAsync();

        var profilAdmin = new Administrateur
        {
            IdUtilisateur = utilisateurAdmin.IdUtilisateur,
            NiveauAcces = "Complet"
        };

        await context.Administrateurs.AddAsync(profilAdmin);
        await context.SaveChangesAsync();
    }
}