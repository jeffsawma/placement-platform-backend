using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;

namespace SystemePlacement.Web.Repositories;

public class DemandeStageRepository : IDemandeStageRepository
{
    private readonly ApplicationDbContext _context;

    public DemandeStageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int?> GetIdEtudiantByUtilisateurAsync(int idUtilisateur) =>
        _context.Etudiants
            .Where(e => e.IdUtilisateur == idUtilisateur)
            .Select(e => (int?)e.IdEtudiant)
            .FirstOrDefaultAsync();

    public async Task AddAsync(DemandeStage demande) =>
        await _context.DemandesStage.AddAsync(demande);

    public Task<List<DemandeStage>> GetByEtudiantAsync(int idEtudiant) =>
        _context.DemandesStage
            .AsNoTracking()
            .Include(d => d.DomaineEtude)
            .Where(d => d.IdEtudiant == idEtudiant)
            .OrderByDescending(d => d.DateCreation)
            .ToListAsync();

    public Task<List<DemandeStage>> GetByDomaineAsync(int idDomaine) =>
        _context.DemandesStage
            .AsNoTracking()
            .Include(d => d.DomaineEtude)
            .Include(d => d.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Where(d => d.IdDomaine == idDomaine)
            .OrderByDescending(d => d.DateCreation)
            .ToListAsync();

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
