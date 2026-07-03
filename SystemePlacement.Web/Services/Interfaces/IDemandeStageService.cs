using SystemePlacement.Web.DTOs.DemandesStage;

namespace SystemePlacement.Web.Services.Interfaces;

public interface IDemandeStageService
{
    Task<DemandeStageResponse?> CreerAsync(CreerDemandeStageRequest request);
    Task<IReadOnlyList<DemandeStageResponse>> GetMesDemandesAsync();
}
