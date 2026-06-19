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
            // Recupere le claim NameIdentifier dans le JWT.
            var id = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(id, out var idUtilisateur)
                ? idUtilisateur
                : null;
        }
    }

    public string? Role
    {
        get
        {
            // Recupere le role de l'utilisateur dans le JWT.
            return _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.Role);
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            // Indique si la requete contient un token valide.
            return _httpContextAccessor.HttpContext?.User
                .Identity?.IsAuthenticated ?? false;
        }
    }
}