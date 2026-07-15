using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Offres;
using SystemePlacement.Web.Enums;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OffresController : ControllerBase
{
    private readonly IOffreService _service;

    public OffresController(IOffreService service)
    {
        _service = service;
    }

    // GET /api/offres
    // Liste publique des offres pour la recherche.
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] TypeOffre? type,
        [FromQuery] StatutOffre? statut,
        [FromQuery] int? idDomaine,
        [FromQuery] string? lieu,
        [FromQuery] string? motsCles)
    {
        var offres = await _service.GetAllAsync(type, statut, idDomaine, lieu, motsCles);
        return Ok(offres);
    }

    // GET /api/offres/mes-offres
    // Employeur : ses offres seulement.
    // Admin/SuperAdmin : toutes les offres.
    [HttpGet("mes-offres")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> GetMesOffres()
    {
        var offres = await _service.GetMesOffresAsync();
        return Ok(offres);
    }

    // GET /api/offres/{id}
    [HttpGet("{idOffre:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int idOffre)
    {
        var offre = await _service.GetByIdAsync(idOffre);
        return offre is null ? NotFound() : Ok(offre);
    }

    // POST /api/offres/emploi
    [HttpPost("emploi")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> CreerEmploi([FromBody] CreerOffreEmploiRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var offre = await _service.CreerEmploiAsync(request);

        return offre is null
            ? Forbid()
            : CreatedAtAction(nameof(GetById), new { idOffre = offre.IdOffre }, offre);
    }

    // POST /api/offres/stage
    [HttpPost("stage")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> CreerStage([FromBody] CreerOffreStageRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var offre = await _service.CreerStageAsync(request);

        return offre is null
            ? Forbid()
            : CreatedAtAction(nameof(GetById), new { idOffre = offre.IdOffre }, offre);
    }

    // PUT /api/offres/emploi/{id}
    [HttpPut("emploi/{idOffre:int}")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> ModifierEmploi(
        int idOffre,
        [FromBody] ModifierOffreEmploiRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var offre = await _service.ModifierEmploiAsync(idOffre, request);
        return offre is null ? NotFound() : Ok(offre);
    }

    // PUT /api/offres/stage/{id}
    [HttpPut("stage/{idOffre:int}")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> ModifierStage(
        int idOffre,
        [FromBody] ModifierOffreStageRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var offre = await _service.ModifierStageAsync(idOffre, request);
        return offre is null ? NotFound() : Ok(offre);
    }

    // DELETE /api/offres/{id}
    [HttpDelete("{idOffre:int}")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> Supprimer(int idOffre)
    {
        var succes = await _service.SupprimerAsync(idOffre);
        return succes ? NoContent() : NotFound();
    }
}