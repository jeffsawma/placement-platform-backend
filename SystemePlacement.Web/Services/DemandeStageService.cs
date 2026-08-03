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

        // On valide que le domaine existe et qu'il est disponible pour le college de l'etudiant.
        var idCollegeEtudiant = await _repository.GetIdCollegeEtudiantAsync(idEtudiant.Value);
        if (idCollegeEtudiant is null)
            return null;

        var domaineDisponible = await _repository.DomaineExistePourCollegeAsync(
            request.IdDomaine,
            idCollegeEtudiant.Value);

        if (!domaineDisponible)
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
        var nomEtudiant = await _repository.GetNomEtudiantAsync(idEtudiant.Value) ?? "Un etudiant";

        // Les responsables du college de l'etudiant sont notifies.
        await _notification.NotifierResponsablesCollegeAsync(
            idCollegeEtudiant.Value,
            $"{nomEtudiant} a formule une demande de stage en « {nomDomaine} ».",
            "/responsable/suivi-etudiants");

        // Les employeurs qui ont deja des offres dans ce domaine sont notifies.
        var idsEmployeurs = await _repository.GetIdsEmployeursByDomaineAsync(request.IdDomaine);
        foreach (var idEmployeur in idsEmployeurs)
        {
            await _notification.NotifierEmployeurAsync(
                idEmployeur,
                $"{nomEtudiant} recherche un stage en « {nomDomaine} », un domaine de vos offres.",
                "/employeur/demandes-stage");
        }

        var demandeComplete = await _repository.GetByIdAsync(demande.IdDemandeStage);

        return Map(demandeComplete ?? demande);
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
        if (_currentUser.Role == "Employeur")
        {
            if (!_currentUser.IdUtilisateur.HasValue)
                return Array.Empty<DemandeStageResponse>();

            var hasAccess = await _repository.EmployeurHasOfferInDomainAsync(
                _currentUser.IdUtilisateur.Value,
                idDomaine);

            if (!hasAccess)
                return Array.Empty<DemandeStageResponse>();
        }

        var demandes = await _repository.GetByDomaineAsync(idDomaine);
        return demandes.Select(Map).ToList();
    }

    private static DemandeStageResponse Map(DemandeStage d) => new()
    {
        IdDemandeStage = d.IdDemandeStage,
        IdDomaine = d.IdDomaine,
        NomDomaine = d.DomaineEtude?.Nom ?? string.Empty,

        // Le college vient maintenant de l'etudiant, pas du domaine.
        NomCollege = d.Etudiant?.Utilisateur?.College?.Nom ?? string.Empty,

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