using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;

namespace SystemePlacement.Web.Repositories.Interfaces;

public interface IOffreRepository
{
    Task<List<Offre>> SearchAsync(TypeOffre? type, int? idDomaine, string? lieu, string? motsCles);
    Task<Offre?> GetByIdAsync(int idOffre);
}
