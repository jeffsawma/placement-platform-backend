namespace SystemePlacement.Web.DTOs.Etudiants;

public class EtudiantSelectDto
{
    // Id utilise par l'offre de stage directe.
    public int IdEtudiant { get; set; }

    // Infos affichees dans le select du frontend.
    public string Prenom { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Courriel { get; set; } = string.Empty;

    // College optionnel, utile pour afficher ou filtrer plus tard.
    public int? IdCollege { get; set; }
    public string? NomCollege { get; set; }
}