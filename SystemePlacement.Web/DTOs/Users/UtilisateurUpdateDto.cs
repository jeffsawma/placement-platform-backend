namespace SystemePlacement.Web.DTOs.Users;

public class UtilisateurUpdateDto
{
    public string Prenom { get; set; } = string.Empty;

    public string Nom { get; set; } = string.Empty;

    public string Courriel { get; set; } = string.Empty;

    public string Langue { get; set; } = "fr";

    public int IdRole { get; set; }

    public int? IdCollege { get; set; }

    public bool Actif { get; set; }
}