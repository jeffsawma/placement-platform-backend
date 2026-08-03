namespace SystemePlacement.Web.DTOs.DomainesEtudes;

public class DomaineEtudeResponseDto
{
    public int IdDomaine { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool Actif { get; set; }

    // Nouvelle logique propre : plusieurs cegeps pour un domaine.
    public List<CollegeDomaineResponseDto> Colleges { get; set; } = new();

    // Champs conserves pour eviter de briser trop vite les anciennes pages front.
    public int? IdCollege { get; set; }
    public string? NomCollege { get; set; }
    public bool? AccepteStagiaires { get; set; }
}