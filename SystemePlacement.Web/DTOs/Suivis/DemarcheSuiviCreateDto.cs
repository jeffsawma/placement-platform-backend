namespace SystemePlacement.Web.DTOs.Suivis;

public class DemarcheSuiviCreateDto
{
    // Exemple : Appel, Courriel, Rencontre, Note.
    public string TypeDemarche { get; set; } = string.Empty;

    // Note ajoutee par le responsable de stage.
    public string Note { get; set; } = string.Empty;
}