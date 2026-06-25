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
            var val = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(val, out var id) ? id : null;
        }
    }

    public string? Role =>
        _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.Role);
}