using SystemePlacement.Web.DTOs.OffresStageDirectes;

namespace SystemePlacement.Web.Services.Interfaces;

public interface IOffreStageDirecteService
{
    Task<IReadOnlyList<OffreStageDirecteReponse>> GetMesOffresAsync(); // Récupère toutes les offres de stage directes de l'utilisateur connecté
    Task<OffreStageDirecteReponse?> GetAsync(int idOffreDirecte); // Récupère une offre de stage directe par son ID si elle appartient à l'utilisateur connecté
    Task<OffreStageDirecteReponse?> CreerAsync(CreerOffreStageDirecteRequest request); // Crée une nouvelle offre de stage directe pour l'utilisateur connecté
    Task<IReadOnlyList<OffreStageDirecteReponse>> GetMesOffresRecuesAsync(); // US-21 : offres reçues par l'étudiant connecté
    Task<bool> RepondreAsync(int idOffreDirecte, RepondreOffreDirecteRequest request); // US-21 : l'étudiant accepte ou refuse
}
