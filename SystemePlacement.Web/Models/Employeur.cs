namespace SystemePlacement.Web.Models;

public class Employeur
{
    public int IdEmployeur { get; set; }
    public int IdUtilisateur { get; set; } // FK
    public string Poste { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string Titre { get; set; } = string.Empty;

    // Navigation vers Utilisateur et Entreprise
    public Utilisateur? Utilisateur { get; set; }
    public Entreprise? Entreprise { get; set; }
}
