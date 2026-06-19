using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Users;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UtilisateurResponseDto>> GetAllAsync()
    {
        // Retourne tous les utilisateurs avec leur role et leur college.
        return await _context.Utilisateurs
            .Include(u => u.Role)
            .Include(u => u.College)
            .OrderBy(u => u.Nom)
            .ThenBy(u => u.Prenom)
            .Select(u => new UtilisateurResponseDto
            {
                IdUtilisateur = u.IdUtilisateur,
                Prenom = u.Prenom,
                Nom = u.Nom,
                Courriel = u.Courriel,
                NomUtilisateur = u.NomUtilisateur,
                Langue = u.Langue,
                Actif = u.Actif,
                DateCreation = u.DateCreation,
                DerniereConnexion = u.DerniereConnexion,
                IdRole = u.IdRole,
                Role = u.Role != null ? u.Role.NomRole : string.Empty,
                IdCollege = u.IdCollege,
                NomCollege = u.College != null ? u.College.Nom : null
            })
            .ToListAsync();
    }

    public async Task<UtilisateurResponseDto?> GetByIdAsync(int idUtilisateur)
    {
        // Retourne un seul utilisateur selon son id.
        return await _context.Utilisateurs
            .Include(u => u.Role)
            .Include(u => u.College)
            .Where(u => u.IdUtilisateur == idUtilisateur)
            .Select(u => new UtilisateurResponseDto
            {
                IdUtilisateur = u.IdUtilisateur,
                Prenom = u.Prenom,
                Nom = u.Nom,
                Courriel = u.Courriel,
                NomUtilisateur = u.NomUtilisateur,
                Langue = u.Langue,
                Actif = u.Actif,
                DateCreation = u.DateCreation,
                DerniereConnexion = u.DerniereConnexion,
                IdRole = u.IdRole,
                Role = u.Role != null ? u.Role.NomRole : string.Empty,
                IdCollege = u.IdCollege,
                NomCollege = u.College != null ? u.College.Nom : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<UtilisateurResponseDto> CreateAsync(UtilisateurCreateDto request)
    {
        // Valide que le courriel n'est pas deja utilise.
        var courrielExiste = await _context.Utilisateurs
            .AnyAsync(u => u.Courriel == request.Courriel);

        if (courrielExiste)
        {
            throw new InvalidOperationException("Ce courriel est deja utilise.");
        }

        // Valide que le nom d'utilisateur n'est pas deja utilise.
        var nomUtilisateurExiste = await _context.Utilisateurs
            .AnyAsync(u => u.NomUtilisateur == request.NomUtilisateur);

        if (nomUtilisateurExiste)
        {
            throw new InvalidOperationException("Ce nom d'utilisateur est deja utilise.");
        }

        // Valide que le role existe.
        var roleExiste = await _context.Roles
            .AnyAsync(r => r.IdRole == request.IdRole && r.Actif);

        if (!roleExiste)
        {
            throw new InvalidOperationException("Le role selectionne est invalide.");
        }

        // Le mot de passe est hash avant d'etre sauvegarde.
        var utilisateur = new Utilisateur
        {
            Prenom = request.Prenom,
            Nom = request.Nom,
            Courriel = request.Courriel,
            NomUtilisateur = request.NomUtilisateur,
            MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(request.MotDePasse),
            Langue = request.Langue,
            IdRole = request.IdRole,
            IdCollege = request.IdCollege,
            Actif = true,
            DateCreation = DateTime.UtcNow
        };

        await _context.Utilisateurs.AddAsync(utilisateur);
        await _context.SaveChangesAsync();

        var utilisateurCree = await GetByIdAsync(utilisateur.IdUtilisateur);

        return utilisateurCree!;
    }

    public async Task<bool> UpdateAsync(int idUtilisateur, UtilisateurUpdateDto request)
    {
        var utilisateur = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.IdUtilisateur == idUtilisateur);

        if (utilisateur == null)
        {
            return false;
        }

        // On ne modifie pas le mot de passe ici.
        utilisateur.Prenom = request.Prenom;
        utilisateur.Nom = request.Nom;
        utilisateur.Courriel = request.Courriel;
        utilisateur.Langue = request.Langue;
        utilisateur.IdRole = request.IdRole;
        utilisateur.IdCollege = request.IdCollege;
        utilisateur.Actif = request.Actif;
        utilisateur.DateModification = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SetActifAsync(int idUtilisateur, bool actif)
    {
        var utilisateur = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.IdUtilisateur == idUtilisateur);

        if (utilisateur == null)
        {
            return false;
        }

        utilisateur.Actif = actif;
        utilisateur.DateModification = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}