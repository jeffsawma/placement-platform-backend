using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Candidatures;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories;
using SystemePlacement.Web.Repositories.Interfaces;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class CandidatureService : ICandidatureService
{
    private readonly ApplicationDbContext _context;
    private readonly ICandidatureRepository _repository;
    private readonly IOffreRepository _offreRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IWebHostEnvironment _env;
    private readonly INotificationService _notification;

    public CandidatureService(
        ApplicationDbContext context,
        ICandidatureRepository repository,
        IOffreRepository offreRepository,
        ICurrentUserService currentUser,
        IWebHostEnvironment env,
        INotificationService notification)
    {
        _context = context;
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

    public async Task<ValidationCandidatureResponse> ValiderPostulationAsync(int idOffre)
    {
        if (!_currentUser.IdUtilisateur.HasValue)
        {
            return new ValidationCandidatureResponse
            {
                PeutPostuler = false,
                Message = "La session de l'utilisateur connecte est invalide."
            };
        }

        var idEtudiant = await _repository.GetIdEtudiantByUtilisateurAsync(
            _currentUser.IdUtilisateur.Value);

        if (idEtudiant is null)
        {
            return new ValidationCandidatureResponse
            {
                PeutPostuler = false,
                Message = "Votre profil etudiant est introuvable."
            };
        }

        var offre = await _offreRepository.GetByIdAsync(idOffre);

        if (offre is null)
        {
            return new ValidationCandidatureResponse
            {
                PeutPostuler = false,
                Message = "L'offre selectionnee est introuvable."
            };
        }

        if (await _repository.ExistsAsync(idOffre, idEtudiant.Value))
        {
            return new ValidationCandidatureResponse
            {
                PeutPostuler = false,
                Message = "Vous avez deja postule a cette offre."
            };
        }

        return new ValidationCandidatureResponse
        {
            PeutPostuler = true
        };
    }

    public async Task<CandidatureResponse?> PostulerAsync(PostulerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CvUrl))
            return null;

        if (!_currentUser.IdUtilisateur.HasValue)
            return null;

        var idEtudiant = await _repository.GetIdEtudiantByUtilisateurAsync(
            _currentUser.IdUtilisateur.Value);

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
        {
            await _notification.NotifierEmployeurAsync(
                offre.IdEmployeur,
                $"Nouvelle candidature recue pour \"{offre.Titre}\".",
                "/employeur/candidatures");
        }

        return Map(candidature);
    }

    public async Task<bool> ChangerStatutAsync(int idCandidature, ChangerStatutRequest request)
    {
        return await ChangerStatutAsync(idCandidature, request.Statut, null);
    }

    public async Task<bool> ChangerStatutAsync(
        int idCandidature,
        StatutCandidature statut,
        string? message = null)
    {
        var candidature = await _repository.GetByIdAsync(idCandidature);

        if (candidature is null || candidature.EmploiConfirme)
            return false;

        candidature.Statut = statut;
        candidature.MessageReponseEmployeur = message;
        candidature.DateReponseEmployeur = DateTime.UtcNow;

        // Si une candidature de stage est acceptee, on cree automatiquement
        // un stage en attente de confirmation par l'employeur et le responsable.
        if (statut == StatutCandidature.Acceptee && candidature.Offre is OffreStage offreStage)
        {
            var stageExiste = await _context.Stages
                .AnyAsync(s =>
                    s.IdEtudiant == candidature.IdEtudiant &&
                    s.IdOffre == candidature.IdOffre);

            if (!stageExiste)
            {
                await _context.Stages.AddAsync(new Stage
                {
                    IdEtudiant = candidature.IdEtudiant,
                    IdOffre = candidature.IdOffre,
                    DateDebut = offreStage.DateDebutStage,
                    DateFin = offreStage.DateFinStage,
                    Lieu = offreStage.Ville,
                    Superviseur = null,
                    Statut = "EnAttente",
                    DateCreation = DateTime.UtcNow
                });
            }
        }

        _repository.Update(candidature);
        await _repository.SaveChangesAsync();

        var libelleStatut = statut switch
        {
            StatutCandidature.Vue => "a été consultée par l'employeur",
            StatutCandidature.Acceptee => "a été acceptée",
            StatutCandidature.Refusee => "a été refusée",
            StatutCandidature.Retiree => "a été retirée",
            _ => $"est maintenant : {statut}"
        };

        var titreOffre = candidature.Offre?.Titre ?? "une offre";

        await _notification.NotifierEtudiantAsync(
            candidature.IdEtudiant,
            $"Votre candidature pour « {titreOffre} » {libelleStatut}.",
            "/mes-candidatures");

        if (statut == StatutCandidature.Acceptee
            && candidature.Offre is not null
            && candidature.Etudiant?.Utilisateur?.IdCollege is int idCollegeEtudiant)
        {
            var nomEtudiant = $"{candidature.Etudiant.Utilisateur.Prenom} {candidature.Etudiant.Utilisateur.Nom}";
            var nomEmployeur = await _repository.GetNomEmployeurAsync(candidature.Offre.IdEmployeur) ?? "un employeur";
            var typePlacement = candidature.Offre is OffreStage ? "en stage" : "pour un emploi";

            await _notification.NotifierResponsablesCollegeAsync(
                idCollegeEtudiant,
                $"{nomEtudiant} a été accepté(e) {typePlacement} chez {nomEmployeur} pour « {titreOffre} ».",
                "/responsable/suivi-etudiants");
        }

        return true;
    }

    public async Task<bool> ConfirmerEmploiAsync(int idCandidature, string? message = null)
    {
        var candidature = await _repository.GetByIdAsync(idCandidature);

        if (candidature is null ||
            candidature.Offre is null ||
            candidature.Etudiant is null)
        {
            return false;
        }

        if (candidature.Offre is not OffreEmploi)
            return false;

        if (candidature.Statut != StatutCandidature.Acceptee)
            return false;

        if (candidature.EmploiConfirme)
            return false;

        if (_currentUser.Role == "Employeur")
        {
            if (!_currentUser.IdUtilisateur.HasValue)
                return false;

            var idEmployeur = await _offreRepository.GetIdEmployeurByUtilisateurAsync(
                _currentUser.IdUtilisateur.Value);

            if (idEmployeur is null ||
                candidature.Offre.IdEmployeur != idEmployeur.Value)
            {
                return false;
            }
        }

        candidature.EmploiConfirme = true;
        candidature.MessageConfirmationEmploi = string.IsNullOrWhiteSpace(message)
            ? "Emploi confirme par l'employeur."
            : message.Trim();

        candidature.DateConfirmationEmploi = DateTime.UtcNow;

        _repository.Update(candidature);
        await _repository.SaveChangesAsync();

        await _notification.NotifierUtilisateurAsync(
            candidature.Etudiant.IdUtilisateur,
            $"Votre embauche pour l'offre \"{candidature.Offre.Titre}\" a ete confirmee par l'employeur.",
            "/mes-candidatures");

        return true;
    }

    public async Task<IReadOnlyList<CandidatureResumeeResponse>> GetCandidaturesOffreAsync(int idOffre)
    {
        if (_currentUser.Role == "Employeur")
        {
            if (!_currentUser.IdUtilisateur.HasValue)
                return Array.Empty<CandidatureResumeeResponse>();

            var idEmployeur = await _offreRepository.GetIdEmployeurByUtilisateurAsync(
                _currentUser.IdUtilisateur.Value);

            var offre = await _offreRepository.GetByIdAsync(idOffre);

            if (offre is null || !idEmployeur.HasValue || offre.IdEmployeur != idEmployeur.Value)
                return Array.Empty<CandidatureResumeeResponse>();
        }

        var candidatures = await _repository.GetByOffreAsync(idOffre);
        return candidatures.Select(MapResumee).ToList();
    }

    public async Task<IReadOnlyList<CandidatureResumeeResponse>> GetCandidaturesParDomaineAsync(int idDomaine)
    {
        var candidatures = await _repository.GetByDomaineAsync(idDomaine);

        if (_currentUser.Role == "Employeur" && _currentUser.IdUtilisateur.HasValue)
        {
            var idEmployeur = await _offreRepository.GetIdEmployeurByUtilisateurAsync(
                _currentUser.IdUtilisateur.Value);

            if (idEmployeur is null)
                return Array.Empty<CandidatureResumeeResponse>();

            candidatures = candidatures
                .Where(c => c.Offre != null && c.Offre.IdEmployeur == idEmployeur.Value)
                .ToList();
        }

        return candidatures.Select(MapResumee).ToList();
    }

    public async Task<IReadOnlyList<CandidatureResumeeResponse>> GetMesCandidaturesAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
            return Array.Empty<CandidatureResumeeResponse>();

        var idEtudiant = await _repository.GetIdEtudiantByUtilisateurAsync(
            _currentUser.IdUtilisateur.Value);

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

        if (candidature is null ||
            candidature.IdEtudiant != idEtudiant.Value ||
            candidature.Statut != StatutCandidature.EnAttente)
        {
            return false;
        }

        candidature.Statut = StatutCandidature.Retiree;

        _repository.Update(candidature);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<CandidatureDetailResponse?> GetDetailAsync(int idCandidature)
    {
        var candidature = await _repository.GetByIdAsync(idCandidature);
        return candidature is null ? null : MapDetail(candidature);
    }

    public async Task<bool> ChangerStatutAsync(int idCandidature, StatutCandidature statut)
    {
        // Centralise la logique pour ne pas oublier la creation automatique du stage.
        return await ChangerStatutAsync(idCandidature, statut, null);
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

    private async Task<int?> IdEtudiantCourantAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
            return null;

        return await _repository.GetIdEtudiantByUtilisateurAsync(
            _currentUser.IdUtilisateur.Value);
    }

    private CandidatureDocument? CreerDocument(string? url, TypeDocument type)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;

        var fichier = Path.GetFileName(url);
        var nomFichier = fichier.Contains('_')
            ? fichier[(fichier.IndexOf('_') + 1)..]
            : fichier;

        var cheminComplet = Path.Combine(_env.WebRootPath, url.TrimStart('/', '\\'));
        var taille = File.Exists(cheminComplet)
            ? new FileInfo(cheminComplet).Length
            : 0;

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
        DateReponseEmployeur = c.DateReponseEmployeur,
        EmploiConfirme = c.EmploiConfirme,
        MessageConfirmationEmploi = c.MessageConfirmationEmploi,
        DateConfirmationEmploi = c.DateConfirmationEmploi
    };

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
        MessageMotivation = c.MessageMotivation ?? c.LettreMotivation,
        MessageReponseEmployeur = c.MessageReponseEmployeur,
        DateReponseEmployeur = c.DateReponseEmployeur,
        EmploiConfirme = c.EmploiConfirme,
        MessageConfirmationEmploi = c.MessageConfirmationEmploi,
        DateConfirmationEmploi = c.DateConfirmationEmploi,
        ACV = c.Documents.Any(d => d.TypeDocument == TypeDocument.CV) ||
              !string.IsNullOrWhiteSpace(c.CvUrl),
        ALettreMotivation = c.Documents.Any(d => d.TypeDocument == TypeDocument.LettreMotivation) ||
                             !string.IsNullOrWhiteSpace(c.LettreMotivation)
    };

    private static CandidatureDetailResponse MapDetail(Candidature c) => new()
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
        EmploiConfirme = c.EmploiConfirme,
        MessageConfirmationEmploi = c.MessageConfirmationEmploi,
        DateConfirmationEmploi = c.DateConfirmationEmploi,
        ACV = c.Documents.Any(d => d.TypeDocument == TypeDocument.CV) ||
              !string.IsNullOrWhiteSpace(c.CvUrl),
        ALettreMotivation = c.Documents.Any(d => d.TypeDocument == TypeDocument.LettreMotivation) ||
                             !string.IsNullOrWhiteSpace(c.LettreMotivation),
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
