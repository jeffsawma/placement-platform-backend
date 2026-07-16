namespace SystemePlacement.Web.Models;

public class Etudiant
{
    public int IdEtudiant { get; set; }

    // Compte utilisateur lie a l'etudiant.
    // Le nom, prenom, courriel, role et college viennent de Utilisateur.
    public int IdUtilisateur { get; set; }

    public Utilisateur? Utilisateur { get; set; }

    // Numero etudiant interne au cegep.
    public string? NumeroEtudiant { get; set; }

    // Programme ou formation de l'etudiant.
    public string? Programme { get; set; }

    // Telephone de contact de l'etudiant.
    public string? Telephone { get; set; }

    // Lien ou chemin vers le CV principal de l'etudiant, si vous voulez le garder au profil.
    public string? CvUrl { get; set; }

    // Indique si l'etudiant cherche actuellement un stage.
    public bool CheminementATE { get; set; }

    // Statut general de l'etudiant : Actif, EnRecherche, Place, Diplome, etc.
    public string StatutEtudiant { get; set; } = "Actif";

    // Candidatures envoyees par l'etudiant.
    public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();

    // Demandes de stage formulees par l'etudiant.
    public ICollection<DemandeStage> DemandesStage { get; set; } = new List<DemandeStage>();
}