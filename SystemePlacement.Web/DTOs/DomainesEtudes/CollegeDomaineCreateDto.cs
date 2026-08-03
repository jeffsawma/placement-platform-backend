namespace SystemePlacement.Web.DTOs.DomainesEtudes;

public class CollegeDomaineCreateDto
{
    public int IdCollege { get; set; }
    public bool AccepteStagiaires { get; set; } = true;
    public bool Actif { get; set; } = true;
}