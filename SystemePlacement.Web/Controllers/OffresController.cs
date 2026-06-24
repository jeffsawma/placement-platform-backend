using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Offres;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OffresController : ControllerBase
{
    private readonly IOffreService _service;

    public OffresController(IOffreService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Rechercher([FromQuery] RechercheOffresQuery query)
        => Ok(await _service.RechercherAsync(query));

    [HttpGet("{idOffre:int}")]
    public async Task<IActionResult> Get(int idOffre)
    {
        var offre = await _service.GetAsync(idOffre);
        return offre is null ? NotFound() : Ok(offre);
    }

    [HttpGet("{idOffre:int}/statut")]
    public async Task<IActionResult> GetStatut(int idOffre)
    {
        var statut = await _service.GetStatutAsync(idOffre);
        return statut is null ? NotFound() : Ok(statut);
    }
}
