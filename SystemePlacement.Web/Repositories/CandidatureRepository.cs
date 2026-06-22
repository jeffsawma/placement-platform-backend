using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;

namespace SystemePlacement.Web.Repositories;

public class CandidatureRepository : ICandidatureRepository
{
    private readonly ApplicationDbContext _context;

    public CandidatureRepository(ApplicationDbContext context) => _context = context;

    public Task<List<Candidature>> GetByOffreAsync(int idOffre) =>
        _context.Candidatures
            .AsNoTracking()
            .Where(c => c.IdOffre == idOffre)
            .ToListAsync();

    public Task<Candidature?> GetByIdAsync(int idCandidature) =>
        _context.Candidatures.FirstOrDefaultAsync(c => c.IdCandidature == idCandidature);

    public Task<bool> ExistsAsync(int idOffre, int idEtudiant) =>
        _context.Candidatures.AnyAsync(c => c.IdOffre == idOffre && c.IdEtudiant == idEtudiant);

    public Task<int?> GetIdEtudiantByUtilisateurAsync(int idUtilisateur) =>
        _context.Etudiants
            .Where(e => e.IdUtilisateur == idUtilisateur)
            .Select(e => (int?)e.IdEtudiant)
            .FirstOrDefaultAsync();

    public async Task AddAsync(Candidature candidature) =>
        await _context.Candidatures.AddAsync(candidature);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
