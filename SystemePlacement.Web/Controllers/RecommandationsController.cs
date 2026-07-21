using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Recommandations;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecommandationsController : ControllerBase
{
    private readonly IRecommandationService _service;

    public RecommandationsController(IRecommandationService service) => _service = service;

    [HttpGet("etudiant/{idEtudiant:int}")]
    [Authorize(Roles = "Employeur,ResponsableStage,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> GetByEtudiant(int idEtudiant)
    {
        var recommandations = await _service.GetByEtudiantAsync(idEtudiant);
        return Ok(recommandations);
    }

    [HttpPost]
    [Authorize(Roles = "Employeur,ResponsableStage,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> Creer(
        [FromForm] CreerRecommandationRequest request,
        IFormFile? lettre)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _service.CreerAsync(request, lettre);
        return result is null
            ? Forbid()
            : CreatedAtAction(nameof(GetByEtudiant), new { idEtudiant = result.IdEtudiant }, result);
    }

    [HttpGet("{idRecommandation:int}/lettre")]
    [Authorize(Roles = "Employeur,ResponsableStage,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> TelechargerLettre(int idRecommandation)
    {
        var result = await _service.TelechargerLettreAsync(idRecommandation);
        if (result is null) return NotFound();

        return File(result.Value.Contenu, result.Value.ContentType, result.Value.NomFichier);
    }

    [HttpDelete("{idRecommandation:int}")]
    [Authorize(Roles = "Employeur,ResponsableStage,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> Supprimer(int idRecommandation)
    {
        var succes = await _service.SupprimerAsync(idRecommandation);
        return succes ? NoContent() : NotFound();
    }
}
