using SystemePlacement.Web.DTOs.Recommandations;

namespace SystemePlacement.Web.Services.Interfaces;

public interface IRecommandationService
{
    Task<IReadOnlyList<RecommandationResponse>> GetByEtudiantAsync(int idEtudiant);
    Task<RecommandationResponse?> CreerAsync(CreerRecommandationRequest request, IFormFile? lettre);
    Task<(byte[] Contenu, string ContentType, string NomFichier)?> TelechargerLettreAsync(int idRecommandation);
    Task<bool> SupprimerAsync(int idRecommandation);
}
