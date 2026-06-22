using SystemePlacement.Web.DTOs.Candidatures;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class CandidatureService : ICandidatureService
{
    private readonly ICandidatureRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CandidatureService(ICandidatureRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<CandidatureResponse>> GetParOffreAsync(int idOffre)
    {
        var candidatures = await _repository.GetByOffreAsync(idOffre);
        return candidatures.Select(Map).ToList();
    }

    public async Task<CandidatureResponse?> GetAsync(int idCandidature)
    {
        var candidature = await _repository.GetByIdAsync(idCandidature);
        return candidature is null ? null : Map(candidature);
    }

    public async Task<CandidatureResponse?> PostulerAsync(PostulerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CvUrl))
            return null;

        if (!_currentUser.IdUtilisateur.HasValue)
            return null;

        var idEtudiant = await _repository.GetIdEtudiantByUtilisateurAsync(_currentUser.IdUtilisateur.Value);
        if (idEtudiant is null)
            return null;

        if (await _repository.ExistsAsync(request.IdOffre, idEtudiant.Value))
            return null;

        var candidature = new Candidature
        {
            IdOffre = request.IdOffre,
            IdEtudiant = idEtudiant.Value,
            CvUrl = request.CvUrl,
            LettreMotivation = request.LettreMotivation,
            DateCandidature = DateTime.UtcNow,
            Statut = StatutCandidature.EnAttente
        };

        await _repository.AddAsync(candidature);
        await _repository.SaveChangesAsync();
        return Map(candidature);
    }

    public async Task<bool> ChangerStatutAsync(int idCandidature, ChangerStatutRequest request)
    {
        var candidature = await _repository.GetByIdAsync(idCandidature);
        if (candidature is null)
            return false;

        candidature.Statut = request.Statut;
        await _repository.SaveChangesAsync();
        return true;
    }

    private static CandidatureResponse Map(Candidature c) => new()
    {
        IdCandidature = c.IdCandidature,
        IdOffre = c.IdOffre,
        IdEtudiant = c.IdEtudiant,
        DateCandidature = c.DateCandidature,
        Statut = c.Statut,
        CvUrl = c.CvUrl,
        LettreMotivation = c.LettreMotivation
    };
}
