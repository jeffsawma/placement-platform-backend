using System.ComponentModel.DataAnnotations;
using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.DTOs.Offres;

public class CreerOffreEmploiRequest
{
    [Required, MaxLength(200)]
    public string Titre { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Ville { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Adresse { get; set; }

    public DateTime? DateExpiration { get; set; }

    [MaxLength(50)]
    public string? TypeContrat { get; set; }

    [Range(0, 999999)]
    public decimal? SalaireMin { get; set; }

    [Range(0, 999999)]
    public decimal? SalaireMax { get; set; }

    [MaxLength(50)]
    public string? TeleTravail { get; set; }

    // Liste des domaines lies a l'offre.
    public List<int> IdsDomaines { get; set; } = new();
}

public class CreerOffreStageRequest
{
    [Required, MaxLength(200)]
    public string Titre { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Ville { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Adresse { get; set; }

    public DateTime? DateExpiration { get; set; }

    public DateTime? DateDebutStage { get; set; }

    public DateTime? DateFinStage { get; set; }

    [Range(1, 168)]
    public int? DureeHeuresParSemaine { get; set; }

    [Range(0, 999999)]
    public decimal? Remuneration { get; set; }

    [MaxLength(50)]
    public string? Session { get; set; }

    // Liste des domaines lies a l'offre de stage.
    public List<int> IdsDomaines { get; set; } = new();
}

public class ModifierOffreEmploiRequest
{
    [Required, MaxLength(200)]
    public string Titre { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Ville { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Adresse { get; set; }

    public DateTime? DateExpiration { get; set; }

    public StatutOffre Statut { get; set; }

    [MaxLength(50)]
    public string? TypeContrat { get; set; }

    [Range(0, 999999)]
    public decimal? SalaireMin { get; set; }

    [Range(0, 999999)]
    public decimal? SalaireMax { get; set; }

    [MaxLength(50)]
    public string? TeleTravail { get; set; }

    // Liste complete des domaines a conserver apres modification.
    public List<int> IdsDomaines { get; set; } = new();
}

public class ModifierOffreStageRequest
{
    [Required, MaxLength(200)]
    public string Titre { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Ville { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Adresse { get; set; }

    public DateTime? DateExpiration { get; set; }

    public StatutOffre Statut { get; set; }

    public DateTime? DateDebutStage { get; set; }

    public DateTime? DateFinStage { get; set; }

    [Range(1, 168)]
    public int? DureeHeuresParSemaine { get; set; }

    [Range(0, 999999)]
    public decimal? Remuneration { get; set; }

    [MaxLength(50)]
    public string? Session { get; set; }

    // Liste complete des domaines a conserver apres modification.
    public List<int> IdsDomaines { get; set; } = new();
}

public class OffreResumeeResponse
{
    public int IdOffre { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public TypeOffre TypeOffre { get; set; }
    public StatutOffre Statut { get; set; }
    public DateTime DatePublication { get; set; }
    public DateTime? DateExpiration { get; set; }
    public string NomEmployeur { get; set; } = string.Empty;

    // Noms affiches dans les cartes/tableaux.
    public List<string> Domaines { get; set; } = new();

    // IDs utilises par le formulaire pour preselectionner les domaines.
    public List<int> IdsDomaines { get; set; } = new();
}

public class OffreEmploiResponse : OffreResumeeResponse
{
    public string Description { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public string? TypeContrat { get; set; }
    public decimal? SalaireMin { get; set; }
    public decimal? SalaireMax { get; set; }
    public string? TeleTravail { get; set; }
}

public class OffreStageResponse : OffreResumeeResponse
{
    public string Description { get; set; } = string.Empty;
    public string? Adresse { get; set; }
    public DateTime? DateDebutStage { get; set; }
    public DateTime? DateFinStage { get; set; }
    public int? DureeHeuresParSemaine { get; set; }
    public decimal? Remuneration { get; set; }
    public string? Session { get; set; }
}