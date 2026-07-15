using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Suivis;
using SystemePlacement.Web.Models;
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
        if (!_currentUser.IdCollege.HasValue)
        {
            return Array.Empty<EtudiantSuiviResponseDto>();
        }

        var idCollege = _currentUser.IdCollege.Value;

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

                NombreCandidatures = _context.Candidatures
                    .Count(c => c.IdEtudiant == e.IdEtudiant),

                DernierStatutCandidature = _context.Candidatures
                    .Where(c => c.IdEtudiant == e.IdEtudiant)
                    .OrderByDescending(c => c.DateCandidature)
                    .Select(c => c.Statut.ToString())
                    .FirstOrDefault(),

                DateDerniereCandidature = _context.Candidatures
                    .Where(c => c.IdEtudiant == e.IdEtudiant)
                    .OrderByDescending(c => c.DateCandidature)
                    .Select(c => (DateTime?)c.DateCandidature)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }

    public async Task<EtudiantSuiviDetailResponseDto?> GetEtudiantSuiviDetailAsync(int idEtudiant)
    {
        if (!_currentUser.IdCollege.HasValue)
        {
            return null;
        }

        var idCollege = _currentUser.IdCollege.Value;

        var etudiant = await _context.Etudiants
            .AsNoTracking()
            .Include(e => e.Utilisateur)
                .ThenInclude(u => u!.College)
            .FirstOrDefaultAsync(e =>
                e.IdEtudiant == idEtudiant &&
                e.Utilisateur != null &&
                e.Utilisateur.IdCollege == idCollege &&
                e.Utilisateur.Actif);

        if (etudiant == null || etudiant.Utilisateur == null)
        {
            return null;
        }

        var candidatures = await _context.Candidatures
            .AsNoTracking()
            .Include(c => c.Offre)
            .Where(c => c.IdEtudiant == idEtudiant)
            .OrderByDescending(c => c.DateCandidature)
            .Select(c => new CandidatureSuiviDto
            {
                IdCandidature = c.IdCandidature,
                IdOffre = c.IdOffre,
                TitreOffre = c.Offre != null ? c.Offre.Titre : string.Empty,
                TypeOffre = c.Offre != null ? c.Offre.TypeOffre.ToString() : string.Empty,
                Statut = c.Statut.ToString(),
                DateCandidature = c.DateCandidature
            })
            .ToListAsync();

        var demarches = await _context.DemarchesSuivi
            .AsNoTracking()
            .Where(d => d.IdEtudiant == idEtudiant)
            .OrderByDescending(d => d.DateDemarche)
            .Select(d => new DemarcheSuiviResponseDto
            {
                IdDemarche = d.IdDemarche,
                IdEtudiant = d.IdEtudiant,
                IdResponsable = d.IdResponsable,
                TypeDemarche = d.TypeDemarche,
                Note = d.Note,
                VisibleEtudiant = d.VisibleEtudiant,
                DateDemarche = d.DateDemarche
            })
            .ToListAsync();

        return new EtudiantSuiviDetailResponseDto
        {
            IdEtudiant = etudiant.IdEtudiant,
            IdUtilisateur = etudiant.IdUtilisateur,
            Prenom = etudiant.Utilisateur.Prenom,
            Nom = etudiant.Utilisateur.Nom,
            Courriel = etudiant.Utilisateur.Courriel,
            Programme = etudiant.Programme,
            Telephone = etudiant.Telephone,
            NomCollege = etudiant.Utilisateur.College?.Nom,
            Candidatures = candidatures,
            Demarches = demarches
        };
    }

    public async Task<DemarcheSuiviResponseDto?> AjouterDemarcheAsync(
        int idEtudiant,
        DemarcheSuiviCreateDto request)
    {
        if (!_currentUser.IdCollege.HasValue)
        {
            return null;
        }

        var idCollege = _currentUser.IdCollege.Value;

        var etudiantExiste = await _context.Etudiants
            .Include(e => e.Utilisateur)
            .AnyAsync(e =>
                e.IdEtudiant == idEtudiant &&
                e.Utilisateur != null &&
                e.Utilisateur.IdCollege == idCollege &&
                e.Utilisateur.Actif);

        if (!etudiantExiste)
        {
            return null;
        }

        var responsable = await _context.ResponsablesStage
            .FirstOrDefaultAsync(r => r.IdUtilisateur == _currentUser.IdUtilisateur);

        if (responsable == null)
        {
            return null;
        }

        var demarche = new DemarcheSuivi
        {
            IdEtudiant = idEtudiant,
            IdResponsable = responsable.IdResponsable,
            TypeDemarche = request.TypeDemarche,
            Note = request.Note,
            VisibleEtudiant = request.VisibleEtudiant,
            DateDemarche = DateTime.UtcNow
        };

        await _context.DemarchesSuivi.AddAsync(demarche);
        await _context.SaveChangesAsync();

        return new DemarcheSuiviResponseDto
        {
            IdDemarche = demarche.IdDemarche,
            IdEtudiant = demarche.IdEtudiant,
            IdResponsable = demarche.IdResponsable,
            TypeDemarche = demarche.TypeDemarche,
            Note = demarche.Note,
            VisibleEtudiant = demarche.VisibleEtudiant,
            DateDemarche = demarche.DateDemarche
        };
    }

    public async Task<IReadOnlyList<DemarcheSuiviResponseDto>> GetMesDemarchesAsync()
    {
        if (!_currentUser.IdUtilisateur.HasValue)
        {
            return Array.Empty<DemarcheSuiviResponseDto>();
        }

        var etudiant = await _context.Etudiants
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.IdUtilisateur == _currentUser.IdUtilisateur.Value);

        if (etudiant == null)
        {
            return Array.Empty<DemarcheSuiviResponseDto>();
        }

        return await _context.DemarchesSuivi
            .AsNoTracking()
            .Where(d => d.IdEtudiant == etudiant.IdEtudiant && d.VisibleEtudiant)
            .OrderByDescending(d => d.DateDemarche)
            .Select(d => new DemarcheSuiviResponseDto
            {
                IdDemarche = d.IdDemarche,
                IdEtudiant = d.IdEtudiant,
                IdResponsable = d.IdResponsable,
                TypeDemarche = d.TypeDemarche,
                Note = d.Note,
                VisibleEtudiant = d.VisibleEtudiant,
                DateDemarche = d.DateDemarche
            })
            .ToListAsync();
    }
}