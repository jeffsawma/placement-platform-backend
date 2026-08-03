namespace SystemePlacement.Web.Models;

public class College
{
    public int IdCollege { get; set; }

    public string Nom { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public bool Actif { get; set; } = true;

    // Theme visuel du college.
    public string CouleurPrimaire { get; set; } = "#009fda";
    public string CouleurPrimaireFoncee { get; set; } = "#003f7d";
    public string CouleurSecondaire { get; set; } = "#0053a1";
    public string CouleurAccent { get; set; } = "#69be28";
    public string CouleurFond { get; set; } = "#f4f7fb";
    public string CouleurTexte { get; set; } = "#172033";
    public string? LogoUrl { get; set; }

    // Liste des domaines disponibles dans ce college.
    public ICollection<CollegeDomaine> CollegeDomaines { get; set; } = new List<CollegeDomaine>();

    public ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}