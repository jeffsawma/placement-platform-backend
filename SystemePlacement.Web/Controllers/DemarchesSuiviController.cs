using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Suivis;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/demarches-suivi")]
[Authorize(Roles = "ResponsableStage")]
public class DemarchesSuiviController : ControllerBase
{
    private readonly ISuiviService _suiviService;

    public DemarchesSuiviController(ISuiviService suiviService)
    {
        _suiviService = suiviService;
    }

    [HttpGet("etudiants")]
    public async Task<IActionResult> GetEtudiantsSuivis()
    {
        var etudiants = await _suiviService.GetEtudiantsSuivisAsync();
        return Ok(etudiants);
    }

    [HttpGet("etudiants/{idEtudiant:int}")]
    public async Task<IActionResult> GetEtudiantSuiviDetail(int idEtudiant)
    {
        var detail = await _suiviService.GetEtudiantSuiviDetailAsync(idEtudiant);

        if (detail == null)
        {
            return NotFound(new { message = "Etudiant introuvable ou non accessible." });
        }

        return Ok(detail);
    }

    [HttpPost("etudiants/{idEtudiant:int}/demarches")]
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
}