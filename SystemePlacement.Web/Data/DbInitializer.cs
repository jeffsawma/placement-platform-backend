using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        Console.WriteLine("[Seed] Debut de l'initialisation de la base.");

        // Cree la base si elle n'existe pas.
        await context.Database.EnsureCreatedAsync();

        // Les roles doivent exister avant les utilisateurs.
        await SeedRolesAsync(context);

        // Compte global de la plateforme.
        await SeedSuperAdminAsync(context);

        // Compte admin local de test.
        await SeedAdminAsync(context);

        Console.WriteLine("[Seed] Initialisation terminee.");
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        var roles = new List<Role>
        {
            new Role
            {
                IdRole = (int)RoleUtilisateur.Administrateur,
                NomRole = "Administrateur",
                Description = "Admin d'un college. Gere les utilisateurs et domaines de son college.",
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
            },
            new Role
            {
                IdRole = (int)RoleUtilisateur.SuperAdministrateur,
                NomRole = "SuperAdministrateur",
                Description = "Admin global. Gere les colleges et les administrateurs de college.",
                Actif = true
            }
        };

        foreach (var role in roles)
        {
            // Evite d'inserer deux fois le meme role.
            var roleExiste = await context.Roles
                .AnyAsync(r => r.IdRole == role.IdRole || r.NomRole == role.NomRole);

            if (!roleExiste)
            {
                Console.WriteLine($"[Seed] Ajout du role : {role.NomRole}");
                await context.Roles.AddAsync(role);
            }
            else
            {
                Console.WriteLine($"[Seed] Role deja present : {role.NomRole}");
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminAsync(ApplicationDbContext context)
    {
        // Evite de creer le superadmin plusieurs fois.
        var existe = await context.Utilisateurs
            .AnyAsync(u => u.NomUtilisateur == "superadmin");

        if (existe)
        {
            Console.WriteLine("[Seed] SuperAdmin deja present.");
            return;
        }

        var roleSuperAdminExiste = await context.Roles
            .AnyAsync(r => r.IdRole == (int)RoleUtilisateur.SuperAdministrateur);

        if (!roleSuperAdminExiste)
        {
            throw new InvalidOperationException("Le role SuperAdministrateur est manquant. Verifiez la table roles.");
        }

        var superAdmin = new Utilisateur
        {
            Prenom = "Super",
            Nom = "Admin",
            Courriel = "superadmin@systemeplacement.local",
            NomUtilisateur = "superadmin",
            MotDePasseHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin123!"),
            Langue = "fr",
            Actif = true,
            DateCreation = DateTime.UtcNow,

            // Le superadmin n'est rattache a aucun college.
            IdCollege = null,

            // Role global.
            IdRole = (int)RoleUtilisateur.SuperAdministrateur
        };

        await context.Utilisateurs.AddAsync(superAdmin);
        await context.SaveChangesAsync();

        await context.Administrateurs.AddAsync(new Administrateur
        {
            IdUtilisateur = superAdmin.IdUtilisateur,
            NiveauAcces = "Global"
        });

        await context.SaveChangesAsync();

        Console.WriteLine("[Seed] SuperAdmin cree : superadmin / SuperAdmin123!");
    }

    private static async Task SeedAdminAsync(ApplicationDbContext context)
    {
        // Ancien admin local de test.
        var adminExiste = await context.Utilisateurs
            .AnyAsync(u => u.NomUtilisateur == "admin");

        if (adminExiste)
        {
            Console.WriteLine("[Seed] Admin de test deja present.");
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

            // Pour l'instant null tant qu'on n'a pas encore cree le college de test.
            IdCollege = null,

            IdRole = (int)RoleUtilisateur.Administrateur
        };

        await context.Utilisateurs.AddAsync(utilisateurAdmin);
        await context.SaveChangesAsync();

        await context.Administrateurs.AddAsync(new Administrateur
        {
            IdUtilisateur = utilisateurAdmin.IdUtilisateur,
            NiveauAcces = "Complet"
        });

        await context.SaveChangesAsync();
    }
}
