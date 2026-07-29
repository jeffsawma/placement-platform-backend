using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Etudiants;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/etudiants")]
[Authorize]
public class EtudiantsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EtudiantsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET /api/etudiants
    // Sert a permettre a un employeur de choisir un etudiant pour une offre de stage directe.
    [HttpGet]
    [Authorize(Roles = "Employeur,ResponsableStage,Administrateur,SuperAdministrateur")]
    public async Task<ActionResult<IEnumerable<EtudiantSelectDto>>> GetEtudiants()
    {
        var etudiants = await _context.Etudiants
            .AsNoTracking()
            .Include(e => e.Utilisateur)
                .ThenInclude(u => u!.College)
            .Where(e =>
                e.Utilisateur != null &&
                e.Utilisateur.Actif)
            .OrderBy(e => e.Utilisateur!.Nom)
            .ThenBy(e => e.Utilisateur!.Prenom)
            .Select(e => new EtudiantSelectDto
            {
                IdEtudiant = e.IdEtudiant,
                Prenom = e.Utilisateur!.Prenom,
                Nom = e.Utilisateur.Nom,
                Courriel = e.Utilisateur.Courriel,
                IdCollege = e.Utilisateur.IdCollege,
                NomCollege = e.Utilisateur.College != null
                    ? e.Utilisateur.College.Nom
                    : null
            })
            .ToListAsync();

        return Ok(etudiants);
    }

    // GET /api/etudiants/5
    // Sert a recuperer un etudiant precis quand on arrive depuis une recommandation.
    [HttpGet("{idEtudiant:int}")]
    [Authorize(Roles = "Employeur,ResponsableStage,Administrateur,SuperAdministrateur")]
    public async Task<ActionResult<EtudiantSelectDto>> GetEtudiantById(int idEtudiant)
    {
        var etudiant = await _context.Etudiants
            .AsNoTracking()
            .Include(e => e.Utilisateur)
                .ThenInclude(u => u!.College)
            .Where(e =>
                e.IdEtudiant == idEtudiant &&
                e.Utilisateur != null &&
                e.Utilisateur.Actif)
            .Select(e => new EtudiantSelectDto
            {
                IdEtudiant = e.IdEtudiant,
                Prenom = e.Utilisateur!.Prenom,
                Nom = e.Utilisateur.Nom,
                Courriel = e.Utilisateur.Courriel,
                IdCollege = e.Utilisateur.IdCollege,
                NomCollege = e.Utilisateur.College != null
                    ? e.Utilisateur.College.Nom
                    : null
            })
            .FirstOrDefaultAsync();

        if (etudiant == null)
        {
            return NotFound(new { message = "Etudiant introuvable." });
        }

        return Ok(etudiant);
    }
}