using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Entreprises;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Services.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace SystemePlacement.Web.Services;

public class EntrepriseService : IEntrepriseService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public EntrepriseService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<EntrepriseResponseDto?> GetMonProfilAsync()
    {
        var employeur = await GetEmployeurConnecteAsync();

        var entreprise = await _context.Entreprises
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.IdEmployeur == employeur.IdEmployeur);

        if (entreprise == null)
        {
            return null;
        }

        return ToResponseDto(entreprise);
    }

    public async Task<EntrepriseResponseDto> CreateMonProfilAsync(EntrepriseCreateDto dto)
    {
        var employeur = await GetEmployeurConnecteAsync();

        var profilExiste = await _context.Entreprises
            .AnyAsync(e => e.IdEmployeur == employeur.IdEmployeur); // Vérifie si un profil d'entreprise existe déjà pour un employeur spécifique

        if (profilExiste)
        {
            throw new InvalidOperationException("Un profil d'entreprise existe déjà pour cet employeur.");
        }

        // Valeurs requises de l'employeur ou utilisateur authentifié pour la création d'un profil d'entreprise
        var nom = dto.Nom.Trim();
        var secteur = dto.Secteur.Trim();
        var adresse = dto.Adresse.Trim();

        // Validation des valeurs
        if (string.IsNullOrWhiteSpace(nom) ||
            string.IsNullOrWhiteSpace(secteur) ||
            string.IsNullOrWhiteSpace(adresse))
        {
            throw new InvalidOperationException("Le nom, le secteur et l'adresse sont obligatoire.");
        }

        // Création du profil d'entreprise
        var entreprise = new Entreprise
        {
            IdEmployeur = employeur.IdEmployeur,
            Nom = nom,
            Secteur = secteur,
            Adresse = adresse,
            SiteWeb = string.IsNullOrWhiteSpace(dto.SiteWeb) ? null : dto.SiteWeb.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            LogoUrl = string.IsNullOrWhiteSpace(dto.LogoUrl) ? null : dto.LogoUrl.Trim()
        };

        _context.Entreprises.Add(entreprise); // Ajout du nouveau profil d’entreprise à la liste des entreprises dans la bd
        await _context.SaveChangesAsync();

        return ToResponseDto(entreprise);
    }

    public async Task<bool> UpdateMonProfilAsync(EntrepriseUpdateDto dto)
    {
        var employeur = await GetEmployeurConnecteAsync();

        var entreprise = await _context.Entreprises
            .FirstOrDefaultAsync(e => e.IdEmployeur == employeur.IdEmployeur);

        if (entreprise == null)
        {
            return false; // Pas de profil d'entreprise existe pour un employeur spécifique
        }

        var nom = dto.Nom.Trim();
        var secteur = dto.Secteur.Trim();
        var adresse = dto.Adresse.Trim();

        if (string.IsNullOrWhiteSpace(nom) ||
            string.IsNullOrWhiteSpace(secteur) ||
            string.IsNullOrWhiteSpace(adresse))
        {
            throw new InvalidOperationException("Le nom, le secteur et l'adresse sont obligatoires.");
        }

        // Modifié les valeurs des attributs d'une entreprise selon une requette de l'employeur
        entreprise.Nom = nom;
        entreprise.Secteur = secteur;
        entreprise.Adresse = adresse;
        entreprise.SiteWeb = string.IsNullOrWhiteSpace(dto.SiteWeb) ? null : dto.SiteWeb.Trim();
        entreprise.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
        entreprise.LogoUrl = string.IsNullOrWhiteSpace(dto.LogoUrl) ? null : dto.LogoUrl.Trim();

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task<Employeur> GetEmployeurConnecteAsync()
    {
        // Vérification de l'authentification de l'utilisateur actuel
        if (!_currentUserService.IsAuthenticated || _currentUserService.IdUtilisateur == null)
        {
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");
        }

        // Vérifier si l'utilisateur authentifié actuel avec un id spécifique est l'employeur possédant le même id
        var employeur = await _context.Employeurs
            .FirstOrDefaultAsync(e => e.IdUtilisateur == _currentUserService.IdUtilisateur.Value);

        if (employeur == null)
        {
            throw new InvalidOperationException("Aucun profil employeur n'est associé à cet utilisateur.");
        }

        return employeur;

    }

    private static EntrepriseResponseDto ToResponseDto(Entreprise entreprise)
    {
        return new EntrepriseResponseDto
        {
            IdEntreprise = entreprise.IdEntreprise,
            IdEmployeur = entreprise.IdEmployeur,
            Nom = entreprise.Nom,
            Secteur = entreprise.Secteur,
            Adresse = entreprise.Adresse,
            SiteWeb = entreprise.SiteWeb,
            Description = entreprise.Description,
            LogoUrl = entreprise.LogoUrl
        };
    }
}
