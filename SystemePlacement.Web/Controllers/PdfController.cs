using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using SystemePlacement.Web.DTOs.Offres;
using SystemePlacement.Web.Services.Interfaces;
using System.Globalization;

namespace SystemePlacement.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PdfController : ControllerBase
{
    private static readonly CultureInfo CultureFrCa = new("fr-CA");

    private readonly IOffreService _offreService;

    static PdfController()
    {
        // Le projet est actuellement exécuté sous Windows.
        // PDFsharp Core doit être autorisé à résoudre les polices Windows.
        if (OperatingSystem.IsWindows())
        {
            GlobalFontSettings.UseWindowsFontsUnderWindows = true;
        }
    }

    public PdfController(IOffreService offreService)
    {
        _offreService = offreService;
    }

    [HttpGet("offre/{idOffre:int}")]
    public async Task<IActionResult> GenererPdfOffre(int idOffre)
    {
        var offre = await _offreService.GetByIdAsync(idOffre);

        if (offre is null)
            return NotFound();

        var pdfBytes = offre switch
        {
            OffreEmploiResponse emploi => GenererPdfEmploi(emploi),
            OffreStageResponse stage => GenererPdfStage(stage),
            _ => null
        };

        if (pdfBytes is null)
            return NotFound();

        return File(
            pdfBytes,
            "application/pdf",
            $"offre-{idOffre}.pdf");
    }

    private static byte[] GenererPdfEmploi(OffreEmploiResponse offre)
    {
        var (document, section) = CreerDocumentBase(
            offre.Titre,
            offre.NomEmployeur,
            offre.Ville,
            "Offre d'emploi",
            FormaterStatut(offre.Statut.ToString()));

        AjouterTitreSection(section, "Description");
        AjouterParagraphe(section, offre.Description);

        AjouterTitreSection(section, "Conditions");

        var table = CreerTableDetails(section);

        AjouterLigne(
            table,
            "Type de contrat",
            offre.TypeContrat ?? "Non spécifié");

        AjouterLigne(
            table,
            "Salaire",
            FormaterSalaire(
                offre.SalaireMin,
                offre.SalaireMax));

        AjouterLigne(
            table,
            "Télétravail",
            offre.TeleTravail ?? "Non spécifié");

        AjouterLigne(
            table,
            "Ville",
            offre.Ville);

        AjouterLigne(
            table,
            "Adresse",
            offre.Adresse ?? "Non spécifiée");

        AjouterLigne(
            table,
            "Publié le",
            FormaterDate(offre.DatePublication));

        AjouterLigne(
            table,
            "Expire le",
            FormaterDate(offre.DateExpiration, "Aucune date"));

        AjouterDomaines(section, offre.Domaines);

        return RendrePdf(document);
    }

    private static byte[] GenererPdfStage(OffreStageResponse offre)
    {
        var (document, section) = CreerDocumentBase(
            offre.Titre,
            offre.NomEmployeur,
            offre.Ville,
            "Offre de stage",
            FormaterStatut(offre.Statut.ToString()));

        AjouterTitreSection(section, "Description");
        AjouterParagraphe(section, offre.Description);

        AjouterTitreSection(section, "Détails du stage");

        var table = CreerTableDetails(section);

        AjouterLigne(
            table,
            "Session",
            offre.Session ?? "Non spécifiée");

        AjouterLigne(
            table,
            "Date de début",
            FormaterDate(
                offre.DateDebutStage,
                "Non spécifiée"));

        AjouterLigne(
            table,
            "Date de fin",
            FormaterDate(
                offre.DateFinStage,
                "Non spécifiée"));

        AjouterLigne(
            table,
            "Heures par semaine",
            offre.DureeHeuresParSemaine.HasValue
                ? $"{offre.DureeHeuresParSemaine.Value} h"
                : "Non spécifiées");

        AjouterLigne(
            table,
            "Rémunération",
            offre.Remuneration.HasValue
                ? $"{offre.Remuneration.Value.ToString("C", CultureFrCa)}/h"
                : "Non rémunéré");

        AjouterLigne(
            table,
            "Ville",
            offre.Ville);

        AjouterLigne(
            table,
            "Adresse",
            offre.Adresse ?? "Non spécifiée");

        AjouterLigne(
            table,
            "Publié le",
            FormaterDate(offre.DatePublication));

        AjouterLigne(
            table,
            "Expire le",
            FormaterDate(
                offre.DateExpiration,
                "Aucune date"));

        AjouterDomaines(section, offre.Domaines);

        return RendrePdf(document);
    }

