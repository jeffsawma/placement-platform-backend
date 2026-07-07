using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.OffresStageDirectes;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/offres-stage-directes")]
[Authorize]
public class OffresStageDirectesController : ControllerBase
{
    private readonly IOffreStageDirecteService _service;

    public OffresStageDirectesController(IOffreStageDirecteService service)
    {
        _service = service;
    }

    // L'employeur consulte ses offres de stage directes
    [HttpGet("mes")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> MesOffres()
        => Ok(await _service.GetMesOffresAsync());

    [HttpGet("{idOffreDirecte:int}")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> Get(int idOffreDirecte)
    {
        var offre = await _service.GetAsync(idOffreDirecte);
        return offre is null ? NotFound() : Ok(offre);
    }

    // L'employeur crée une offre de stage directe
    [HttpPost]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> Creer([FromBody] CreerOffreStageDirecteRequest request)
    {
        var offre = await _service.CreerAsync(request);

        return offre is null
            ? BadRequest(new { message = "Offre de stage directe impossible: employeur, étudiant ou conditions invalides." })
            : CreatedAtAction(nameof(Get), new { idOffreDirecte = offre.IdOffreDirecte }, offre);
    }
}
