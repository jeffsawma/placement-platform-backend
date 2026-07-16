namespace SystemePlacement.Web.DTOs.Colleges;

public class CollegeResponseDto
{
    public int IdCollege { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string Ville { get; set; } = string.Empty;

    public bool Actif { get; set; }

    public string CouleurPrimaire { get; set; } = string.Empty;

    public string CouleurPrimaireFoncee { get; set; } = string.Empty;

    public string CouleurSecondaire { get; set; } = string.Empty;

    public string CouleurAccent { get; set; } = string.Empty;

    public string CouleurFond { get; set; } = string.Empty;

    public string CouleurTexte { get; set; } = string.Empty;

    public string? LogoUrl { get; set; }
}