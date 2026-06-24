using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Users;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UserService(ApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<UtilisateurResponseDto>> GetAllAsync()
    {
        // SuperAdmin voit tous les utilisateurs.
        // Admin voit seulement les utilisateurs de son college.
        var query = ApplyCollegeScope(_context.Utilisateurs.AsQueryable());

        return await query
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
        // Retourne un seul utilisateur, en respectant le college de l'admin.
        var query = ApplyCollegeScope(_context.Utilisateurs.AsQueryable());

        return await query
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
        ApplyCreateRules(request);

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

        await ValidateCollegeAsync(request.IdCollege);

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
        ApplyUpdateRules(request);

        var query = ApplyCollegeScope(_context.Utilisateurs.AsQueryable());

        var utilisateur = await query
            .FirstOrDefaultAsync(u => u.IdUtilisateur == idUtilisateur);

        if (utilisateur == null)
        {
            return false;
        }

        await ValidateCollegeAsync(request.IdCollege);

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
        var query = ApplyCollegeScope(_context.Utilisateurs.AsQueryable());

        var utilisateur = await query
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

    private IQueryable<Utilisateur> ApplyCollegeScope(IQueryable<Utilisateur> query)
    {
        // Le SuperAdmin est global : il peut consulter tous les colleges.
        if (_currentUserService.Role == nameof(RoleUtilisateur.SuperAdministrateur))
        {
            return query;
        }

        // Un admin local ne doit voir que les utilisateurs de son college.
        if (_currentUserService.Role == nameof(RoleUtilisateur.Administrateur))
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                return query.Where(u => false);
            }

            return query.Where(u => u.IdCollege == _currentUserService.IdCollege.Value);
        }

        // Par securite, aucun autre role ne doit acceder a la gestion des utilisateurs.
        return query.Where(u => false);
    }

    private void ApplyCreateRules(UtilisateurCreateDto request)
    {
        // Un admin de college ne peut pas creer un SuperAdmin
        // et ne peut creer que des utilisateurs dans son propre college.
        if (_currentUserService.Role == nameof(RoleUtilisateur.Administrateur))
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                throw new InvalidOperationException("Votre compte administrateur n'est rattache a aucun college.");
            }

            if (request.IdRole == (int)RoleUtilisateur.SuperAdministrateur)
            {
                throw new InvalidOperationException("Seul un SuperAdministrateur peut creer un SuperAdministrateur.");
            }

            request.IdCollege = _currentUserService.IdCollege.Value;
        }
    }

    private void ApplyUpdateRules(UtilisateurUpdateDto request)
    {
        // Un admin de college ne peut pas transformer un utilisateur en SuperAdmin
        // et ne peut pas le deplacer dans un autre college.
        if (_currentUserService.Role == nameof(RoleUtilisateur.Administrateur))
        {
            if (!_currentUserService.IdCollege.HasValue)
            {
                throw new InvalidOperationException("Votre compte administrateur n'est rattache a aucun college.");
            }

            if (request.IdRole == (int)RoleUtilisateur.SuperAdministrateur)
            {
                throw new InvalidOperationException("Seul un SuperAdministrateur peut attribuer le role SuperAdministrateur.");
            }

            request.IdCollege = _currentUserService.IdCollege.Value;
        }
    }

    private async Task ValidateCollegeAsync(int? idCollege)
    {
        // Le SuperAdmin peut ne pas etre rattache a un college.
        if (!idCollege.HasValue)
        {
            return;
        }

        var collegeExiste = await _context.Colleges
            .AnyAsync(c => c.IdCollege == idCollege.Value && c.Actif);

        if (!collegeExiste)
        {
            throw new InvalidOperationException("Le college selectionne est invalide.");
        }
    }
}
