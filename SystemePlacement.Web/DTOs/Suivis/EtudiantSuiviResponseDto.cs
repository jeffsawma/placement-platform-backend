namespace SystemePlacement.Web.DTOs.Suivis;

public class EtudiantSuiviResponseDto
{
    public int IdEtudiant { get; set; }

    public int IdUtilisateur { get; set; }

    public string Prenom { get; set; } = string.Empty;

    public string Nom { get; set; } = string.Empty;

    public string Courriel { get; set; } = string.Empty;

    public int? IdCollege { get; set; }

    public string? NomCollege { get; set; }

    public int NombreCandidatures { get; set; }

    public string? DernierStatutCandidature { get; set; }

    public DateTime? DateDerniereCandidature { get; set; }
}