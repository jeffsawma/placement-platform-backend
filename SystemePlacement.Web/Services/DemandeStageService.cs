using SystemePlacement.Web.DTOs.DemandesStage;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class DemandeStageService : IDemandeStageService
{
    private readonly IDemandeStageRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notification;

    public DemandeStageService(
        IDemandeStageRepository repository,
        ICurrentUserService currentUser,
        INotificationService notification)
    {
        _repository = repository;
        _currentUser = currentUser;
        _notification = notification;
    }

    public async Task<DemandeStageResponse?> CreerAsync(CreerDemandeStageRequest request)
    {
        if (!_currentUser.IdUtilisateur.HasValue)
            return null;

        var idEtudiant = await _repository.GetIdEtudiantByUtilisateurAsync(_currentUser.IdUtilisateur.Value);
        if (idEtudiant is null)
            return null;

        var demande = new DemandeStage
        {
            IdEtudiant = idEtudiant.Value,
            IdDomaine = request.IdDomaine,
            Description = request.Description,
            PeriodeSouhaitee = request.PeriodeSouhaitee,
            Competences = request.Competences,
            Statut = StatutDemandeStage.Ouverte,
            DateCreation = DateTime.UtcNow
        };

        await _repository.AddAsync(demande);
        await _repository.SaveChangesAsync();

        var nomDomaine = await _repository.GetNomDomaineAsync(request.IdDomaine) ?? "un domaine";
        var nomEtudiant = await _repository.GetNomEtudiantAsync(idEtudiant.Value) ?? "Un étudiant";
        var idCollege = await _repository.GetIdCollegeByDomaineAsync(request.IdDomaine);
        if (idCollege.HasValue)
            await _notification.NotifierResponsablesCollegeAsync(
                idCollege.Value,
                $"{nomEtudiant} a formulé une demande de stage en « {nomDomaine} ».");

        return Map(demande);
    }

    public async Task<IReadOnlyList<DemandeStageResponse>> GetMesDemandesAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
            return Array.Empty<DemandeStageResponse>();

        var idEtudiant = await _repository.GetIdEtudiantByUtilisateurAsync(_currentUser.IdUtilisateur.Value);
        if (idEtudiant is null)
            return Array.Empty<DemandeStageResponse>();

        var demandes = await _repository.GetByEtudiantAsync(idEtudiant.Value);
        return demandes.Select(Map).ToList();
    }

    public async Task<IReadOnlyList<DemandeStageResponse>> GetDemandesParDomaineAsync(int idDomaine)
    {
        var demandes = await _repository.GetByDomaineAsync(idDomaine);
        return demandes.Select(Map).ToList();
    }

    private static DemandeStageResponse Map(DemandeStage d) => new()
    {
        IdDemandeStage = d.IdDemandeStage,
        IdDomaine = d.IdDomaine,
        NomDomaine = d.DomaineEtude?.Nom ?? string.Empty,
        NomCollege = d.DomaineEtude?.College?.Nom ?? string.Empty,
        NomEtudiant = d.Etudiant?.Utilisateur?.Nom ?? string.Empty,
        PrenomEtudiant = d.Etudiant?.Utilisateur?.Prenom ?? string.Empty,
        CourrielEtudiant = d.Etudiant?.Utilisateur?.Courriel,
        Description = d.Description,
        PeriodeSouhaitee = d.PeriodeSouhaitee,
        Competences = d.Competences,
        Statut = d.Statut,
        DateCreation = d.DateCreation
    };
}
