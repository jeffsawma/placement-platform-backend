namespace SystemePlacement.Web.Models;

public class Role
{
    public int IdRole { get; set; }

    public string NomRole { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool Actif { get; set; } = true;

    public ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}