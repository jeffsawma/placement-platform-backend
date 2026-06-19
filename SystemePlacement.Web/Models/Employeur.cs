namespace SystemePlacement.Web.Models;

public class Employeur
{
    public int IdEmployeur { get; set; }

    public int IdUtilisateur { get; set; }

    public Utilisateur? Utilisateur { get; set; }
}