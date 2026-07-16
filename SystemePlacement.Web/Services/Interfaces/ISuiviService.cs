using SystemePlacement.Web.DTOs.Suivis;

namespace SystemePlacement.Web.Services.Interfaces;

public interface ISuiviService
{
    Task<IReadOnlyList<EtudiantSuiviResponseDto>> GetEtudiantsSuivisAsync();

    Task<EtudiantSuiviDetailResponseDto?> GetEtudiantSuiviDetailAsync(int idEtudiant);

    Task<DemarcheSuiviResponseDto?> AjouterDemarcheAsync(
        int idEtudiant,
        DemarcheSuiviCreateDto request);

   
    Task<IReadOnlyList<DemarcheSuiviResponseDto>> GetMesDemarchesAsync();
}