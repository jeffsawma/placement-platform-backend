
using SystemePlacement.Web.DTOs.Candidatures;
using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.Services.Interfaces;

public interface ICandidatureService
{
    Task<IReadOnlyList<CandidatureResponse>> GetParOffreAsync(int idOffre);
    Task<CandidatureResponse?> GetAsync(int idCandidature);
    Task<CandidatureResponse?> PostulerAsync(PostulerRequest request);
    Task<bool> ChangerStatutAsync(int idCandidature, ChangerStatutRequest request);

    Task<IReadOnlyList<CandidatureResumeeResponse>> GetCandidaturesOffreAsync(int idOffre);
    Task<IReadOnlyList<CandidatureResumeeResponse>> GetCandidaturesParDomaineAsync(int idDomaine);
    Task<IReadOnlyList<CandidatureResumeeResponse>> GetMesCandidaturesAsync();
    Task<bool> MettreAJourAsync(int idCandidature, MettreAJourCandidatureRequest request);
    Task<bool> RetirerAsync(int idCandidature);
    Task<CandidatureDetailResponse?> GetDetailAsync(int idCandidature);
    Task<bool> ChangerStatutAsync(int idCandidature, StatutCandidature statut, string? message = null);
    Task<(byte[] Contenu, string ContentType, string NomFichier)?> TelechargerDocumentAsync(int idDocument);
}
