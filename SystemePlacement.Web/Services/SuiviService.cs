using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Suivis;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class SuiviService : ISuiviService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SuiviService(ApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<EtudiantSuiviResponseDto>> GetEtudiantsSuivisAsync()
    {
        // Le responsable doit etre rattache a un college.
        // Sans college, il ne peut suivre aucun etudiant.
        if (!_currentUser.IdCollege.HasValue)
        {
            return Array.Empty<EtudiantSuiviResponseDto>();
        }

        var idCollege = _currentUser.IdCollege.Value;

        // On retourne seulement les etudiants actifs du meme college que le responsable.
        return await _context.Etudiants
            .AsNoTracking()
            .Include(e => e.Utilisateur)
                .ThenInclude(u => u!.College)
            .Where(e =>
                e.Utilisateur != null &&
                e.Utilisateur.IdCollege == idCollege &&
                e.Utilisateur.Actif)
            .OrderBy(e => e.Utilisateur!.Nom)
            .ThenBy(e => e.Utilisateur!.Prenom)
            .Select(e => new EtudiantSuiviResponseDto
            {
                IdEtudiant = e.IdEtudiant,
                IdUtilisateur = e.IdUtilisateur,
                Prenom = e.Utilisateur!.Prenom,
                Nom = e.Utilisateur.Nom,
                Courriel = e.Utilisateur.Courriel,
                IdCollege = e.Utilisateur.IdCollege,
                NomCollege = e.Utilisateur.College != null
                    ? e.Utilisateur.College.Nom
                    : null,

                // Calcule le nombre total de candidatures de l'etudiant.
                NombreCandidatures = _context.Candidatures
                    .Count(c => c.IdEtudiant == e.IdEtudiant),

                // Recupere le dernier statut de candidature.
                DernierStatutCandidature = _context.Candidatures
                    .Where(c => c.IdEtudiant == e.IdEtudiant)
                    .OrderByDescending(c => c.DateCandidature)
                    .Select(c => c.Statut.ToString())
                    .FirstOrDefault(),

                // Recupere la date de la derniere candidature.
                DateDerniereCandidature = _context.Candidatures
                    .Where(c => c.IdEtudiant == e.IdEtudiant)
                    .OrderByDescending(c => c.DateCandidature)
                    .Select(c => (DateTime?)c.DateCandidature)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }
}