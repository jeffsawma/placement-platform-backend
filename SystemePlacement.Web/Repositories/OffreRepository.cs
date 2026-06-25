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

    // ── Lecture ──────────────────────────────────────────────────────────────

    public Task<List<Offre>> GetAllAsync(TypeOffre? type = null, StatutOffre? statut = null)
    {
        IQueryable<Offre> query = _context.Offres
            .AsNoTracking()
            .Include(o => o.Employeur)
                .ThenInclude(e => e!.Utilisateur)
            .Include(o => o.OffreDomaines)
                .ThenInclude(od => od.DomaineEtude);

        if (type.HasValue)
            query = query.Where(o => o.TypeOffre == type.Value);

        if (statut.HasValue)
            query = query.Where(o => o.Statut == statut.Value);

        return query
            .OrderByDescending(o => o.DatePublication)
            .ToListAsync();
    }

    public Task<List<Offre>> GetByEmployeurAsync(int idEmployeur) =>
        _context.Offres
            .AsNoTracking()
            .Include(o => o.OffreDomaines)
                .ThenInclude(od => od.DomaineEtude)
            .Where(o => o.IdEmployeur == idEmployeur)
            .OrderByDescending(o => o.DatePublication)
            .ToListAsync();

    public Task<Offre?> GetByIdAsync(int idOffre) =>
        _context.Offres
            .Include(o => o.Employeur)
                .ThenInclude(e => e!.Utilisateur)
            .Include(o => o.OffreDomaines)
                .ThenInclude(od => od.DomaineEtude)
            .FirstOrDefaultAsync(o => o.IdOffre == idOffre);

    public Task<int?> GetIdEmployeurByUtilisateurAsync(int idUtilisateur) =>
        _context.Employeurs
            .Where(e => e.IdUtilisateur == idUtilisateur)
            .Select(e => (int?)e.IdEmployeur)
            .FirstOrDefaultAsync();

    // ── Écriture ──────────────────────────────────────────────────────────────

    public async Task AddAsync(Offre offre) => await _context.Offres.AddAsync(offre);

    public void Update(Offre offre) => _context.Offres.Update(offre);

    public void Delete(Offre offre) => _context.Offres.Remove(offre);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();

    // ── Domaines ─────────────────────────────────────────────────────────────

    public Task<List<OffreDomaine>> GetDomainesOffreAsync(int idOffre) =>
        _context.Set<OffreDomaine>()
            .Where(od => od.IdOffre == idOffre)
            .ToListAsync();

    public void RemoveDomaines(IEnumerable<OffreDomaine> domaines) =>
        _context.Set<OffreDomaine>().RemoveRange(domaines);

    public async Task AddDomainesAsync(IEnumerable<OffreDomaine> domaines) =>
        await _context.Set<OffreDomaine>().AddRangeAsync(domaines);
}
