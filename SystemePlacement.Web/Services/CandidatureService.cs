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
    private readonly INotificationService _notification;

    public CandidatureService(
        ICandidatureRepository repository,
        IOffreRepository offreRepository,
        ICurrentUserService currentUser,
        IWebHostEnvironment env,
        INotificationService notification)
    {
        _repository = repository;
        _offreRepository = offreRepository;
        _currentUser = currentUser;
        _env = env;
        _notification = notification;
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

        var cvDocument = CreerDocument(request.CvUrl, TypeDocument.CV);
        if (cvDocument is not null)
            candidature.Documents.Add(cvDocument);

        var lettreDocument = CreerDocument(request.LettreUrl, TypeDocument.LettreMotivation);
        if (lettreDocument is not null)
            candidature.Documents.Add(lettreDocument);

        await _repository.AddAsync(candidature);
        await _repository.SaveChangesAsync();

        var offre = await _offreRepository.GetByIdAsync(candidature.IdOffre);
        if (offre is not null)
            await _notification.NotifierEmployeurAsync(offre.IdEmployeur, $"Nouvelle candidature reçue pour « {offre.Titre} ».");

        return Map(candidature);
    }

    public async Task<bool> ChangerStatutAsync(int idCandidature, ChangerStatutRequest request)
    {
        return await ChangerStatutAsync(idCandidature, request.Statut, null); // null pour le message, car il n'est pas fourni dans cette surcharge
    }

    // Cette surcharge permet de modifier le statut et, �ventuellement, de fournir un message
    public async Task<bool> ChangerStatutAsync(int idCandidature, StatutCandidature statut, string? message = null)
    {
        var candidature = await _repository.GetByIdAsync(idCandidature);
        if (candidature is null)
            return false;

        candidature.Statut = statut;
        candidature.MessageReponseEmployeur = message;
        candidature.DateReponseEmployeur = DateTime.UtcNow;

        _repository.Update(candidature);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ConfirmerEmploiAsync(int idCandidature, string? message = null)
    {
        var candidature = await _repository.GetByIdAsync(idCandidature);

        if (candidature is null || candidature.Offre is null)
            return false;

        // US-16 concerne uniquement les offres d'emploi
        if (candidature.Offre is not OffreEmploi)
            return false;

        if (_currentUser.Role == "Employeur")
        {
            if (!_currentUser.IdUtilisateur.HasValue)
                return false;

            var idEmployeur = await _offreRepository.GetIdEmployeurByUtilisateurAsync(_currentUser.IdUtilisateur.Value);

            if (idEmployeur is null || candidature.Offre.IdEmployeur != idEmployeur.Value)
                return false;
        }

        candidature.Statut = StatutCandidature.Acceptee; // Changer le statut de la candidature � "Accept�e"
        candidature.MessageReponseEmployeur = string.IsNullOrWhiteSpace(message)
            ? "Emploi confirm� par l'employeur."
            : message;

        candidature.DateReponseEmployeur = DateTime.UtcNow;

        _repository.Update(candidature);
        await _repository.SaveChangesAsync();

        return true;
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
        return candidatures.Select(MapResumee).ToList();
    }

    public async Task<IReadOnlyList<CandidatureResumeeResponse>> GetMesCandidaturesAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
            return Array.Empty<CandidatureResumeeResponse>();

        var idEtudiant = await _repository.GetIdEtudiantByUtilisateurAsync(_currentUser.IdUtilisateur.Value);
        if (idEtudiant is null)
            return Array.Empty<CandidatureResumeeResponse>();

        var candidatures = await _repository.GetByEtudiantAsync(idEtudiant.Value);
        return candidatures.Select(MapResumee).ToList();
    }

    public async Task<bool> MettreAJourAsync(int idCandidature, MettreAJourCandidatureRequest request)
    {
        var idEtudiant = await IdEtudiantCourantAsync();
        if (idEtudiant is null)
            return false;

        var candidature = await _repository.GetByIdAsync(idCandidature);
        if (candidature is null || candidature.IdEtudiant != idEtudiant.Value)
            return false;

        if (candidature.Statut != StatutCandidature.EnAttente)
            return false;

        candidature.MessageMotivation = request.Message;
        candidature.LettreMotivation = request.Message;

        _repository.Update(candidature);
        await _repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RetirerAsync(int idCandidature)
    {
        var idEtudiant = await IdEtudiantCourantAsync();
        if (idEtudiant is null)
            return false;

        var candidature = await _repository.GetByIdAsync(idCandidature);
        if (candidature is null || candidature.IdEtudiant != idEtudiant.Value)
            return false;

        candidature.Statut = StatutCandidature.Retiree;
        _repository.Update(candidature);
        await _repository.SaveChangesAsync();
        return true;
    }

    private async Task<int?> IdEtudiantCourantAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
            return null;

        return await _repository.GetIdEtudiantByUtilisateurAsync(_currentUser.IdUtilisateur.Value);
    }

    public async Task<IReadOnlyList<CandidatureResumeeResponse>> GetCandidaturesParDomaineAsync(int idDomaine)
    {
        var candidatures = await _repository.GetByDomaineAsync(idDomaine);

        if (_currentUser.Role == "Employeur" && _currentUser.IdUtilisateur.HasValue)
        {
            var idEmployeur = await _offreRepository.GetIdEmployeurByUtilisateurAsync(_currentUser.IdUtilisateur.Value);
            if (idEmployeur is null)
                return Array.Empty<CandidatureResumeeResponse>();

            candidatures = candidatures.Where(c => c.Offre!.IdEmployeur == idEmployeur.Value).ToList();
        }

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

    private CandidatureDocument? CreerDocument(string? url, TypeDocument type)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var fichier = Path.GetFileName(url);
        var nomFichier = fichier.Contains('_') ? fichier[(fichier.IndexOf('_') + 1)..] : fichier;
        var cheminComplet = Path.Combine(_env.WebRootPath, url.TrimStart('/', '\\'));
        var taille = File.Exists(cheminComplet) ? new FileInfo(cheminComplet).Length : 0;

        return new CandidatureDocument
        {
            TypeDocument = type,
            CheminFichier = url,
            NomFichier = nomFichier,
            ContentType = TypeContenu(Path.GetExtension(nomFichier)),
            TailleFichier = taille,
            DateUpload = DateTime.UtcNow
        };
    }

    private static string TypeContenu(string extension) => extension.ToLowerInvariant() switch
    {
        ".pdf" => "application/pdf",
        ".doc" => "application/msword",
        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        _ => "application/octet-stream"
    };

    private static CandidatureResponse Map(Candidature c) => new()
    {
        IdCandidature = c.IdCandidature,
        IdOffre = c.IdOffre,
        IdEtudiant = c.IdEtudiant,
        DateCandidature = c.DateCandidature,
        Statut = c.Statut,
        CvUrl = c.CvUrl,
        LettreMotivation = c.LettreMotivation,
        MessageMotivation = c.MessageMotivation,
        MessageReponseEmployeur = c.MessageReponseEmployeur,
        DateReponseEmployeur = c.DateReponseEmployeur
    };

    // Ajout de l'id de l'étudiant pour permettre la création d'une offre de stage directe
    private static CandidatureResumeeResponse MapResumee(Candidature c) => new()
    {
        IdCandidature = c.IdCandidature,
        IdOffre = c.IdOffre,
        IdEtudiant = c.IdEtudiant,
        TitreOffre = c.Offre?.Titre ?? string.Empty,
        NomEtudiant = c.Etudiant?.Utilisateur?.Nom ?? string.Empty,
        PrenomEtudiant = c.Etudiant?.Utilisateur?.Prenom ?? string.Empty,
        CourrielEtudiant = c.Etudiant?.Utilisateur?.Courriel,
        Statut = c.Statut,
        DateCandidature = c.DateCandidature,
        MessageReponseEmployeur = c.MessageReponseEmployeur,
        DateReponseEmployeur = c.DateReponseEmployeur,
        ACV = c.Documents.Any(d => d.TypeDocument == TypeDocument.CV) || !string.IsNullOrWhiteSpace(c.CvUrl),
        ALettreMotivation = c.Documents.Any(d => d.TypeDocument == TypeDocument.LettreMotivation) || !string.IsNullOrWhiteSpace(c.LettreMotivation)
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
        MessageReponseEmployeur = c.MessageReponseEmployeur,
        DateReponseEmployeur = c.DateReponseEmployeur,
        ACV = c.Documents.Any(d => d.TypeDocument == TypeDocument.CV) || !string.IsNullOrWhiteSpace(c.CvUrl),
        ALettreMotivation = c.Documents.Any(d => d.TypeDocument == TypeDocument.LettreMotivation) || !string.IsNullOrWhiteSpace(c.LettreMotivation),
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
