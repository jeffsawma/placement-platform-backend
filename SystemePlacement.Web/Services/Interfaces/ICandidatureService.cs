using SystemePlacement.Web.DTOs.Candidatures;
using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.Services.Interfaces;

public interface ICandidatureService
{
    // US-10 : liste des candidatures d'une offre
    Task<IReadOnlyList<CandidatureResumeeResponse>> GetCandidaturesOffreAsync(int idOffre);

    // Detail d'une candidature
    Task<CandidatureDetailResponse?> GetDetailAsync(int idCandidature);

    // US-10 : changer le statut d'une candidature
    Task<bool> ChangerStatutAsync(int idCandidature, StatutCandidature statut);

    // US-12 : obtenir le fichier pour telechargement
    Task<(byte[] Contenu, string ContentType, string NomFichier)?> TelechargerDocumentAsync(int idDocument);
}
