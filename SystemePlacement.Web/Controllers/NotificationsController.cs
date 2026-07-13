using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;

    public NotificationsController(INotificationService service) => _service = service;

    // US-20 : notifications de l'utilisateur connecte.
    [HttpGet("mes")]
    public async Task<IActionResult> MesNotifications()
        => Ok(await _service.GetMesNotificationsAsync());

    [HttpGet("non-lues")]
    public async Task<IActionResult> CompterNonLues()
        => Ok(new { nonLues = await _service.CompterMesNonLuesAsync() });

    [HttpPost("{idNotification:int}/lue")]
    public async Task<IActionResult> MarquerLue(int idNotification)
        => await _service.MarquerLueAsync(idNotification) ? NoContent() : NotFound();
}
