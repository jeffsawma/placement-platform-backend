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

    public OffresController(IOffreService service) => _service = service;

    // ── GET /api/offres?type=Emploi&statut=Active ─────────────────────────────
    /// <summary>Retourne la liste des offres (filtrable par type et statut).</summary>
    [HttpGet]
    [AllowAnonymous]                         // Les étudiants non connectés peuvent consulter
    public async Task<IActionResult> GetAll(
        [FromQuery] TypeOffre? type,
        [FromQuery] StatutOffre? statut)
        => Ok(await _service.GetAllAsync(type, statut));

    // ── GET /api/offres/{id} ──────────────────────────────────────────────────
    /// <summary>Retourne le détail d'une offre (emploi ou stage).</summary>
    [HttpGet("{idOffre:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int idOffre)
    {
        var offre = await _service.GetByIdAsync(idOffre);
        return offre is null ? NotFound() : Ok(offre);
    }

    // ── POST /api/offres/emploi ───────────────────────────────────────────────
    /// <summary>US-07 — Crée une offre d'emploi.</summary>
    [HttpPost("emploi")]
    [Authorize(Roles = "Employeur,Administrateur")]
    public async Task<IActionResult> CreerEmploi([FromBody] CreerOffreEmploiRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var offre = await _service.CreerEmploiAsync(request);
        return offre is null
            ? Forbid()
            : CreatedAtAction(nameof(GetById), new { idOffre = offre.IdOffre }, offre);
    }

    // ── POST /api/offres/stage ────────────────────────────────────────────────
    /// <summary>US-07 — Crée une offre de stage.</summary>
    [HttpPost("stage")]
    [Authorize(Roles = "Employeur,Administrateur")]
    public async Task<IActionResult> CreerStage([FromBody] CreerOffreStageRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var offre = await _service.CreerStageAsync(request);
        return offre is null
            ? Forbid()
            : CreatedAtAction(nameof(GetById), new { idOffre = offre.IdOffre }, offre);
    }

    // ── PUT /api/offres/emploi/{id} ───────────────────────────────────────────
    /// <summary>US-08 — Modifie une offre d'emploi.</summary>
    [HttpPut("emploi/{idOffre:int}")]
    [Authorize(Roles = "Employeur,Administrateur")]
    public async Task<IActionResult> ModifierEmploi(int idOffre, [FromBody] ModifierOffreEmploiRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var offre = await _service.ModifierEmploiAsync(idOffre, request);
        return offre is null ? NotFound() : Ok(offre);
    }

    // ── PUT /api/offres/stage/{id} ────────────────────────────────────────────
    /// <summary>US-09 — Modifie une offre de stage.</summary>
    [HttpPut("stage/{idOffre:int}")]
    [Authorize(Roles = "Employeur,Administrateur")]
    public async Task<IActionResult> ModifierStage(int idOffre, [FromBody] ModifierOffreStageRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var offre = await _service.ModifierStageAsync(idOffre, request);
        return offre is null ? NotFound() : Ok(offre);
    }

    // ── DELETE /api/offres/{id} ───────────────────────────────────────────────
    /// <summary>US-10 — Supprime une offre (soft: uniquement si aucune candidature).</summary>
    [HttpDelete("{idOffre:int}")]
    [Authorize(Roles = "Employeur,Administrateur")]
    public async Task<IActionResult> Supprimer(int idOffre)
    {
        var succes = await _service.SupprimerAsync(idOffre);
        return succes ? NoContent() : NotFound();
    }
}
