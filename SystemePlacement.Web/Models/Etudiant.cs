namespace SystemePlacement.Web.Models;

public class Etudiant
{
    public int IdEtudiant { get; set; }

    public int IdUtilisateur { get; set; }

    public Utilisateur? Utilisateur { get; set; }
}