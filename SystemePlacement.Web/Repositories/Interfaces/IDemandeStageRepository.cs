using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Repositories.Interfaces;

public interface IDemandeStageRepository
{
    // Trouve le profil étudiant lié à l'utilisateur connecté.
    Task<int?> GetIdEtudiantByUtilisateurAsync(int idUtilisateur);

    // Trouve le collège de l'étudiant.
    Task<int?> GetIdCollegeEtudiantAsync(int idEtudiant);

    // Vérifie que le domaine est disponible pour le collège de l'étudiant.
    Task<bool> DomaineExistePourCollegeAsync(int idDomaine, int idCollege);

    Task AddAsync(DemandeStage demande);

    Task<DemandeStage?> GetByIdAsync(int idDemandeStage);

    Task<List<DemandeStage>> GetByEtudiantAsync(int idEtudiant);

    Task<List<DemandeStage>> GetByDomaineAsync(int idDomaine);

    Task<bool> EmployeurHasOfferInDomainAsync(int idUtilisateur, int idDomaine);

    Task<string?> GetNomDomaineAsync(int idDomaine);

    Task<string?> GetNomEtudiantAsync(int idEtudiant);

    Task<List<int>> GetIdsEmployeursByDomaineAsync(int idDomaine);

    Task SaveChangesAsync();
}