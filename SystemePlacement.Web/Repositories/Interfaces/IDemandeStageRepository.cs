using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Repositories.Interfaces;

public interface IDemandeStageRepository
{
    Task<int?> GetIdEtudiantByUtilisateurAsync(int idUtilisateur);
    Task AddAsync(DemandeStage demande);
    Task<DemandeStage?> GetByIdAsync(int idDemandeStage);
    Task<List<DemandeStage>> GetByEtudiantAsync(int idEtudiant);
    Task<List<DemandeStage>> GetByDomaineAsync(int idDomaine);
    Task<bool> EmployeurHasOfferInDomainAsync(int idUtilisateur, int idDomaine);
    Task<string?> GetNomDomaineAsync(int idDomaine);
    Task<int?> GetIdCollegeByDomaineAsync(int idDomaine);
    Task<string?> GetNomEtudiantAsync(int idEtudiant);
    Task<List<int>> GetIdsEmployeursByDomaineAsync(int idDomaine);
    Task SaveChangesAsync();
}
