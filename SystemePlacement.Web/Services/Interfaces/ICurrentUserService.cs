namespace SystemePlacement.Web.Services.Interfaces;

public interface ICurrentUserService
{
    // Id de l'utilisateur connecte, lu dans le token JWT.
    int? IdUtilisateur { get; }

    // Id du college de l'utilisateur connecte.
    // Null pour un SuperAdministrateur.
    int? IdCollege { get; }

    // Role de l'utilisateur connecte.
    string? Role { get; }

    // Indique si la requete contient un token valide.
    bool IsAuthenticated { get; }
}