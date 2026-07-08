using SystemePlacement.Web.DTOs.OffresStageDirectes;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class OffreStageDirecteService : IOffreStageDirecteService
{
    private readonly IOffreStageDirecteRepository _repository;
    private readonly IOffreRepository _offreRepository;
    private readonly ICurrentUserService _currentUser;

    public OffreStageDirecteService(
        IOffreStageDirecteRepository repository,
        IOffreRepository offreRepository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _offreRepository = offreRepository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<OffreStageDirecteReponse>> GetMesOffresAsync()
    {
        var idEmployeur = await GetIdEmployeurCourantAsync();
        if (idEmployeur is null)
            return Array.Empty<OffreStageDirecteReponse>();

        var offres = await _repository.GetByEmployeurAsync(idEmployeur.Value);
        return offres.Select(Map).ToList();
    }

    public async Task<OffreStageDirecteReponse?> GetAsync(int idOffreDirecte)
    {
        var idEmployeur = await GetIdEmployeurCourantAsync();
        if (idEmployeur is null)
            return null;

        var offre = await _repository.GetByIdAsync(idOffreDirecte);
        if (offre is null || offre.IdEmployeur != idEmployeur.Value)
            return null;

        return Map(offre);
    }

    public async Task<OffreStageDirecteReponse?> CreerAsync(CreerOffreStageDirecteRequest request)
    {
        var idEmployeur = await GetIdEmployeurCourantAsync();
        if (idEmployeur is null)
            return null;

        if (!await _repository.EtudiantExistsAsync(request.IdEtudiant))
            return null;

        if (string.IsNullOrWhiteSpace(request.Conditions))
            return null;

        var offre = new OffreStageDirecte
        {
            IdEmployeur = idEmployeur.Value,
            IdEtudiant = request.IdEtudiant,
            IdOffreStage = request.IdOffreStage,
            IdCandidature = request.IdCandidature,
            IdDemandeStage = request.IdDemandeStage,
            Conditions = request.Conditions,
            DateDebutProposee = request.DateDebutProposee,
            DateFinProposee = request.DateFinProposee,
            DateProposition = DateTime.UtcNow,
            Statut = StatutOffreStageDirecte.Envoyee,
            Commentaire = request.Commentaire
        };

        await _repository.AddAsync(offre);
        await _repository.SaveChangesAsync();

        var saved = await _repository.GetByIdAsync(offre.IdOffreDirecte);
        return saved is null ? Map(offre) : Map(saved);
    }

    public async Task<IReadOnlyList<OffreStageDirecteReponse>> GetMesOffresRecuesAsync()
    {
        var idEtudiant = await GetIdEtudiantCourantAsync();
        if (idEtudiant is null)
            return Array.Empty<OffreStageDirecteReponse>();

        var offres = await _repository.GetByEtudiantAsync(idEtudiant.Value);
        return offres.Select(Map).ToList();
    }

    public async Task<bool> RepondreAsync(int idOffreDirecte, RepondreOffreDirecteRequest request)
    {
        var idEtudiant = await GetIdEtudiantCourantAsync();
        if (idEtudiant is null)
            return false;

        var offre = await _repository.GetByIdAsync(idOffreDirecte);
        if (offre is null || offre.IdEtudiant != idEtudiant.Value)
            return false;

        if (offre.Statut != StatutOffreStageDirecte.Envoyee)
            return false;

        offre.Statut = request.Accepte ? StatutOffreStageDirecte.Acceptee : StatutOffreStageDirecte.Refusee;
        offre.ReponseEtudiant = request.Reponse;

        await _repository.SaveChangesAsync();
        return true;
    }

    private async Task<int?> GetIdEtudiantCourantAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
            return null;

        return await _repository.GetIdEtudiantByUtilisateurAsync(_currentUser.IdUtilisateur.Value);
    }

    private async Task<int?> GetIdEmployeurCourantAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
            return null;

        return await _offreRepository.GetIdEmployeurByUtilisateurAsync(_currentUser.IdUtilisateur.Value);
    }

    private static OffreStageDirecteReponse Map(OffreStageDirecte offre) => new()
    {
        IdOffreDirecte = offre.IdOffreDirecte,

        NomEtudiant = offre.Etudiant?.Utilisateur?.Nom ?? string.Empty,
        PrenomEtudiant = offre.Etudiant?.Utilisateur?.Prenom ?? string.Empty,
        CourrielEtudiant = offre.Etudiant?.Utilisateur?.Courriel,

        IdOffreStage = offre.IdOffreStage,
        IdCandidature = offre.IdCandidature,
        IdDemandeStage = offre.IdDemandeStage,

        Conditions = offre.Conditions,

        DateDebutProposee = offre.DateDebutProposee,
        DateFinProposee = offre.DateFinProposee,

        DateProposition = offre.DateProposition,

        Statut = offre.Statut,
        Commentaire = offre.Commentaire,
        ReponseEtudiant = offre.ReponseEtudiant
    };
}
