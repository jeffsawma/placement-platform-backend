namespace SystemePlacement.Web.Models;

public class Administrateur
{
    public int IdAdministrateur { get; set; }

    public string NiveauAcces { get; set; } = "Standard";

    public int IdUtilisateur { get; set; }

    public Utilisateur? Utilisateur { get; set; }
}