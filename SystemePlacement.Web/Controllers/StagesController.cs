using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Stages;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/stages")]
[Authorize]
public class StagesController : ControllerBase
{
    private readonly IStageService _stageService;

    public StagesController(IStageService stageService)
    {
        _stageService = stageService;
    }

    [HttpGet]
    [Authorize(Roles = "ResponsableStage,Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> GetStages()
    {
        return Ok(await _stageService.GetStagesAsync());
    }

    [HttpGet("{idStage:int}")]
    [Authorize(Roles = "ResponsableStage,Employeur,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> GetStageById(int idStage)
    {
        var stage = await _stageService.GetStageByIdAsync(idStage);

        if (stage == null)
        {
            return NotFound(new { message = "Stage introuvable." });
        }

        return Ok(stage);
    }

    [HttpPost]
    [Authorize(Roles = "ResponsableStage,Administrateur,SuperAdministrateur")]
    public async Task<IActionResult> CreerStage(StageCreateDto request)
    {
        try
        {
            var stage = await _stageService.CreerStageAsync(request);

            return CreatedAtAction(
                nameof(GetStageById),
                new { idStage = stage.IdStage },
                stage);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}