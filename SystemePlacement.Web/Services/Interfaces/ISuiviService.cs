using SystemePlacement.Web.DTOs.Suivis;

namespace SystemePlacement.Web.Services.Interfaces;

public interface ISuiviService
{
    // Retourne les etudiants que le responsable de stage peut suivre.
    Task<IReadOnlyList<EtudiantSuiviResponseDto>> GetEtudiantsSuivisAsync();

    // Retourne le detail d'un etudiant suivi.
    Task<EtudiantSuiviDetailResponseDto?> GetEtudiantSuiviDetailAsync(int idEtudiant);

    // Ajoute une demarche ou note de suivi pour un etudiant.
    Task<DemarcheSuiviResponseDto?> AjouterDemarcheAsync(
        int idEtudiant,
        DemarcheSuiviCreateDto request);
}