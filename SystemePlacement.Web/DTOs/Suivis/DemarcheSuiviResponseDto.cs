namespace SystemePlacement.Web.DTOs.Suivis;

public class DemarcheSuiviResponseDto
{
    public int IdDemarche { get; set; }

    public int IdEtudiant { get; set; }

    public int IdResponsable { get; set; }

    public string TypeDemarche { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public bool VisibleEtudiant { get; set; }

    public DateTime DateDemarche { get; set; }
}