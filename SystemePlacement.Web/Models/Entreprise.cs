namespace SystemePlacement.Web.Models;

public class Entreprise
{
    public int IdEntreprise { get; set; }
    public int IdEmployeur { get; set; } // Clé étrangère vers Employeur // À ajouter après le FK IdEmployeur dans l'entité Entreprise
    public string Nom { get; set; } = string.Empty;
    public string Secteur { get; set; } = string.Empty;
    public string Adresse {  get; set; } = string.Empty;
    public string? SiteWeb { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }

    // Relations
    // Une entreprise appartient à un employeur
    public Employeur? Employeur { get; set; }
}
