namespace SystemePlacement.Web.DTOs.Auth;

public class LoginRequestDto
{
    public string NomUtilisateur { get; set; } = string.Empty;

    public string MotDePasse { get; set; } = string.Empty;
}