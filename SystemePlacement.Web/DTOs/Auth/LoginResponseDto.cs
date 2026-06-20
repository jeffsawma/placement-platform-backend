namespace SystemePlacement.Web.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;

    public DateTime Expiration { get; set; }

    public UtilisateurConnecteDto Utilisateur { get; set; } = new();
}