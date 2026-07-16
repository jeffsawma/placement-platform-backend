namespace SystemePlacement.Web.Models;

public class College
{
    public int IdCollege { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string Ville { get; set; } = string.Empty;

    public bool Actif { get; set; } = true;

    // Theme visuel du college.
    // Ces valeurs alimentent les variables CSS du frontend.
    public string CouleurPrimaire { get; set; } = "#009fda";

    public string CouleurPrimaireFoncee { get; set; } = "#003f7d";

    public string CouleurSecondaire { get; set; } = "#0053a1";

    public string CouleurAccent { get; set; } = "#69be28";

    public string CouleurFond { get; set; } = "#f4f7fb";

    public string CouleurTexte { get; set; } = "#172033";

    public string? LogoUrl { get; set; }

    public ICollection<DomaineEtude> DomaineEtudes { get; set; } = new List<DomaineEtude>();

    public ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}