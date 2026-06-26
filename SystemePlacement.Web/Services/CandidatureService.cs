using SystemePlacement.Web.DTOs.Candidatures;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories;
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

    public async Task<IReadOnlyList<CandidatureResponse>> GetParOffreAsync(int idOffre)
    {
        var candidatures = await _repository.GetByOffreAsync(idOffre);
        return candidatures.Select(Map).ToList();
    }

    public async Task<CandidatureResponse?> GetAsync(int idCandidature)
    {
        var candidature = await _repository.GetByIdAsync(idCandidature);
        return candidature is null ? null : Map(candidature);
    }

    public async Task<CandidatureResponse?> PostulerAsync(PostulerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CvUrl))
            return null;

        if (!_currentUser.IdUtilisateur.HasValue)
            return null;

        var idEtudiant = await _repository.GetIdEtudiantByUtilisateurAsync(_currentUser.IdUtilisateur.Value);
        if (idEtudiant is null)
            return null;

        if (await _repository.ExistsAsync(request.IdOffre, idEtudiant.Value))
            return null;

        var candidature = new Candidature
        {
            IdOffre = request.IdOffre,
            IdEtudiant = idEtudiant.Value,
            CvUrl = request.CvUrl,
            LettreMotivation = request.LettreMotivation,
            MessageMotivation = request.LettreMotivation,
            DateCandidature = DateTime.UtcNow,
            Statut = StatutCandidature.EnAttente
        };

        await _repository.AddAsync(candidature);
        await _repository.SaveChangesAsync();

        return Map(candidature);
    }

    public async Task<bool> ChangerStatutAsync(int idCandidature, ChangerStatutRequest request)
    {
        return await ChangerStatutAsync(idCandidature, request.Statut);
    }

    public async Task<IReadOnlyList<CandidatureResumeeResponse>> GetCandidaturesOffreAsync(int idOffre)
    {
        if (_currentUser.Role == "Employeur")
        {
            if (!_currentUser.IdUtilisateur.HasValue)
                return Array.Empty<CandidatureResumeeResponse>();

            var idEmployeur = await _offreRepository.GetIdEmployeurByUtilisateurAsync(_currentUser.IdUtilisateur.Value);
            var offre = await _offreRepository.GetByIdAsync(idOffre);

            if (offre is null || !idEmployeur.HasValue || offre.IdEmployeur != idEmployeur.Value)
                return Array.Empty<CandidatureResumeeResponse>();
        }

        var candidatures = await _repository.GetByOffreAsync(idOffre);
        return candidatures.Select(MapResumee).ToList();
    }

    public async Task<CandidatureDetailResponse?> GetDetailAsync(int idCandidature)
    {
        var candidature = await _repository.GetByIdAsync(idCandidature);
        return candidature is null ? null : MapDetail(candidature);
    }

    public async Task<bool> ChangerStatutAsync(int idCandidature, StatutCandidature statut)
    {
        var candidature = await _repository.GetByIdAsync(idCandidature);
        if (candidature is null)
            return false;

        candidature.Statut = statut;
        _repository.Update(candidature);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<(byte[] Contenu, string ContentType, string NomFichier)?> TelechargerDocumentAsync(int idDocument)
    {
        var document = await _repository.GetDocumentAsync(idDocument);
        if (document is null)
            return null;

        var cheminRelatif = document.CheminFichier.TrimStart('/', '\\');
        var cheminComplet = Path.Combine(_env.WebRootPath, cheminRelatif);

        if (!File.Exists(cheminComplet))
            return null;

        var contenu = await File.ReadAllBytesAsync(cheminComplet);
        var contentType = document.ContentType ?? "application/octet-stream";

        return (contenu, contentType, document.NomFichier);
    }

    private static CandidatureResponse Map(Candidature c) => new()
    {
        IdCandidature = c.IdCandidature,
        IdOffre = c.IdOffre,
        IdEtudiant = c.IdEtudiant,
        DateCandidature = c.DateCandidature,
        Statut = c.Statut,
        CvUrl = c.CvUrl,
        LettreMotivation = c.LettreMotivation
    };

    private static CandidatureResumeeResponse MapResumee(Candidature c) => new()
    {
        IdCandidature = c.IdCandidature,
        IdOffre = c.IdOffre,
        TitreOffre = c.Offre?.Titre ?? string.Empty,
        NomEtudiant = c.Etudiant?.Utilisateur?.Nom ?? string.Empty,
        PrenomEtudiant = c.Etudiant?.Utilisateur?.Prenom ?? string.Empty,
        CourrielEtudiant = c.Etudiant?.Utilisateur?.Courriel,
        Statut = c.Statut,
        DateCandidature = c.DateCandidature,
        ACV = c.Documents.Any(d => d.TypeDocument == TypeDocument.CV),
        ALettreMotivation = c.Documents.Any(d => d.TypeDocument == TypeDocument.LettreMotivation)
    };

    private static CandidatureDetailResponse MapDetail(Candidature c) => new()
    {
        IdCandidature = c.IdCandidature,
        IdOffre = c.IdOffre,
        TitreOffre = c.Offre?.Titre ?? string.Empty,
        NomEtudiant = c.Etudiant?.Utilisateur?.Nom ?? string.Empty,
        PrenomEtudiant = c.Etudiant?.Utilisateur?.Prenom ?? string.Empty,
        CourrielEtudiant = c.Etudiant?.Utilisateur?.Courriel,
        Statut = c.Statut,
        DateCandidature = c.DateCandidature,
        ACV = c.Documents.Any(d => d.TypeDocument == TypeDocument.CV),
        ALettreMotivation = c.Documents.Any(d => d.TypeDocument == TypeDocument.LettreMotivation),
        MessageMotivation = c.MessageMotivation ?? c.LettreMotivation,
        Documents = c.Documents.Select(d => new DocumentResponse
        {
            IdDocument = d.IdDocument,
            TypeDocument = d.TypeDocument,
            NomFichier = d.NomFichier,
            TailleFichier = d.TailleFichier,
            DateUpload = d.DateUpload
        }).ToList()
    };
}