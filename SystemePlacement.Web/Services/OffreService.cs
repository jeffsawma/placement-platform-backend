using SystemePlacement.Web.DTOs.Offres;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class OffreService : IOffreService
{
    private readonly IOffreRepository _repository;

    public OffreService(IOffreRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<OffreResponse>> RechercherAsync(RechercheOffresQuery query)
    {
        var offres = await _repository.SearchAsync(query.Type, query.IdDomaine, query.Lieu, query.MotsCles);
        return offres.Select(Map).ToList();
    }

    public async Task<OffreResponse?> GetAsync(int idOffre)
    {
        var offre = await _repository.GetByIdAsync(idOffre);
        return offre is null ? null : Map(offre);
    }

    public async Task<OffreStatutResponse?> GetStatutAsync(int idOffre)
    {
        var offre = await _repository.GetByIdAsync(idOffre);
        return offre is null
            ? null
            : new OffreStatutResponse { IdOffre = offre.IdOffre, Statut = offre.Statut };
    }

    private static OffreResponse Map(Offre o) => new()
    {
        IdOffre = o.IdOffre,
        Titre = o.Titre,
        Description = o.Description,
        Lieu = o.Lieu,
        TypeOffre = o.TypeOffre,
        Statut = o.Statut,
        IdEntreprise = o.IdEntreprise,
        NombrePostes = o.NombrePostes,
        Remunere = o.Remunere,
        DatePublication = o.DatePublication,
        DateExpiration = o.DateExpiration
    };
}
