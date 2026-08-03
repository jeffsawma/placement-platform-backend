using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Repositories.Interfaces;

public interface IDemandeStageRepository
{
    // Trouve le profil etudiant lie a l'utilisateur connecte.
    Task<int?> GetIdEtudiantByUtilisateurAsync(int idUtilisateur);

    // Trouve le college de l'etudiant.
    Task<int?> GetIdCollegeEtudiantAsync(int idEtudiant);

    // Verifie que le domaine est disponible pour le college de l'etudiant.
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