namespace SystemePlacement.Web.DTOs.DomainesEtudes;

public class CollegeDomaineResponseDto
{
    public int IdCollege { get; set; }
    public string NomCollege { get; set; } = string.Empty;
    public bool AccepteStagiaires { get; set; }
    public bool Actif { get; set; }
}