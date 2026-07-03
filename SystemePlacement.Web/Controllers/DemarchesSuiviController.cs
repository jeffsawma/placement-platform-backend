using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    // GET /api/demarches-suivi/etudiants
    // Retourne la liste des etudiants suivis par le responsable connecte.
    [HttpGet("etudiants")]
    public async Task<IActionResult> GetEtudiantsSuivis()
    {
        var etudiants = await _suiviService.GetEtudiantsSuivisAsync();
        return Ok(etudiants);
    }
}