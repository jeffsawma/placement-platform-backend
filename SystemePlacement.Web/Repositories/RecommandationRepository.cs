using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;

namespace SystemePlacement.Web.Repositories;

public class RecommandationRepository : IRecommandationRepository
{
    private readonly ApplicationDbContext _context;

    public RecommandationRepository(ApplicationDbContext context) => _context = context;

    public Task<List<Recommandation>> GetByEtudiantAsync(int idEtudiant) =>
        _context.Recommandations
            .AsNoTracking()
            .Include(r => r.Auteur)
            .Include(r => r.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Where(r => r.IdEtudiant == idEtudiant)
            .OrderByDescending(r => r.DateCreation)
            .ToListAsync();

    public Task<Recommandation?> GetByIdAsync(int idRecommandation) =>
        _context.Recommandations
            .Include(r => r.Auteur)
            .Include(r => r.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .FirstOrDefaultAsync(r => r.IdRecommandation == idRecommandation);

    public async Task AddAsync(Recommandation recommandation) =>
        await _context.Recommandations.AddAsync(recommandation);

    public void Delete(Recommandation recommandation) =>
        _context.Recommandations.Remove(recommandation);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
