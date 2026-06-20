namespace SystemePlacement.Web.Services.Interfaces;

public interface ICurrentUserService
{
    // Identifiant de l'utilisateur connecte, extrait du token JWT.
    int? IdUtilisateur { get; }

    // Role de l'utilisateur connecte, extrait du token JWT.
    string? Role { get; }

    // Indique rapidement si une requete est authentifiee.
    bool IsAuthenticated { get; }
}