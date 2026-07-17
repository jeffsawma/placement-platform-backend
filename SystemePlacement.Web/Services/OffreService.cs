using SystemePlacement.Web.DTOs.Offres;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class OffreService : IOffreService
{
    private readonly IOffreRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public OffreService(IOffreRepository repository, ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<OffreResumeeResponse>> GetAllAsync(
        TypeOffre? type,
        StatutOffre? statut,
        int? idDomaine = null,
        string? lieu = null,
        string? motsCles = null)
    {
        // Liste publique : utilisee pour la recherche d'offres.
        var offres = await _repository.GetAllAsync(type, statut, idDomaine, lieu, motsCles);
        return offres.Select(MapResumee).ToList();
    }

    public async Task<IReadOnlyList<OffreResumeeResponse>> GetMesOffresAsync()
    {
        // Les admins peuvent voir toutes les offres.
        if (_currentUser.Role == "Administrateur" || _currentUser.Role == "SuperAdministrateur")
        {
            var toutesLesOffres = await _repository.GetAllAsync();
            return toutesLesOffres.Select(MapResumee).ToList();
        }

        // Un employeur voit seulement les offres reliees a son profil employeur.
        var idEmployeur = await ObtenirIdEmployeurAsync();

        if (idEmployeur is null)
        {
            return Array.Empty<OffreResumeeResponse>();
        }

        var offres = await _repository.GetByEmployeurAsync(idEmployeur.Value);
        return offres.Select(MapResumee).ToList();
    }

    public async Task<object?> GetByIdAsync(int idOffre)
    {
        var offre = await _repository.GetByIdAsync(idOffre);
        if (offre is null) return null;

        return offre switch
        {
            OffreEmploi emploi => MapEmploi(emploi),
            OffreStage stage => MapStage(stage),
            _ => MapResumee(offre)
        };
    }

    public async Task<OffreEmploiResponse?> CreerEmploiAsync(CreerOffreEmploiRequest req)
    {
        var idEmployeur = await ObtenirIdEmployeurAsync();
        if (idEmployeur is null) return null;

        var offre = new OffreEmploi
        {
            Titre = req.Titre,
            Description = req.Description,
            Ville = req.Ville,
            Adresse = req.Adresse,
            TypeOffre = TypeOffre.Emploi,
            Statut = StatutOffre.Active,
            DatePublication = DateTime.UtcNow,
            DateExpiration = req.DateExpiration,
            IdEmployeur = idEmployeur.Value,
            TypeContrat = req.TypeContrat,
            SalaireMin = req.SalaireMin,
            SalaireMax = req.SalaireMax,
            TeleTravail = req.TeleTravail
        };

        await _repository.AddAsync(offre);
        await _repository.SaveChangesAsync();

        await SynchroniserDomainesAsync(offre.IdOffre, req.IdsDomaines);

        var result = await _repository.GetByIdAsync(offre.IdOffre) as OffreEmploi;
        return result is null ? null : MapEmploi(result);
    }

    public async Task<OffreStageResponse?> CreerStageAsync(CreerOffreStageRequest req)
    {
        var idEmployeur = await ObtenirIdEmployeurAsync();
        if (idEmployeur is null) return null;

        var offre = new OffreStage
        {
            Titre = req.Titre,
            Description = req.Description,
            Ville = req.Ville,
            Adresse = req.Adresse,
            TypeOffre = TypeOffre.Stage,
            Statut = StatutOffre.Active,
            DatePublication = DateTime.UtcNow,
            DateExpiration = req.DateExpiration,
            IdEmployeur = idEmployeur.Value,
            DateDebutStage = req.DateDebutStage,
            DateFinStage = req.DateFinStage,
            DureeHeuresParSemaine = req.DureeHeuresParSemaine,
            Remuneration = req.Remuneration,
            Session = req.Session
        };

        await _repository.AddAsync(offre);
        await _repository.SaveChangesAsync();

        await SynchroniserDomainesAsync(offre.IdOffre, req.IdsDomaines);

        var result = await _repository.GetByIdAsync(offre.IdOffre) as OffreStage;
        return result is null ? null : MapStage(result);
    }

    public async Task<OffreEmploiResponse?> ModifierEmploiAsync(int idOffre, ModifierOffreEmploiRequest req)
    {
        var offre = await _repository.GetByIdAsync(idOffre) as OffreEmploi;
        if (offre is null) return null;

        if (!await EstProprietaireOuAdmin(offre)) return null;

        offre.Titre = req.Titre;
        offre.Description = req.Description;
        offre.Ville = req.Ville;
        offre.Adresse = req.Adresse;
        offre.DateExpiration = req.DateExpiration;
        offre.Statut = req.Statut;
        offre.TypeContrat = req.TypeContrat;
        offre.SalaireMin = req.SalaireMin;
        offre.SalaireMax = req.SalaireMax;
        offre.TeleTravail = req.TeleTravail;

        _repository.Update(offre);
        await SynchroniserDomainesAsync(idOffre, req.IdsDomaines);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(idOffre) as OffreEmploi;
        return result is null ? null : MapEmploi(result);
    }

    public async Task<OffreStageResponse?> ModifierStageAsync(int idOffre, ModifierOffreStageRequest req)
    {
        var offre = await _repository.GetByIdAsync(idOffre) as OffreStage;
        if (offre is null) return null;

        if (!await EstProprietaireOuAdmin(offre)) return null;

        offre.Titre = req.Titre;
        offre.Description = req.Description;
        offre.Ville = req.Ville;
        offre.Adresse = req.Adresse;
        offre.DateExpiration = req.DateExpiration;
        offre.Statut = req.Statut;
        offre.DateDebutStage = req.DateDebutStage;
        offre.DateFinStage = req.DateFinStage;
        offre.DureeHeuresParSemaine = req.DureeHeuresParSemaine;
        offre.Remuneration = req.Remuneration;
        offre.Session = req.Session;

        _repository.Update(offre);
        await SynchroniserDomainesAsync(idOffre, req.IdsDomaines);
        await _repository.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(idOffre) as OffreStage;
        return result is null ? null : MapStage(result);
    }

    public async Task<bool> SupprimerAsync(int idOffre)
    {
        var offre = await _repository.GetByIdAsync(idOffre);
        if (offre is null) return false;

        if (!await EstProprietaireOuAdmin(offre)) return false;

        _repository.Delete(offre);
        await _repository.SaveChangesAsync();
        return true;
    }

    private async Task<int?> ObtenirIdEmployeurAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue) return null;

        return await _repository.GetIdEmployeurByUtilisateurAsync(_currentUser.IdUtilisateur.Value);
    }

    private async Task<bool> EstProprietaireOuAdmin(Offre offre)
    {
        // Les admins peuvent modifier toutes les offres.
        if (_currentUser.Role == "Administrateur" || _currentUser.Role == "SuperAdministrateur")
        {
            return true;
        }

        // Un employeur peut modifier seulement ses propres offres.
        var idEmployeur = await ObtenirIdEmployeurAsync();
        return idEmployeur.HasValue && offre.IdEmployeur == idEmployeur.Value;
    }

    private async Task SynchroniserDomainesAsync(int idOffre, List<int> idsDomaines)
    {
        var existants = await _repository.GetDomainesOffreAsync(idOffre);
        _repository.RemoveDomaines(existants);

        if (idsDomaines.Count > 0)
        {
            var nouveaux = idsDomaines.Select(idDomaine => new OffreDomaine
            {
                IdOffre = idOffre,
                IdDomaine = idDomaine
            });

            await _repository.AddDomainesAsync(nouveaux);
        }

        await _repository.SaveChangesAsync();
    }

    private static OffreResumeeResponse MapResumee(Offre o) => new()
    {
        IdOffre = o.IdOffre,
        Titre = o.Titre,
        Ville = o.Ville,
        TypeOffre = o.TypeOffre,
        Statut = o.Statut,
        DatePublication = o.DatePublication,
        DateExpiration = o.DateExpiration,
        NomEmployeur = o.Employeur?.Utilisateur is { } u
            ? $"{u.Prenom} {u.Nom}"
            : string.Empty,
        Domaines = o.OffreDomaines
            .Select(od => od.DomaineEtude?.Nom ?? string.Empty)
            .Where(n => n != string.Empty)
            .ToList()
    };

    private static OffreEmploiResponse MapEmploi(OffreEmploi o) => new()
    {
        IdOffre = o.IdOffre,
        Titre = o.Titre,
        Description = o.Description,
        Ville = o.Ville,
        Adresse = o.Adresse,
        TypeOffre = o.TypeOffre,
        Statut = o.Statut,
        DatePublication = o.DatePublication,
        DateExpiration = o.DateExpiration,
        NomEmployeur = o.Employeur?.Utilisateur is { } u
            ? $"{u.Prenom} {u.Nom}"
            : string.Empty,
        Domaines = o.OffreDomaines
            .Select(od => od.DomaineEtude?.Nom ?? string.Empty)
            .Where(n => n != string.Empty)
            .ToList(),
        TypeContrat = o.TypeContrat,
        SalaireMin = o.SalaireMin,
        SalaireMax = o.SalaireMax,
        TeleTravail = o.TeleTravail
    };

    private static OffreStageResponse MapStage(OffreStage o) => new()
    {
        IdOffre = o.IdOffre,
        Titre = o.Titre,
        Description = o.Description,
        Ville = o.Ville,
        Adresse = o.Adresse,
        TypeOffre = o.TypeOffre,
        Statut = o.Statut,
        DatePublication = o.DatePublication,
        DateExpiration = o.DateExpiration,
        NomEmployeur = o.Employeur?.Utilisateur is { } u
            ? $"{u.Prenom} {u.Nom}"
            : string.Empty,
        Domaines = o.OffreDomaines
            .Select(od => od.DomaineEtude?.Nom ?? string.Empty)
            .Where(n => n != string.Empty)
            .ToList(),
        DateDebutStage = o.DateDebutStage,
        DateFinStage = o.DateFinStage,
        DureeHeuresParSemaine = o.DureeHeuresParSemaine,
        Remuneration = o.Remuneration,
        Session = o.Session
    };
}