namespace SystemePlacement.Web.Models;

public class OffreDomaine
{
    public int IdOffre { get; set; }

    public int IdDomaine { get; set; }

    public Offre? Offre { get; set; }

    public DomaineEtude? DomaineEtude { get; set; }
}
