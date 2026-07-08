using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Repositories.Interfaces;
public interface IOffreStageDirecteRepository
{
    Task<List<OffreStageDirecte>> GetByEmployeurAsync(int idEmployeur); // Obtenir toutes les offres de stage directes reliées à un employeur spécifique
    Task<OffreStageDirecte?> GetByIdAsync(int idOffreDirecte); // Obtenir une offre de stage directe par son ID si elle existe
    Task<bool> EtudiantExistsAsync(int idEtudiant); // Vérifier si un étudiant existe dans la base de données
    Task<int?> GetIdEtudiantByUtilisateurAsync(int idUtilisateur);
    Task<List<OffreStageDirecte>> GetByEtudiantAsync(int idEtudiant);
    Task AddAsync(OffreStageDirecte offreStageDirecte); // Ajouter une nouvelle offre de stage directe à la base de données
    Task SaveChangesAsync();
}

