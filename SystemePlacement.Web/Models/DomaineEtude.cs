namespace SystemePlacement.Web.Models;

public class DomaineEtude
{
    public int IdDomaine { get; set; }

    // Nom global du domaine, exemple : Informatique.
    public string Nom { get; set; } = string.Empty;

    // Code global du domaine, exemple : INFO.
    public string Code { get; set; } = string.Empty;

    // Permet de désactiver un domaine global sans le supprimer.
    public bool Actif { get; set; } = true;

    // Liste des cegeps qui utilisent ce domaine.
    public ICollection<CollegeDomaine> CollegeDomaines { get; set; } = new List<CollegeDomaine>();

    // Offres liees a ce domaine.
    public ICollection<OffreDomaine> OffreDomaines { get; set; } = new List<OffreDomaine>();

    // Demandes de stage liees a ce domaine.
    public ICollection<DemandeStage> DemandesStage { get; set; } = new List<DemandeStage>();
}