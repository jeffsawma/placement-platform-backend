using System.ComponentModel.DataAnnotations;

namespace SystemePlacement.Web.Models;

public class DemarcheSuivi
{
    [Key]
    public int IdDemarche { get; set; }

    // Etudiant concerne par la demarche de suivi.
    public int IdEtudiant { get; set; }

    public Etudiant? Etudiant { get; set; }

    // Responsable de stage qui ajoute la note.
    public int IdResponsable { get; set; }

    public ResponsableStage? Responsable { get; set; }

    // Exemple : Appel, Courriel, Rencontre, Note.
    public string TypeDemarche { get; set; } = string.Empty;

    // Contenu de la note de suivi.
    public string Note { get; set; } = string.Empty;

    public DateTime DateDemarche { get; set; } = DateTime.UtcNow;
}