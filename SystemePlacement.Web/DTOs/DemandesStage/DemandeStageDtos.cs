using System.ComponentModel.DataAnnotations;
using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.DTOs.DemandesStage;

public class CreerDemandeStageRequest
{
    [Required]
    public int IdDomaine { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? PeriodeSouhaitee { get; set; }

    public string? Competences { get; set; }
}

public class DemandeStageResponse
{
    public int IdDemandeStage { get; set; }
    public int IdDomaine { get; set; }
    public string NomDomaine { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? PeriodeSouhaitee { get; set; }
    public string? Competences { get; set; }
    public StatutDemandeStage Statut { get; set; }
    public DateTime DateCreation { get; set; }
}
