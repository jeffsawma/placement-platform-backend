using SystemePlacement.Web.DTOs.Offres;
using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.Services.Interfaces;

public interface IOffreService
{
    // US-07 : Lister
    Task<IReadOnlyList<OffreResumeeResponse>> GetAllAsync(TypeOffre? type, StatutOffre? statut);

    // US-07 : Détail
    Task<object?> GetByIdAsync(int idOffre);

    // US-07 : Créer emploi
    Task<OffreEmploiResponse?> CreerEmploiAsync(CreerOffreEmploiRequest request);

    // US-07 : Créer stage
    Task<OffreStageResponse?> CreerStageAsync(CreerOffreStageRequest request);

    // US-08 : Modifier emploi
    Task<OffreEmploiResponse?> ModifierEmploiAsync(int idOffre, ModifierOffreEmploiRequest request);

    // US-09 : Modifier stage
    Task<OffreStageResponse?> ModifierStageAsync(int idOffre, ModifierOffreStageRequest request);

    // US-10 : Supprimer
    Task<bool> SupprimerAsync(int idOffre);
}
    // Liste publique des offres, utilisee pour la recherche etudiant.
    Task<IReadOnlyList<OffreResumeeResponse>> GetAllAsync(
        TypeOffre? type,
        StatutOffre? statut,
        int? idDomaine = null,
        string? lieu = null,
        string? motsCles = null);

    // Liste privee : employeur = ses offres seulement, admin = toutes les offres.
    Task<IReadOnlyList<OffreResumeeResponse>> GetMesOffresAsync();

    Task<object?> GetByIdAsync(int idOffre);

    Task<OffreEmploiResponse?> CreerEmploiAsync(CreerOffreEmploiRequest request);

    Task<OffreStageResponse?> CreerStageAsync(CreerOffreStageRequest request);

    Task<OffreEmploiResponse?> ModifierEmploiAsync(int idOffre, ModifierOffreEmploiRequest request);

    Task<OffreStageResponse?> ModifierStageAsync(int idOffre, ModifierOffreStageRequest request);

    Task<bool> SupprimerAsync(int idOffre);
}
