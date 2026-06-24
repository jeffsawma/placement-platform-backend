using SystemePlacement.Web.DTOs.Offres;

namespace SystemePlacement.Web.Services.Interfaces;

public interface IOffreService
{
    Task<IReadOnlyList<OffreResponse>> RechercherAsync(RechercheOffresQuery query);
    Task<OffreResponse?> GetAsync(int idOffre);
    Task<OffreStatutResponse?> GetStatutAsync(int idOffre);
}
