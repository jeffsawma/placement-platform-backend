using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Repositories.Interfaces;

public interface IOffreRepository
{
    // Lecture
    Task<List<Offre>> GetAllAsync(TypeOffre? type = null, StatutOffre? statut = null, int? idDomaine = null, string? lieu = null, string? motsCles = null);
    Task<List<Offre>> GetByEmployeurAsync(int idEmployeur);
    Task<Offre?> GetByIdAsync(int idOffre);
    Task<int?> GetIdEmployeurByUtilisateurAsync(int idUtilisateur);

    // Écriture
    Task AddAsync(Offre offre);
    void Update(Offre offre);
    void Delete(Offre offre);
    Task SaveChangesAsync();

    // Domaines (relation N-N)
    Task<List<OffreDomaine>> GetDomainesOffreAsync(int idOffre);
    void RemoveDomaines(IEnumerable<OffreDomaine> domaines);
    Task AddDomainesAsync(IEnumerable<OffreDomaine> domaines);
}
