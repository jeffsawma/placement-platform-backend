namespace SystemePlacement.Web.Models;

public class Recommandation
{
    public int IdRecommandation { get; set; }

    public int IdEtudiant { get; set; }

    public int IdAuteur { get; set; }

    public string Commentaire { get; set; } = string.Empty;

    public string? CheminLettreRecommandation { get; set; }

    public string? NomFichierLettre { get; set; }

    public string? ContentTypeLettre { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public Etudiant? Etudiant { get; set; }

    public Utilisateur? Auteur { get; set; }
}
