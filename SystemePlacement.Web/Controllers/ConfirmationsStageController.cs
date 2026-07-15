using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Stages;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/stages/{idStage:int}/confirmations")]
[Authorize(Roles = "Employeur,ResponsableStage")]
public class ConfirmationsStageController : ControllerBase
{
    private readonly IStageService _stageService;

    public ConfirmationsStageController(IStageService stageService)
    {
        _stageService = stageService;
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmerStage(
        int idStage,
        ConfirmationStageCreateDto request)
    {
        try
        {
            var stage = await _stageService.ConfirmerStageAsync(idStage, request);

            if (stage == null)
            {
                return NotFound(new { message = "Stage introuvable." });
            }

            return Ok(stage);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}