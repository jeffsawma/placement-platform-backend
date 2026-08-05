namespace SystemePlacement.Web.Models;

public class Etudiant
{
    public int IdEtudiant { get; set; }

    // Compte utilisateur lié à l'étudiant.
    // Le nom, prenom, courriel, role et college viennent de Utilisateur.
    public int IdUtilisateur { get; set; }

    public Utilisateur? Utilisateur { get; set; }

    // Numéro étudiant interne au cégep.
    public string? NumeroEtudiant { get; set; }

    // Programme ou formation de l'étudiant.
    public string? Programme { get; set; }

    // Téléphone de contact de l'étudiant.
    public string? Telephone { get; set; }

    // Lien ou chemin vers le CV principal de l'étudiant, si vous voulez le garder au profil.
    public string? CvUrl { get; set; }

    // Indique si l'étudiant cherche actuellement un stage.
    public bool CheminementATE { get; set; }

    // Statut général de l'étudiant : Actif, EnRecherche, Place, Diplome, etc.
    public string StatutEtudiant { get; set; } = "Actif";

    // Candidatures envoyées par l'étudiant.
    public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();

    // Demandes de stage formulées par l'étudiant.
    public ICollection<DemandeStage> DemandesStage { get; set; } = new List<DemandeStage>();
}