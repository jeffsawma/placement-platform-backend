namespace SystemePlacement.Web.Models;

public class CollegeDomaine
{
    public int IdCollegeDomaine { get; set; }

    public int IdCollege { get; set; }
    public int IdDomaine { get; set; }

    // Un college peut accepter ou non les stagiaires pour ce domaine.
    public bool AccepteStagiaires { get; set; } = true;

    // Permet de retirer un domaine d'un collège sans supprimer le domaine global.
    public bool Actif { get; set; } = true;

    public College? College { get; set; }
    public DomaineEtude? DomaineEtude { get; set; }
}