using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Repositories.Interfaces;

public interface ICandidatureRepository
{
    Task<List<Candidature>> GetByOffreAsync(int idOffre);
    Task<List<Candidature>> GetByEtudiantAsync(int idEtudiant);
    Task<List<Candidature>> GetByDomaineAsync(int idDomaine);
    Task<Candidature?> GetByIdAsync(int idCandidature);

    Task<bool> ExistsAsync(int idOffre, int idEtudiant);
    Task<int?> GetIdEtudiantByUtilisateurAsync(int idUtilisateur);
    Task AddAsync(Candidature candidature);

    Task<CandidatureDocument?> GetDocumentAsync(int idDocument);
    void Update(Candidature candidature);

    Task SaveChangesAsync();
}