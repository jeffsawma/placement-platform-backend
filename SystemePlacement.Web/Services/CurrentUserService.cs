using System.Security.Claims;
using SystemePlacement.Web.Services.Interfaces;

namespace SystemePlacement.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? IdUtilisateur
    {
        get
        {
            // Recupere l'id utilisateur dans le token JWT.
            var id = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(id, out var idUtilisateur)
                ? idUtilisateur
                : null;
        }
    }

    public int? IdCollege
    {
        get
        {
            // Recupere l'id du college dans le token JWT.
            // Pour un SuperAdministrateur, ce claim peut etre absent.
            var idCollege = _httpContextAccessor.HttpContext?.User
                .FindFirstValue("idCollege");

            return int.TryParse(idCollege, out var value)
                ? value
                : null;
        }
    }

    public string? Role
    {
        get
        {
            // Recupere le role dans le token JWT.
            return _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Role);
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            // Verifie si l'utilisateur est authentifie.
            return _httpContextAccessor.HttpContext?.User
                .Identity?.IsAuthenticated ?? false;
        }
    }
}