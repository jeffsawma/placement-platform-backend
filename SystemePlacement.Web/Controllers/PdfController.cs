using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemePlacement.Web.DTOs.Offres;
using SystemePlacement.Web.Services.Interfaces;
using System.Globalization;
using System.Text;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PdfController : ControllerBase
{
    private readonly IOffreService _offreService;

    public PdfController(IOffreService offreService) => _offreService = offreService;

    [HttpGet("offre/{idOffre:int}")]
    public async Task<IActionResult> GenererPdfOffre(int idOffre)
    {
        var offre = await _offreService.GetByIdAsync(idOffre);
        if (offre is null) return NotFound();

        var html = offre switch
        {
            OffreEmploiResponse emploi => GenererHtmlEmploi(emploi),
            OffreStageResponse stage   => GenererHtmlStage(stage),
            _                          => string.Empty
        };

        if (string.IsNullOrEmpty(html)) return NotFound();

        var pdfBytes = Encoding.UTF8.GetBytes(html);
        return File(pdfBytes, "text/html", "offre-" + idOffre + ".html");
    }

    private static string GenererHtmlEmploi(OffreEmploiResponse o)
    {
        var culture = new CultureInfo("fr-CA");
        var datePublication = o.DatePublication.ToString("d MMMM yyyy", culture);
        var dateExpiration = o.DateExpiration.HasValue
            ? o.DateExpiration.Value.ToString("d MMMM yyyy", culture)
            : "Aucune";
        var salaire = o.SalaireMin.HasValue
            ? o.SalaireMin.Value.ToString("C", culture) + (o.SalaireMax.HasValue ? " - " + o.SalaireMax.Value.ToString("C", culture) : "")
            : "Non specifie";
        var adresseLigne = o.Adresse is not null
            ? "<dt>Adresse</dt><dd>" + o.Adresse + "</dd>"
            : "";

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"fr\">");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset=\"UTF-8\"/>");
        sb.AppendLine("  <title>" + o.Titre + "</title>");
        sb.AppendLine("  <style>");
        sb.AppendLine("    body { font-family: Arial, sans-serif; color: #1a1a1a; margin: 40px; }");
        sb.AppendLine("    .entete { background: #003D7D; color: #fff; padding: 24px 32px; border-radius: 6px; margin-bottom: 32px; }");
        sb.AppendLine("    .entete h1 { margin: 0 0 8px; font-size: 22px; }");
        sb.AppendLine("    .entete p { margin: 0; font-size: 14px; opacity: 0.85; }");
        sb.AppendLine("    .badge { display: inline-block; padding: 3px 12px; border-radius: 20px; font-size: 12px; font-weight: 700; background: #E8EFF8; color: #003D7D; margin-right: 8px; }");
        sb.AppendLine("    .section { margin-bottom: 24px; }");
        sb.AppendLine("    .section h2 { font-size: 14px; font-weight: 700; color: #003D7D; border-bottom: 2px solid #003D7D; padding-bottom: 6px; margin-bottom: 12px; }");
        sb.AppendLine("    .section p { margin: 0; font-size: 13px; line-height: 1.6; white-space: pre-wrap; }");
        sb.AppendLine("    dl { display: grid; grid-template-columns: auto 1fr; gap: 6px 20px; font-size: 13px; }");
        sb.AppendLine("    dt { font-weight: 700; color: #003D7D; }");
        sb.AppendLine("    dd { margin: 0; }");
        sb.AppendLine("    .pied { margin-top: 48px; font-size: 11px; color: #888; border-top: 1px solid #ddd; padding-top: 12px; }");
        sb.AppendLine("    @media print { .no-print { display: none; } }");
        sb.AppendLine("  </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("  <div class=\"entete\">");
        sb.AppendLine("    <h1>" + o.Titre + "</h1>");
        sb.AppendLine("    <p>" + o.NomEmployeur + " &mdash; " + o.Ville + "</p>");
        sb.AppendLine("  </div>");
        sb.AppendLine("  <div class=\"section\">");
        sb.AppendLine("    <span class=\"badge\">Emploi</span>");
        sb.AppendLine("    <span class=\"badge\">" + o.Statut + "</span>");
        sb.AppendLine("  </div>");
        sb.AppendLine("  <div class=\"section\">");
        sb.AppendLine("    <h2>Description</h2>");
        sb.AppendLine("    <p>" + o.Description + "</p>");
        sb.AppendLine("  </div>");
        sb.AppendLine("  <div class=\"section\">");
        sb.AppendLine("    <h2>Conditions</h2>");
        sb.AppendLine("    <dl>");
        sb.AppendLine("      <dt>Type de contrat</dt><dd>" + (o.TypeContrat ?? "Non specifie") + "</dd>");
        sb.AppendLine("      <dt>Salaire</dt><dd>" + salaire + "</dd>");
        sb.AppendLine("      <dt>Teletravail</dt><dd>" + (o.TeleTravail ?? "Non specifie") + "</dd>");
        sb.AppendLine("      <dt>Ville</dt><dd>" + o.Ville + "</dd>");
        sb.AppendLine("      " + adresseLigne);
        sb.AppendLine("      <dt>Publie le</dt><dd>" + datePublication + "</dd>");
        sb.AppendLine("      <dt>Expire le</dt><dd>" + dateExpiration + "</dd>");
        sb.AppendLine("    </dl>");
        sb.AppendLine("  </div>");
        sb.AppendLine("  <div class=\"no-print\" style=\"margin-top:24px;\">");
        sb.AppendLine("    <button onclick=\"window.print()\" style=\"padding:10px 24px;background:#003D7D;color:#fff;border:none;border-radius:6px;font-size:14px;cursor:pointer;\">Imprimer</button>");
        sb.AppendLine("  </div>");
        sb.AppendLine("  <div class=\"pied\">");
        sb.AppendLine("    Document genere le " + DateTime.Now.ToString("d MMMM yyyy", culture) + " &mdash; Systeme de placement Cegep");
        sb.AppendLine("  </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }

    private static string GenererHtmlStage(OffreStageResponse o)
    {
        var culture = new CultureInfo("fr-CA");
        var datePublication = o.DatePublication.ToString("d MMMM yyyy", culture);
        var dateExpiration = o.DateExpiration.HasValue
            ? o.DateExpiration.Value.ToString("d MMMM yyyy", culture)
            : "Aucune";
        var dateDebut = o.DateDebutStage.HasValue
            ? o.DateDebutStage.Value.ToString("d MMMM yyyy", culture)
            : "Non specifie";
        var dateFin = o.DateFinStage.HasValue
            ? o.DateFinStage.Value.ToString("d MMMM yyyy", culture)
            : "Non specifie";
        var heures = o.DureeHeuresParSemaine.HasValue
            ? o.DureeHeuresParSemaine.Value + " h"
            : "Non specifie";
        var remuneration = o.Remuneration.HasValue
            ? o.Remuneration.Value.ToString("C", culture) + "/h"
            : "Non remunere";
        var adresseLigne = o.Adresse is not null
            ? "<dt>Adresse</dt><dd>" + o.Adresse + "</dd>"
            : "";
        var sessionLigne = o.Session is not null
            ? "<span class=\"badge\">" + o.Session + "</span>"
            : "";

        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html lang=\"fr\">");
        sb.AppendLine("<head>");
        sb.AppendLine("  <meta charset=\"UTF-8\"/>");
        sb.AppendLine("  <title>" + o.Titre + "</title>");
        sb.AppendLine("  <style>");
        sb.AppendLine("    body { font-family: Arial, sans-serif; color: #1a1a1a; margin: 40px; }");
        sb.AppendLine("    .entete { background: #003D7D; color: #fff; padding: 24px 32px; border-radius: 6px; margin-bottom: 32px; }");
        sb.AppendLine("    .entete h1 { margin: 0 0 8px; font-size: 22px; }");
        sb.AppendLine("    .entete p { margin: 0; font-size: 14px; opacity: 0.85; }");
        sb.AppendLine("    .badge { display: inline-block; padding: 3px 12px; border-radius: 20px; font-size: 12px; font-weight: 700; background: #E8EFF8; color: #003D7D; margin-right: 8px; }");
        sb.AppendLine("    .section { margin-bottom: 24px; }");
        sb.AppendLine("    .section h2 { font-size: 14px; font-weight: 700; color: #003D7D; border-bottom: 2px solid #003D7D; padding-bottom: 6px; margin-bottom: 12px; }");
        sb.AppendLine("    .section p { margin: 0; font-size: 13px; line-height: 1.6; white-space: pre-wrap; }");
        sb.AppendLine("    dl { display: grid; grid-template-columns: auto 1fr; gap: 6px 20px; font-size: 13px; }");
        sb.AppendLine("    dt { font-weight: 700; color: #003D7D; }");
        sb.AppendLine("    dd { margin: 0; }");
        sb.AppendLine("    .pied { margin-top: 48px; font-size: 11px; color: #888; border-top: 1px solid #ddd; padding-top: 12px; }");
        sb.AppendLine("    @media print { .no-print { display: none; } }");
        sb.AppendLine("  </style>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("  <div class=\"entete\">");
        sb.AppendLine("    <h1>" + o.Titre + "</h1>");
        sb.AppendLine("    <p>" + o.NomEmployeur + " &mdash; " + o.Ville + "</p>");
        sb.AppendLine("  </div>");
        sb.AppendLine("  <div class=\"section\">");
        sb.AppendLine("    <span class=\"badge\">Stage</span>");
        sb.AppendLine("    <span class=\"badge\">" + o.Statut + "</span>");
        sb.AppendLine("    " + sessionLigne);
        sb.AppendLine("  </div>");
        sb.AppendLine("  <div class=\"section\">");
        sb.AppendLine("    <h2>Description</h2>");
        sb.AppendLine("    <p>" + o.Description + "</p>");
        sb.AppendLine("  </div>");
        sb.AppendLine("  <div class=\"section\">");
        sb.AppendLine("    <h2>Details du stage</h2>");
        sb.AppendLine("    <dl>");
        sb.AppendLine("      <dt>Debut</dt><dd>" + dateDebut + "</dd>");
        sb.AppendLine("      <dt>Fin</dt><dd>" + dateFin + "</dd>");
        sb.AppendLine("      <dt>Heures / semaine</dt><dd>" + heures + "</dd>");
        sb.AppendLine("      <dt>Remuneration</dt><dd>" + remuneration + "</dd>");
        sb.AppendLine("      <dt>Ville</dt><dd>" + o.Ville + "</dd>");
        sb.AppendLine("      " + adresseLigne);
        sb.AppendLine("      <dt>Publie le</dt><dd>" + datePublication + "</dd>");
        sb.AppendLine("      <dt>Expire le</dt><dd>" + dateExpiration + "</dd>");
        sb.AppendLine("    </dl>");
        sb.AppendLine("  </div>");
        sb.AppendLine("  <div class=\"no-print\" style=\"margin-top:24px;\">");
        sb.AppendLine("    <button onclick=\"window.print()\" style=\"padding:10px 24px;background:#003D7D;color:#fff;border:none;border-radius:6px;font-size:14px;cursor:pointer;\">Imprimer</button>");
        sb.AppendLine("  </div>");
        sb.AppendLine("  <div class=\"pied\">");
        sb.AppendLine("    Document genere le " + DateTime.Now.ToString("d MMMM yyyy", culture) + " &mdash; Systeme de placement Cegep");
        sb.AppendLine("  </div>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }
}