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

    [HttpGet("offre/{idOffre:int}")]
    public async Task<IActionResult> GetParOffre(int idOffre)
        => Ok(await _service.GetParOffreAsync(idOffre));

    [HttpGet("{idCandidature:int}")]
    public async Task<IActionResult> Get(int idCandidature)
    {
        var candidature = await _service.GetAsync(idCandidature);
        return candidature is null ? NotFound() : Ok(candidature);
    }

    [HttpPost]
    public async Task<IActionResult> Postuler(PostulerRequest request)
    {
        var candidature = await _service.PostulerAsync(request);
        return candidature is null
            ? Conflict(new { message = "Candidature impossible : CV manquant, déjà postulé, ou profil étudiant introuvable." })
            : CreatedAtAction(nameof(Get), new { idCandidature = candidature.IdCandidature }, candidature);
    }

    [HttpPut("{idCandidature:int}/statut")]
    public async Task<IActionResult> ChangerStatut(int idCandidature, ChangerStatutRequest request)
        => await _service.ChangerStatutAsync(idCandidature, request) ? NoContent() : NotFound();
}
