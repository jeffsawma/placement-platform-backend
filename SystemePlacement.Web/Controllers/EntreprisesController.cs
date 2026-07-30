using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemePlacement.Web.Data;
using SystemePlacement.Web.DTOs.Entreprises;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/entreprises")]
[Authorize]
public class EntreprisesController : ControllerBase
{
    private readonly IEntrepriseService _entrepriseService;
    private readonly ApplicationDbContext _context;

    public EntreprisesController(
        IEntrepriseService entrepriseService,
        ApplicationDbContext context)
    {
        _entrepriseService = entrepriseService;
        _context = context;
    }

    // GET /api/entreprises/employeurs
    // Sert à remplir le select d'employeurs dans la page Recommandations.
    [HttpGet("employeurs")]
    [Authorize(Roles = "ResponsableStage,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> GetEmployeurs()
    {
        var employeurs = await _context.Employeurs
            .AsNoTracking()
            .Include(e => e.Utilisateur)
            .Include(e => e.Entreprise)
            .Where(e => e.Utilisateur != null && e.Utilisateur.Actif)
            .OrderBy(e => e.Entreprise != null ? e.Entreprise.Nom : e.Utilisateur!.Nom)
            .Select(e => new
            {
                e.IdEmployeur,
                Nom = e.Entreprise != null
                    ? e.Entreprise.Nom
                    : e.Utilisateur!.Prenom + " " + e.Utilisateur.Nom,
                Courriel = e.Utilisateur != null ? e.Utilisateur.Courriel : null
            })
            .ToListAsync();

        return Ok(employeurs);
    }

    // GET /api/entreprises/mon-profil
    [HttpGet("mon-profil")]
    [Authorize(Roles = "Employeur")]
    public async Task<ActionResult<EntrepriseResponseDto>> GetMonProfil()
    {
        try
        {
            var entreprise = await _entrepriseService.GetMonProfilAsync();

            if (entreprise == null)
            {
                return NotFound(new { message = "Aucun profil d'entreprise trouvé pour cet employeur." });
            }

            return Ok(entreprise);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /api/entreprises/mon-profil
    [HttpPost("mon-profil")]
    [Authorize(Roles = "Employeur")]
    public async Task<ActionResult<EntrepriseCreateDto>> CreateMonProfil([FromBody] EntrepriseCreateDto dto)
    {
        try
        {
            var entreprise = await _entrepriseService.CreateMonProfilAsync(dto);

            return CreatedAtAction(nameof(GetMonProfil), entreprise);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/entreprises/mon-profil
    [HttpPut("mon-profil")]
    [Authorize(Roles = "Employeur")]
    public async Task<IActionResult> UpdateMonProfil([FromBody] EntrepriseUpdateDto dto)
    {
        try
        {
            var updated = await _entrepriseService.UpdateMonProfilAsync(dto);

            if (!updated)
            {
                return NotFound(new { message = "Aucun profil d'entreprise trouvé pour cet employeur." });
            }

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}