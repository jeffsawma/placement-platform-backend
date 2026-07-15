namespace SystemePlacement.Web.DTOs.Suivis;

public class DemarcheSuiviCreateDto
{
    public string TypeDemarche { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public bool VisibleEtudiant { get; set; }
}