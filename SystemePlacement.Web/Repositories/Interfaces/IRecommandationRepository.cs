using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Repositories.Interfaces;

public interface IRecommandationRepository
{
    Task<List<Recommandation>> GetByEtudiantAsync(int idEtudiant);
    Task<Recommandation?> GetByIdAsync(int idRecommandation);
    Task AddAsync(Recommandation recommandation);
    void Delete(Recommandation recommandation);
    Task SaveChangesAsync();
}
