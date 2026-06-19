namespace SystemePlacement.Web.Models;

public class College
{
    public int IdCollege { get; set; }

    public string Nom { get; set; } = string.Empty;

    public string Ville { get; set; } = string.Empty;

    public bool Actif { get; set; } = true;

    public ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}