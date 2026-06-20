namespace SystemePlacement.Web.Models;

public class College
{
    public int IdCollege { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public bool Actif { get; set; } = true; // Par défaut, un collège est actif

    // Relations
    // Un collège peut avoir plusieurs domaines d'étude et plusieurs utilisateurs
    public ICollection<DomaineEtude> DomaineEtudes { get; set; } = new List<DomaineEtude>();
    public ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}
