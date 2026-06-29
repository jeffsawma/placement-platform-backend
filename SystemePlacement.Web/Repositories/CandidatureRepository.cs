using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;

namespace SystemePlacement.Web.Repositories;

public class CandidatureRepository : ICandidatureRepository
{
    private readonly ApplicationDbContext _context;

    public CandidatureRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<Candidature>> GetByOffreAsync(int idOffre) =>
        _context.Candidatures
            .AsNoTracking()
            .Include(c => c.Offre)
            .Include(c => c.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Include(c => c.Documents)
            .Where(c => c.IdOffre == idOffre)
            .OrderByDescending(c => c.DateCandidature)
            .ToListAsync();

    public Task<List<Candidature>> GetByEtudiantAsync(int idEtudiant) =>
        _context.Candidatures
            .AsNoTracking()
            .Include(c => c.Offre)
            .Include(c => c.Documents)
            .Where(c => c.IdEtudiant == idEtudiant)
            .OrderByDescending(c => c.DateCandidature)
            .ToListAsync();

    public Task<Candidature?> GetByIdAsync(int idCandidature) =>
        _context.Candidatures
            .Include(c => c.Offre)
            .Include(c => c.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.IdCandidature == idCandidature);

    public Task<bool> ExistsAsync(int idOffre, int idEtudiant) =>
        _context.Candidatures
            .AnyAsync(c => c.IdOffre == idOffre && c.IdEtudiant == idEtudiant);

    public Task<int?> GetIdEtudiantByUtilisateurAsync(int idUtilisateur) =>
        _context.Etudiants
            .Where(e => e.IdUtilisateur == idUtilisateur)
            .Select(e => (int?)e.IdEtudiant)
            .FirstOrDefaultAsync();

    public async Task AddAsync(Candidature candidature)
    {
        await _context.Candidatures.AddAsync(candidature);
    }

    public Task<CandidatureDocument?> GetDocumentAsync(int idDocument) =>
        _context.CandidatureDocuments
            .Include(d => d.Candidature)
            .FirstOrDefaultAsync(d => d.IdDocument == idDocument);

    public void Update(Candidature candidature)
    {
        _context.Candidatures.Update(candidature);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}