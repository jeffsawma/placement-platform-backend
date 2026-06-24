using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;

namespace SystemePlacement.Web.Repositories;

public class OffreRepository : IOffreRepository
{
    private readonly ApplicationDbContext _context;

    public OffreRepository(ApplicationDbContext context) => _context = context;

    public Task<List<Offre>> SearchAsync(TypeOffre? type, int? idDomaine, string? lieu, string? motsCles)
    {
        var query = _context.Set<Offre>()
            .AsNoTracking()
            .Where(o => o.Statut == StatutOffre.Publiee);

        if (type is not null)
            query = query.Where(o => o.TypeOffre == type);

        if (idDomaine is not null)
            query = query.Where(o => o.OffreDomaines.Any(od => od.IdDomaine == idDomaine));

        if (!string.IsNullOrWhiteSpace(lieu))
            query = query.Where(o => o.Lieu != null && o.Lieu.Contains(lieu));

        if (!string.IsNullOrWhiteSpace(motsCles))
            query = query.Where(o => o.Titre.Contains(motsCles) || o.Description.Contains(motsCles));

        return query
            .OrderByDescending(o => o.DatePublication)
            .ToListAsync();
    }

    public Task<Offre?> GetByIdAsync(int idOffre) =>
        _context.Set<Offre>().FirstOrDefaultAsync(o => o.IdOffre == idOffre);
}
