using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Candidatures;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CandidaturesController : ControllerBase
{
    private readonly ICandidatureService _service;

    public CandidaturesController(ICandidatureService service) => _service = service;

    // GET /api/offres/{idOffre}/candidatures
    // US-10 : liste des candidatures recues pour une offre
    [HttpGet("/api/offres/{idOffre:int}/candidatures")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> GetCandidaturesOffre(int idOffre)
    {
        var candidatures = await _service.GetCandidaturesOffreAsync(idOffre);
        return Ok(candidatures);
    }

    // GET /api/candidatures/{id}
    // Detail d'une candidature
    [HttpGet("{idCandidature:int}")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> GetDetail(int idCandidature)
    {
        var detail = await _service.GetDetailAsync(idCandidature);
        return detail is null ? NotFound() : Ok(detail);
    }

    // PATCH /api/candidatures/{id}/statut
    // US-10 : changer le statut d'une candidature (Vue, Acceptee, Refusee)
    [HttpPatch("{idCandidature:int}/statut")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> ChangerStatut(
        int idCandidature,
        [FromBody] ChangerStatutCandidatureRequest request)
    {
        var succes = await _service.ChangerStatutAsync(idCandidature, request.Statut);
        return succes ? NoContent() : NotFound();
    }

    // GET /api/candidatures/documents/{idDocument}/telecharger
    // US-12 : telecharger le CV ou la lettre de motivation
    [HttpGet("documents/{idDocument:int}/telecharger")]
    [Authorize(Roles = "Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> TelechargerDocument(int idDocument)
    {
        var result = await _service.TelechargerDocumentAsync(idDocument);
        if (result is null) return NotFound();

        var (contenu, contentType, nomFichier) = result;
        return File(contenu, contentType, nomFichier);
    }
}
