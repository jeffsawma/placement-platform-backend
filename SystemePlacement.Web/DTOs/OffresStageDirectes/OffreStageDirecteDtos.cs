using SystemePlacement.Web.Enums;

namespace SystemePlacement.Web.DTOs.OffresStageDirectes
{
    public class CreerOffreStageDirecteRequest
    {
        public int IdEtudiant { get; set; }

        public int? IdOffreStage { get; set; }
        public int? IdCandidature { get; set; }
        public int? IdDemandeStage { get; set; }

        public string Conditions { get; set; } = string.Empty;

        public DateTime? DateDebutProposee { get; set; }
        public DateTime? DateFinProposee { get; set; }

        public string? Commentaire { get; set; }
    }

    public class OffreStageDirecteReponse
    {
        public int IdOffreDirecte { get; set; }
        public string NomEtudiant { get; set; } = string.Empty;
        public string PrenomEtudiant { get; set; } = string.Empty;
        public string? CourrielEtudiant { get; set; }

        public int? IdOffreStage { get; set; }
        public int? IdCandidature { get; set; }
        public int? IdDemandeStage { get; set; }

        public string Conditions { get; set; } = string.Empty;

        public DateTime? DateDebutProposee { get; set; }
        public DateTime? DateFinProposee { get; set; }

        public DateTime DateProposition { get; set; }

        public StatutOffreStageDirecte Statut { get; set; }
        public string? Commentaire { get; set; }
    }
}
