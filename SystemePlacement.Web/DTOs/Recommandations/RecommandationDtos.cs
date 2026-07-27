using System.ComponentModel.DataAnnotations;

namespace SystemePlacement.Web.DTOs.Recommandations;

public class CreerRecommandationRequest
{
    [Required]
    public int IdEtudiant { get; set; }

    [Required, MaxLength(2000)]
    public string Commentaire { get; set; } = string.Empty;
}

public class RecommandationResponse
{
    public int IdRecommandation { get; set; }
    public int IdEtudiant { get; set; }
    public string NomEtudiant { get; set; } = string.Empty;
    public string PrenomEtudiant { get; set; } = string.Empty;
    public string NomAuteur { get; set; } = string.Empty;
    public string PrenomAuteur { get; set; } = string.Empty;
    public string Commentaire { get; set; } = string.Empty;
    public bool ALettre { get; set; }
    public string? NomFichierLettre { get; set; }
    public DateTime DateCreation { get; set; }
}
