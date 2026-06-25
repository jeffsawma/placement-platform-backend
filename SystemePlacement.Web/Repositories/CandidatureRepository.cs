using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.Models;
using SystemePlacement.Web.Repositories.Interfaces;

namespace SystemePlacement.Web.Repositories;

public class CandidatureRepository : ICandidatureRepository
{
    private readonly ApplicationDbContext _context;

    public CandidatureRepository(ApplicationDbContext context) => _context = context;

    // US-10 : toutes les candidatures d'une offre
    public Task<List<Candidature>> GetByOffreAsync(int idOffre) =>
        _context.Candidatures
            .AsNoTracking()
            .Include(c => c.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Include(c => c.Documents)
            .Where(c => c.IdOffre == idOffre)
            .OrderByDescending(c => c.DateCandidature)
            .ToListAsync();

    // Detail complet d'une candidature
    public Task<Candidature?> GetByIdAsync(int idCandidature) =>
        _context.Candidatures
            .Include(c => c.Etudiant)
                .ThenInclude(e => e!.Utilisateur)
            .Include(c => c.Offre)
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.IdCandidature == idCandidature);

    // US-12 : document specifique pour telechargement
    public Task<CandidatureDocument?> GetDocumentAsync(int idDocument) =>
        _context.CandidatureDocuments
            .Include(d => d.Candidature)
            .FirstOrDefaultAsync(d => d.IdDocument == idDocument);

    public void Update(Candidature candidature) =>
        _context.Candidatures.Update(candidature);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
