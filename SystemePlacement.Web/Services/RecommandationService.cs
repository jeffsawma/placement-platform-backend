using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Recommandations;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class RecommandationService : IRecommandationService
{
    private readonly IRecommandationRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IWebHostEnvironment _env;
    private readonly ApplicationDbContext _context;

    public RecommandationService(
        IRecommandationRepository repository,
        ICurrentUserService currentUser,
        IWebHostEnvironment env,
        ApplicationDbContext context)
    {
        _repository = repository;
        _currentUser = currentUser;
        _env = env;
        _context = context;
    }

    public async Task<IReadOnlyList<RecommandationResponse>> GetByEtudiantAsync(int idEtudiant)
    {
        var recommandations = await _repository.GetByEtudiantAsync(idEtudiant);
        return recommandations.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<RecommandationResponse>> GetMesRecommandationsRecuesAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
        {
            return [];
        }

        // On récupère le profil employeur lié à l'utilisateur connecté.
        var employeur = await _context.Employeurs
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.IdUtilisateur == _currentUser.IdUtilisateur.Value);

        if (employeur is null)
        {
            return [];
        }

        var recommandations = await _repository.GetByEmployeurAsync(employeur.IdEmployeur);

        return recommandations.Select(Map).ToList();
    }

    public async Task<RecommandationResponse?> CreerAsync(CreerRecommandationRequest request, IFormFile? lettre)
    {
        if (!_currentUser.IdUtilisateur.HasValue)
        {
            return null;
        }

        if (!request.IdEmployeurDestinataire.HasValue)
        {
            return null;
        }

        // Vérifie que l'employeur choisi existe vraiment.
        var employeurExiste = await _context.Employeurs
            .AsNoTracking()
            .AnyAsync(e => e.IdEmployeur == request.IdEmployeurDestinataire.Value);

        if (!employeurExiste)
        {
            return null;
        }

        var recommandation = new Recommandation
        {
            IdEtudiant = request.IdEtudiant,
            IdAuteur = _currentUser.IdUtilisateur.Value,
            IdEmployeurDestinataire = request.IdEmployeurDestinataire.Value,
            Commentaire = request.Commentaire,
            DateCreation = DateTime.UtcNow
        };

        if (lettre is { Length: > 0 })
        {
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var dossier = Path.Combine(webRootPath, "uploads", "recommandations");
            Directory.CreateDirectory(dossier);

            var nomFichier = $"{Guid.NewGuid():N}_{Path.GetFileName(lettre.FileName)}";
            var chemin = Path.Combine(dossier, nomFichier);

            await using var stream = File.Create(chemin);
            await lettre.CopyToAsync(stream);

            recommandation.CheminLettreRecommandation = Path.Combine("uploads", "recommandations", nomFichier);
            recommandation.NomFichierLettre = lettre.FileName;
            recommandation.ContentTypeLettre = lettre.ContentType;
        }

        await _repository.AddAsync(recommandation);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(recommandation.IdRecommandation);

        return result is null ? null : Map(result);
    }

    public async Task<(byte[] Contenu, string ContentType, string NomFichier)?> TelechargerLettreAsync(
        int idRecommandation)
    {
        var recommandation = await _repository.GetByIdAsync(idRecommandation);

        if (recommandation?.CheminLettreRecommandation is null)
        {
            return null;
        }

        var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        var chemin = Path.Combine(webRootPath, recommandation.CheminLettreRecommandation);

        if (!File.Exists(chemin))
        {
            return null;
        }

        var contenu = await File.ReadAllBytesAsync(chemin);

        return (
            contenu,
            recommandation.ContentTypeLettre ?? "application/octet-stream",
            recommandation.NomFichierLettre ?? "lettre.pdf"
        );
    }

    public async Task<bool> SupprimerAsync(int idRecommandation)
    {
        var recommandation = await _repository.GetByIdAsync(idRecommandation);

        if (recommandation is null)
        {
            return false;
        }

        if (recommandation.CheminLettreRecommandation is not null)
        {
            var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var chemin = Path.Combine(webRootPath, recommandation.CheminLettreRecommandation);

            if (File.Exists(chemin))
            {
                File.Delete(chemin);
            }
        }

        _repository.Delete(recommandation);
        await _repository.SaveChangesAsync();

        return true;
    }

    private static RecommandationResponse Map(Recommandation recommandation)
    {
        var nomEntreprise =
            recommandation.EmployeurDestinataire?.Entreprise?.Nom;

        var nomCompteEmployeur =
            $"{recommandation.EmployeurDestinataire?.Utilisateur?.Prenom} {recommandation.EmployeurDestinataire?.Utilisateur?.Nom}".Trim();

        return new RecommandationResponse
        {
            IdRecommandation = recommandation.IdRecommandation,
            IdEtudiant = recommandation.IdEtudiant,
            NomEtudiant = recommandation.Etudiant?.Utilisateur?.Nom ?? string.Empty,
            PrenomEtudiant = recommandation.Etudiant?.Utilisateur?.Prenom ?? string.Empty,
            IdEmployeurDestinataire = recommandation.IdEmployeurDestinataire,
            NomEmployeurDestinataire = !string.IsNullOrWhiteSpace(nomEntreprise)
                ? nomEntreprise
                : nomCompteEmployeur,
            NomAuteur = recommandation.Auteur?.Nom ?? string.Empty,
            PrenomAuteur = recommandation.Auteur?.Prenom ?? string.Empty,
            Commentaire = recommandation.Commentaire,
            ALettre = recommandation.CheminLettreRecommandation is not null,
            NomFichierLettre = recommandation.NomFichierLettre,
            DateCreation = recommandation.DateCreation
        };
    }
}