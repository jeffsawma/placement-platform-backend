namespace SystemePlacement.Web.DTOs.Colleges;

public class CollegeCreateDto
{
    public string Nom { get; set; } = string.Empty;

    public string Ville { get; set; } = string.Empty;

    public bool Actif { get; set; } = true;

    // Couleurs par defaut : theme Cegep Gerald-Godin.
    public string CouleurPrimaire { get; set; } = "#009fda";

    public string CouleurPrimaireFoncee { get; set; } = "#003f7d";

    public string CouleurSecondaire { get; set; } = "#0053a1";

    public string CouleurAccent { get; set; } = "#69be28";

    public string CouleurFond { get; set; } = "#f4f7fb";

    public string CouleurTexte { get; set; } = "#172033";

    public string? LogoUrl { get; set; }
}