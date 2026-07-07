using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;

namespace SystemePlacement.Web.Repositories;

public class OffreStageDirecteRepository : IOffreStageDirecteRepository
{
    private readonly ApplicationDbContext _context;

    public OffreStageDirecteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<OffreStageDirecte>> GetByEmployeurAsync(int idEmployeur) =>
        _context.OffresStageDirectes
            .AsNoTracking()
            .Include(o => o.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Where(o => o.IdEmployeur == idEmployeur)
            .OrderByDescending(o => o.DateProposition)
            .ToListAsync();

    public Task<OffreStageDirecte?> GetByIdAsync(int idOffreDirecte) =>
        _context.OffresStageDirectes
            .Include(o => o.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Include(o => o.Employeur)
            .FirstOrDefaultAsync(o => o.IdOffreDirecte == idOffreDirecte);

    public Task<bool> EtudiantExistsAsync(int idEtudiant) =>
        _context.Etudiants.AnyAsync(e => e.IdEtudiant == idEtudiant);

    public async Task AddAsync(OffreStageDirecte offreStageDirecte)
    {
        await _context.OffresStageDirectes.AddAsync(offreStageDirecte);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
