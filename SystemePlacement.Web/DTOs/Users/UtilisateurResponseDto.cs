namespace SystemePlacement.Web.DTOs.Users;

public class UtilisateurResponseDto
{
    public int IdUtilisateur { get; set; }

    public string Prenom { get; set; } = string.Empty;

    public string Nom { get; set; } = string.Empty;

    public string Courriel { get; set; } = string.Empty;

    public string NomUtilisateur { get; set; } = string.Empty;

    public string Langue { get; set; } = string.Empty;

    public bool Actif { get; set; }

    public DateTime DateCreation { get; set; }

    public DateTime? DerniereConnexion { get; set; }

    public int IdRole { get; set; }

    public string Role { get; set; } = string.Empty;

    public int? IdCollege { get; set; }

    public string? NomCollege { get; set; }
}