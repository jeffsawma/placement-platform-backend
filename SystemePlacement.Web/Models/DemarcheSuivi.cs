using System.ComponentModel.DataAnnotations;

namespace SystemePlacement.Web.Models;

public class DemarcheSuivi
{
    [Key]
    public int IdDemarche { get; set; }

    public int IdEtudiant { get; set; }

    public Etudiant? Etudiant { get; set; }

    public int IdResponsable { get; set; }

    public ResponsableStage? Responsable { get; set; }

    public string TypeDemarche { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public bool VisibleEtudiant { get; set; }

    public DateTime DateDemarche { get; set; } = DateTime.UtcNow;
}