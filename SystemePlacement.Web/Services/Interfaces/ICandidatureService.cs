using SystemePlacement.Web.DTOs.Candidatures;

namespace SystemePlacement.Web.Services.Interfaces;

public interface ICandidatureService
{
    Task<IReadOnlyList<CandidatureResponse>> GetParOffreAsync(int idOffre);
    Task<CandidatureResponse?> GetAsync(int idCandidature);
    Task<CandidatureResponse?> PostulerAsync(PostulerRequest request);
    Task<bool> ChangerStatutAsync(int idCandidature, ChangerStatutRequest request);
}
