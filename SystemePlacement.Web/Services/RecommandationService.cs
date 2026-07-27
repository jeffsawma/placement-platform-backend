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

    public RecommandationService(
        IRecommandationRepository repository,
        ICurrentUserService currentUser,
        IWebHostEnvironment env)
    {
        _repository = repository;
        _currentUser = currentUser;
        _env = env;
    }

    public async Task<IReadOnlyList<RecommandationResponse>> GetByEtudiantAsync(int idEtudiant)
    {
        var recommandations = await _repository.GetByEtudiantAsync(idEtudiant);
        return recommandations.Select(Map).ToList();
    }

    public async Task<RecommandationResponse?> CreerAsync(CreerRecommandationRequest request, IFormFile? lettre)
    {
        if (!_currentUser.IdUtilisateur.HasValue) return null;

        var recommandation = new Recommandation
        {
            IdEtudiant   = request.IdEtudiant,
            IdAuteur     = _currentUser.IdUtilisateur.Value,
            Commentaire  = request.Commentaire,
            DateCreation = DateTime.UtcNow
        };

        if (lettre is { Length: > 0 })
        {
            var dossier = Path.Combine(_env.WebRootPath, "uploads", "recommandations");
            Directory.CreateDirectory(dossier);

            var nomFichier = $"{Guid.NewGuid():N}_{Path.GetFileName(lettre.FileName)}";
            var chemin = Path.Combine(dossier, nomFichier);

            await using var stream = File.Create(chemin);
            await lettre.CopyToAsync(stream);

            recommandation.CheminLettreRecommandation = Path.Combine("uploads", "recommandations", nomFichier);
            recommandation.NomFichierLettre           = lettre.FileName;
            recommandation.ContentTypeLettre          = lettre.ContentType;
        }

        await _repository.AddAsync(recommandation);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(recommandation.IdRecommandation);
        return result is null ? null : Map(result);
    }

    public async Task<(byte[] Contenu, string ContentType, string NomFichier)?> TelechargerLettreAsync(
        int idRecommandation)
    {
        var r = await _repository.GetByIdAsync(idRecommandation);
        if (r?.CheminLettreRecommandation is null) return null;

        var chemin = Path.Combine(_env.WebRootPath, r.CheminLettreRecommandation);
        if (!File.Exists(chemin)) return null;

        var contenu = await File.ReadAllBytesAsync(chemin);
        return (contenu, r.ContentTypeLettre ?? "application/octet-stream", r.NomFichierLettre ?? "lettre.pdf");
    }

    public async Task<bool> SupprimerAsync(int idRecommandation)
    {
        var r = await _repository.GetByIdAsync(idRecommandation);
        if (r is null) return false;

        if (r.CheminLettreRecommandation is not null)
        {
            var chemin = Path.Combine(_env.WebRootPath, r.CheminLettreRecommandation);
            if (File.Exists(chemin)) File.Delete(chemin);
        }

        _repository.Delete(r);
        await _repository.SaveChangesAsync();
        return true;
    }

    private static RecommandationResponse Map(Recommandation r) => new()
    {
        IdRecommandation = r.IdRecommandation,
        IdEtudiant       = r.IdEtudiant,
        NomEtudiant      = r.Etudiant?.Utilisateur?.Nom ?? string.Empty,
        PrenomEtudiant   = r.Etudiant?.Utilisateur?.Prenom ?? string.Empty,
        NomAuteur        = r.Auteur?.Nom ?? string.Empty,
        PrenomAuteur     = r.Auteur?.Prenom ?? string.Empty,
        Commentaire      = r.Commentaire,
        ALettre          = r.CheminLettreRecommandation is not null,
        NomFichierLettre = r.NomFichierLettre,
        DateCreation     = r.DateCreation
    };
}
