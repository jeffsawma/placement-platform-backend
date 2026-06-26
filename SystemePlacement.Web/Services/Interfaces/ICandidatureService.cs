
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
    Task<CandidatureDetailResponse?> GetDetailAsync(int idCandidature);
    Task<bool> ChangerStatutAsync(int idCandidature, StatutCandidature statut);
    Task<(byte[] Contenu, string ContentType, string NomFichier)?> TelechargerDocumentAsync(int idDocument);
}