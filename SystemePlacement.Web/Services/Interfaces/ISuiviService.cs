using SystemePlacement.Web.DTOs.Suivis;

namespace SystemePlacement.Web.Services.Interfaces;

public interface ISuiviService
{
    Task<IReadOnlyList<EtudiantSuiviResponseDto>> GetEtudiantsSuivisAsync();
}