using SystemePlacement.Web.DTOs.Candidatures;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Repositories.Interfaces;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class CandidatureService : ICandidatureService
{
    private readonly ICandidatureRepository _repository;
    private readonly IOffreRepository _offreRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IWebHostEnvironment _env;

    public CandidatureService(
        ICandidatureRepository repository,
        IOffreRepository offreRepository,
        ICurrentUserService currentUser,
        IWebHostEnvironment env)
    {
        _repository = repository;
        _offreRepository = offreRepository;
        _currentUser = currentUser;
        _env = env;
    }

    // US-10 : liste des candidatures d'une offre
    public async Task<IReadOnlyList<CandidatureResumeeResponse>> GetCandidaturesOffreAsync(int idOffre)
    {
        // Verifier que l'employeur connecte est bien proprietaire de l'offre
        if (_currentUser.Role == "Employeur")
        {
            var idEmployeur = await _offreRepository.GetIdEmployeurByUtilisateurAsync(
                _currentUser.IdUtilisateur!.Value);

            var offre = await _offreRepository.GetByIdAsync(idOffre);
            if (offre is null || offre.IdEmployeur != idEmployeur)
                return Array.Empty<CandidatureResumeeResponse>();
        }

        var candidatures = await _repository.GetByOffreAsync(idOffre);
        return candidatures.Select(c => new CandidatureResumeeResponse
        {
            IdCandidature     = c.IdCandidature,
            IdOffre           = c.IdOffre,
            TitreOffre        = c.Offre?.Titre ?? string.Empty,
            NomEtudiant       = c.Etudiant?.Utilisateur?.Nom ?? string.Empty,
            PrenomEtudiant    = c.Etudiant?.Utilisateur?.Prenom ?? string.Empty,
            EmailEtudiant     = c.Etudiant?.Utilisateur?.Email,
            Statut            = c.Statut,
            DateCandidature   = c.DateCandidature,
            ACV               = c.Documents.Any(d => d.TypeDocument == TypeDocument.CV),
            ALettreMotivation = c.Documents.Any(d => d.TypeDocument == TypeDocument.LettreMotivation)
        }).ToList();
    }

    // Detail d'une candidature
    public async Task<CandidatureDetailResponse?> GetDetailAsync(int idCandidature)
    {
        var c = await _repository.GetByIdAsync(idCandidature);
        if (c is null) return null;

        return new CandidatureDetailResponse
        {
            IdCandidature     = c.IdCandidature,
            IdOffre           = c.IdOffre,
            TitreOffre        = c.Offre?.Titre ?? string.Empty,
            NomEtudiant       = c.Etudiant?.Utilisateur?.Nom ?? string.Empty,
            PrenomEtudiant    = c.Etudiant?.Utilisateur?.Prenom ?? string.Empty,
            EmailEtudiant     = c.Etudiant?.Utilisateur?.Email,
            Statut            = c.Statut,
            DateCandidature   = c.DateCandidature,
            MessageMotivation = c.MessageMotivation,
            ACV               = c.Documents.Any(d => d.TypeDocument == TypeDocument.CV),
            ALettreMotivation = c.Documents.Any(d => d.TypeDocument == TypeDocument.LettreMotivation),
            Documents         = c.Documents.Select(d => new DocumentResponse
            {
                IdDocument    = d.IdDocument,
                TypeDocument  = d.TypeDocument,
                NomFichier    = d.NomFichier,
                TailleFichier = d.TailleFichier,
                DateUpload    = d.DateUpload
            }).ToList()
        };
    }

    // US-10 : changer le statut
    public async Task<bool> ChangerStatutAsync(int idCandidature, StatutCandidature statut)
    {
        var candidature = await _repository.GetByIdAsync(idCandidature);
        if (candidature is null) return false;

        candidature.Statut = statut;
        _repository.Update(candidature);
        await _repository.SaveChangesAsync();
        return true;
    }

    // US-12 : telecharger un document
    public async Task<(byte[] Contenu, string ContentType, string NomFichier)?> TelechargerDocumentAsync(
        int idDocument)
    {
        var document = await _repository.GetDocumentAsync(idDocument);
        if (document is null) return null;

        var cheminComplet = Path.Combine(_env.WebRootPath, document.CheminFichier.TrimStart('/'));
        if (!File.Exists(cheminComplet)) return null;

        var contenu = await File.ReadAllBytesAsync(cheminComplet);
        var contentType = document.ContentType ?? "application/octet-stream";
        return (contenu, contentType, document.NomFichier);
    }
}
