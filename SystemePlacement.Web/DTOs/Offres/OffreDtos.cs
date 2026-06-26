using System.ComponentModel.DataAnnotations;
using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.DTOs.Offres;

// ─── REQUÊTES ────────────────────────────────────────────────────────────────

/// <summary>Payload pour créer une offre d'emploi (US-07).</summary>
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

    public List<int> IdsDomaines { get; set; } = new();
}

/// <summary>Payload pour créer une offre de stage (US-07).</summary>
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

    public List<int> IdsDomaines { get; set; } = new();
}

/// <summary>Payload pour modifier une offre d'emploi (US-08).</summary>
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

    public List<int> IdsDomaines { get; set; } = new();
}

/// <summary>Payload pour modifier une offre de stage (US-09).</summary>
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

    public List<int> IdsDomaines { get; set; } = new();
}

// ─── RÉPONSES ────────────────────────────────────────────────────────────────

/// <summary>Réponse commune (liste, résumé).</summary>
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
    public List<string> Domaines { get; set; } = new();
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