    private static (Document Document, Section Section) CreerDocumentBase(
        string titre,
        string nomEmployeur,
        string ville,
        string typeOffre,
        string statut)
    {
        var document = new Document();

        document.Info.Title = titre;
        document.Info.Subject = typeOffre;
        document.Info.Author = "Système de placement";

        var normalStyle = document.Styles[StyleNames.Normal]
            ?? throw new InvalidOperationException(
                "Le style Normal de MigraDoc est introuvable.");
        normalStyle.Font.Name = "Arial";
        normalStyle.Font.Size = Unit.FromPoint(10);

        var section = document.AddSection();

        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.Orientation = Orientation.Portrait;
        section.PageSetup.TopMargin = Unit.FromCentimeter(1.7);
        section.PageSetup.BottomMargin = Unit.FromCentimeter(1.7);
        section.PageSetup.LeftMargin = Unit.FromCentimeter(2);
        section.PageSetup.RightMargin = Unit.FromCentimeter(2);

        var titreParagraphe = section.AddParagraph();
        titreParagraphe.Format.SpaceAfter =
            Unit.FromCentimeter(0.2);

        var titreTexte = titreParagraphe.AddFormattedText(
            titre,
            TextFormat.Bold);

        titreTexte.Font.Size = Unit.FromPoint(18);

        var employeurParagraphe = section.AddParagraph(
            $"{nomEmployeur} - {ville}");

        employeurParagraphe.Format.SpaceAfter =
            Unit.FromCentimeter(0.2);

        var typeParagraphe = section.AddParagraph();

        typeParagraphe.AddFormattedText(
            typeOffre,
            TextFormat.Bold);

        typeParagraphe.AddText($"  |  Statut : {statut}");

        typeParagraphe.Format.SpaceAfter =
            Unit.FromCentimeter(0.5);

        var footer = section.Footers.Primary.AddParagraph(
            $"Document généré le " +
            $"{DateTime.Now.ToString("d MMMM yyyy", CultureFrCa)} " +
            "- Système de placement Cégep");

        footer.Format.Alignment = ParagraphAlignment.Center;
        footer.Format.Font.Size = Unit.FromPoint(8);

        return (document, section);
    }

    private static void AjouterTitreSection(
        Section section,
        string titre)
    {
        var paragraphe = section.AddParagraph();

        paragraphe.Format.SpaceBefore =
            Unit.FromCentimeter(0.35);

        paragraphe.Format.SpaceAfter =
            Unit.FromCentimeter(0.15);

        var texte = paragraphe.AddFormattedText(
            titre,
            TextFormat.Bold);

        texte.Font.Size = Unit.FromPoint(12);
    }

    private static void AjouterParagraphe(
        Section section,
        string texte)
    {
        var paragraphe = section.AddParagraph(texte);

        paragraphe.Format.SpaceAfter =
            Unit.FromCentimeter(0.3);
    }

    private static Table CreerTableDetails(Section section)
    {
        var table = section.AddTable();

        table.Borders.Width = Unit.FromPoint(0.5);

        table.AddColumn(Unit.FromCentimeter(5));
        table.AddColumn(Unit.FromCentimeter(11));

        return table;
    }

    private static void AjouterLigne(
        Table table,
        string libelle,
        string valeur)
    {
        var row = table.AddRow();

        var libelleParagraphe = row[0].AddParagraph();

        libelleParagraphe.AddFormattedText(
            libelle,
            TextFormat.Bold);

        row[1].AddParagraph(valeur);
    }

    private static void AjouterDomaines(
        Section section,
        IReadOnlyCollection<string> domaines)
    {
        if (domaines.Count == 0)
            return;

        AjouterTitreSection(section, "Domaines d'études");

        AjouterParagraphe(
            section,
            string.Join(", ", domaines));
    }

    private static string FormaterStatut(string statut)
    {
        return statut switch
        {
            "Fermee" => "Fermée",
            _ => statut
        };
    }
    private static string FormaterDate(
        DateTime date)
    {
        return date.ToString(
            "d MMMM yyyy",
            CultureFrCa);
    }

    private static string FormaterDate(
        DateTime? date,
        string valeurAbsente)
    {
        return date.HasValue
            ? FormaterDate(date.Value)
            : valeurAbsente;
    }

    private static string FormaterSalaire(
        decimal? minimum,
        decimal? maximum)
    {
        if (minimum.HasValue && maximum.HasValue)
        {
            return
                $"{minimum.Value.ToString("C", CultureFrCa)} - " +
                $"{maximum.Value.ToString("C", CultureFrCa)}";
        }

        if (minimum.HasValue)
        {
            return minimum.Value.ToString(
                "C",
                CultureFrCa);
        }

        if (maximum.HasValue)
        {
            return maximum.Value.ToString(
                "C",
                CultureFrCa);
        }

        return "Non spécifié";
    }

    private static byte[] RendrePdf(Document document)
    {
        var renderer = new PdfDocumentRenderer
        {
            Document = document
        };

        renderer.RenderDocument();

        using var stream = new MemoryStream();

        renderer.PdfDocument.Save(
            stream,
            closeStream: false);

        return stream.ToArray();
    }
}