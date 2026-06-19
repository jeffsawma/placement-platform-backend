using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Auth;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;

    public AuthController(
        IAuthService authService,
        ICurrentUserService currentUserService)
    {
        _authService = authService;
        _currentUserService = currentUserService;
    }

    // POST: api/auth/login
    // Connecte un utilisateur et retourne un token JWT.
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    // GET: api/auth/me
    // Retourne les informations de l'utilisateur connecte.
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UtilisateurConnecteDto>> Me()
    {
        if (!_currentUserService.IdUtilisateur.HasValue)
        {
            return Unauthorized(new { message = "Utilisateur non authentifie." });
        }

        var utilisateur = await _authService.GetUtilisateurConnecteAsync(
            _currentUserService.IdUtilisateur.Value);

        if (utilisateur == null)
        {
            return NotFound(new { message = "Utilisateur introuvable." });
        }

        return Ok(utilisateur);
    }

    // POST: api/auth/logout
    // Avec JWT, le logout est surtout gere cote frontend en supprimant le token.
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        return Ok(new { message = "Deconnexion reussie. Supprimez le token cote client." });
    }
}