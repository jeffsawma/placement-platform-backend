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
    private readonly INotificationService _notification;

    public RecommandationService(
        IRecommandationRepository repository,
        ICurrentUserService currentUser,
        IWebHostEnvironment env,
        ApplicationDbContext context,
        INotificationService notification)
    {
        _repository = repository;
        _currentUser = currentUser;
        _env = env;
        _context = context;
        _notification = notification;
    }

        public async Task<IReadOnlyList<RecommandationResponse>> GetByEtudiantAsync(
        int idEtudiant)
    {
        if (!await PeutGererEtudiantAsync(idEtudiant))
        {
            return Array.Empty<RecommandationResponse>();
        }

        var recommandations =
            await _repository.GetByEtudiantAsync(idEtudiant);

        return recommandations.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<RecommandationResponse>> GetMesRecommandationsRecuesAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
        {
            return [];
        }

        // On rÃ©cupÃ¨re le profil employeur liÃ© Ã  l'utilisateur connectÃ©.
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

        public async Task<RecommandationResponse?> CreerAsync(
        CreerRecommandationRequest request,
        IFormFile? lettre)
    {
        if (!_currentUser.IdUtilisateur.HasValue)
        {
            return null;
        }

        if (!await PeutGererEtudiantAsync(request.IdEtudiant))
        {
            return null;
        }

        if (!request.IdEmployeurDestinataire.HasValue)
        {
            return null;
        }

        // VÃ©rifie que l'employeur choisi existe vraiment.
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

        if (result is not null)
        {
            var nomEtudiant =
                $"{result.Etudiant?.Utilisateur?.Prenom} {result.Etudiant?.Utilisateur?.Nom}"
                    .Trim();

            if (string.IsNullOrWhiteSpace(nomEtudiant))
            {
                nomEtudiant = "l'étudiant concerné";
            }

            await _notification.NotifierEmployeurAsync(
                request.IdEmployeurDestinataire.Value,
                $"Vous avez reçu une recommandation pour {nomEtudiant}.",
                "/employeur/recommandations-recues");
        }

        return result is null ? null : Map(result);
    }

    public async Task<(byte[] Contenu, string ContentType, string NomFichier)?> TelechargerLettreAsync(
        int idRecommandation)
    {
                var recommandation =
            await _repository.GetByIdAsync(idRecommandation);

        if (recommandation is null ||
            recommandation.CheminLettreRecommandation is null ||
            !await PeutConsulterRecommandationAsync(recommandation))
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
                var recommandation =
            await _repository.GetByIdAsync(idRecommandation);

        if (recommandation is null ||
            !PeutSupprimerRecommandation(recommandation))
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

    private async Task<bool> PeutGererEtudiantAsync(
        int idEtudiant)
    {
        if (_currentUser.Role == "SuperAdministrateur")
        {
            return true;
        }

        if (_currentUser.Role != "ResponsableStage" &&
            _currentUser.Role != "Administrateur")
        {
            return false;
        }

        if (!_currentUser.IdCollege.HasValue)
        {
            return false;
        }

        var idCollege = _currentUser.IdCollege.Value;

        return await _context.Etudiants
            .AsNoTracking()
            .AnyAsync(e =>
                e.IdEtudiant == idEtudiant &&
                e.Utilisateur != null &&
                e.Utilisateur.IdCollege == idCollege &&
                e.Utilisateur.Actif);
    }

    private async Task<bool> PeutConsulterRecommandationAsync(
        Recommandation recommandation)
    {
        if (!_currentUser.IdUtilisateur.HasValue)
        {
            return false;
        }

        if (_currentUser.Role == "SuperAdministrateur")
        {
            return true;
        }

        if (_currentUser.Role == "ResponsableStage" ||
            _currentUser.Role == "Administrateur")
        {
            return await PeutGererEtudiantAsync(
                recommandation.IdEtudiant);
        }

        if (_currentUser.Role == "Employeur")
        {
            var idUtilisateur =
                _currentUser.IdUtilisateur.Value;

            var idEmployeur = await _context.Employeurs
                .AsNoTracking()
                .Where(e =>
                    e.IdUtilisateur == idUtilisateur)
                .Select(e => (int?)e.IdEmployeur)
                .FirstOrDefaultAsync();

            return idEmployeur.HasValue &&
                recommandation.IdEmployeurDestinataire ==
                idEmployeur.Value;
        }

        return false;
    }

    private bool PeutSupprimerRecommandation(
        Recommandation recommandation)
    {
        if (!_currentUser.IdUtilisateur.HasValue)
        {
            return false;
        }

        return _currentUser.Role == "SuperAdministrateur" ||
            recommandation.IdAuteur ==
            _currentUser.IdUtilisateur.Value;
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