using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Repositories.Interfaces;

public interface IDemandeStageRepository
{
    Task<int?> GetIdEtudiantByUtilisateurAsync(int idUtilisateur);
    Task AddAsync(DemandeStage demande);
    Task<List<DemandeStage>> GetByEtudiantAsync(int idEtudiant);
    Task SaveChangesAsync();
}
