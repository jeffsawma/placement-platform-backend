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
                .ThenInclude(dom => dom!.College)
            .Include(d => d.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Where(d => d.IdDomaine == idDomaine)
            .OrderByDescending(d => d.DateCreation)
            .ToListAsync();

    public Task<string?> GetNomDomaineAsync(int idDomaine) =>
        _context.DomainesEtudes
            .Where(d => d.IdDomaine == idDomaine)
            .Select(d => d.Nom)
            .FirstOrDefaultAsync();

    public Task<int?> GetIdCollegeByDomaineAsync(int idDomaine) =>
        _context.DomainesEtudes
            .Where(d => d.IdDomaine == idDomaine)
            .Select(d => (int?)d.IdCollege)
            .FirstOrDefaultAsync();

    public Task<string?> GetNomEtudiantAsync(int idEtudiant) =>
        _context.Etudiants
            .Where(e => e.IdEtudiant == idEtudiant)
            .Select(e => e.Utilisateur != null
                ? e.Utilisateur.Prenom + " " + e.Utilisateur.Nom
                : null)
            .FirstOrDefaultAsync();

    public Task<List<int>> GetIdsEmployeursByDomaineAsync(int idDomaine) =>
        _context.OffreDomaines
            .Where(od => od.IdDomaine == idDomaine && od.Offre != null)
            .Select(od => od.Offre!.IdEmployeur)
            .Distinct()
            .ToListAsync();

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
