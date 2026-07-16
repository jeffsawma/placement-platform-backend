namespace SystemePlacement.Web.DTOs.Auth;

public class UtilisateurConnecteDto
{
    public int IdUtilisateur { get; set; }

    public string Prenom { get; set; } = string.Empty;

    public string Nom { get; set; } = string.Empty;

    public string Courriel { get; set; } = string.Empty;

    public string NomUtilisateur { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public int? IdCollege { get; set; }
}