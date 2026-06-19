namespace SystemePlacement.Web.Models;

public class Utilisateur
{
    public int IdUtilisateur { get; set; }

    public string Prenom { get; set; } = string.Empty;

    public string Nom { get; set; } = string.Empty;

    public string Courriel { get; set; } = string.Empty;

    public string NomUtilisateur { get; set; } = string.Empty;

    public string MotDePasseHash { get; set; } = string.Empty;

    public string Langue { get; set; } = "fr";

    public bool Actif { get; set; } = true;

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    public DateTime? DateModification { get; set; }

    public DateTime? DerniereConnexion { get; set; }

    public int IdRole { get; set; }

    public Role? Role { get; set; }

    public int? IdCollege { get; set; }

    public College? College { get; set; }

    public Etudiant? Etudiant { get; set; }

    public Employeur? Employeur { get; set; }

    public ResponsableStage? ResponsableStage { get; set; }

    public Administrateur? Administrateur { get; set; }
}