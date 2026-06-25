using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Repositories.Interfaces;

public interface ICandidatureRepository
{
    // US-10 : Candidatures d'une offre
    Task<List<Candidature>> GetByOffreAsync(int idOffre);

    // Detail d'une candidature
    Task<Candidature?> GetByIdAsync(int idCandidature);

    // US-12 : Document d'une candidature
    Task<CandidatureDocument?> GetDocumentAsync(int idDocument);

    // Changer statut
    void Update(Candidature candidature);

    Task SaveChangesAsync();
}
