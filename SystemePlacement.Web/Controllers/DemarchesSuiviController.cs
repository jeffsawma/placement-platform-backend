using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Suivis;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/demarches-suivi")]
[Authorize]
public class DemarchesSuiviController : ControllerBase
{
    private readonly ISuiviService _suiviService;

    public DemarchesSuiviController(ISuiviService suiviService)
    {
        _suiviService = suiviService;
    }

    // GET /api/demarches-suivi/etudiants
    // Liste des etudiants suivis par le responsable de stage.
    [HttpGet("etudiants")]
    [Authorize(Roles = "ResponsableStage")]
    public async Task<IActionResult> GetEtudiantsSuivis()
    {
        var etudiants = await _suiviService.GetEtudiantsSuivisAsync();
        return Ok(etudiants);
    }

    // GET /api/demarches-suivi/etudiants/5
    // Detail d'un etudiant suivi.
    [HttpGet("etudiants/{idEtudiant:int}")]
    [Authorize(Roles = "ResponsableStage")]
    public async Task<IActionResult> GetEtudiantSuiviDetail(int idEtudiant)
    {
        var detail = await _suiviService.GetEtudiantSuiviDetailAsync(idEtudiant);

        if (detail == null)
        {
            return NotFound(new { message = "Etudiant introuvable ou non accessible." });
        }

        return Ok(detail);
    }

    // POST /api/demarches-suivi/etudiants/5/demarches
    // Ajoute une demarche ou note de suivi pour un etudiant.
    [HttpPost("etudiants/{idEtudiant:int}/demarches")]
    [Authorize(Roles = "ResponsableStage")]
    public async Task<IActionResult> AjouterDemarche(
        int idEtudiant,
        DemarcheSuiviCreateDto request)
    {
        var demarche = await _suiviService.AjouterDemarcheAsync(idEtudiant, request);

        if (demarche == null)
        {
            return NotFound(new { message = "Etudiant introuvable ou non accessible." });
        }

        return CreatedAtAction(
            nameof(GetEtudiantSuiviDetail),
            new { idEtudiant },
            demarche);
    }

    // GET /api/demarches-suivi/mes-demarches
    // Liste des demarches visibles pour l'etudiant connecte.
    [HttpGet("mes-demarches")]
    [Authorize(Roles = "Etudiant")]
    public async Task<IActionResult> GetMesDemarches()
    {
        var demarches = await _suiviService.GetMesDemarchesAsync();
        return Ok(demarches);
    }
}