using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using V = DocumentFormat.OpenXml.Vml;

namespace indigoERP.Blazor.Utils;

// Zamienia treść pliku .docx na HTML do edytora wiadomości: akapity, nagłówki, pogrubienie, kursywa, podkreślenie,
// listy (punktowane), tabele i obrazy osadzone (jako data URI). Hiperłącza zostają jako sam tekst.
public static class WordHtmlConverter
{
    private static readonly string[] ObslugiwaneObrazy = ["image/png", "image/jpeg", "image/gif", "image/bmp"];

    private static readonly Regex NaglowekRegex = new(@"^(heading|nagwek|nagłówek)\s*([1-6])$", RegexOptions.IgnoreCase);
    private static readonly Regex TytulRegex = new(@"^(title|tytuł|tytul)$", RegexOptions.IgnoreCase);

    public static string KonwertujNaHtml(Stream docx)
    {
        using var dokument = WordprocessingDocument.Open(docx, false);
        var glowny = dokument.MainDocumentPart ?? throw new InvalidDataException("Plik nie zawiera dokumentu Word.");
        var tresc = glowny.Document?.Body ?? throw new InvalidDataException("Dokument nie zawiera treści.");
        var sb = new StringBuilder();
        var wListe = false;

        foreach (var element in tresc.Elements())
        {
            if (element is Paragraph akapitListy && JestElementemListy(akapitListy))
            {
                if (!wListe)
                {
                    sb.Append("<ul>");
                    wListe = true;
                }

                AppendAkapit(sb, akapitListy, glowny, lista: true);
                continue;
            }

            if (wListe)
            {
                sb.Append("</ul>");
                wListe = false;
            }

            switch (element)
            {
                case Paragraph akapit:
                    AppendAkapit(sb, akapit, glowny);
                    break;
                case Table tabela:
                    AppendTabela(sb, tabela, glowny);
                    break;
            }
        }

        if (wListe)
        {
            sb.Append("</ul>");
        }

        return sb.ToString();
    }

    private static bool JestElementemListy(Paragraph akapit) =>
        akapit.ParagraphProperties?.NumberingProperties != null && OkreslTag(StylAkapitu(akapit)) == "p";

    private static string? StylAkapitu(Paragraph akapit) => akapit.ParagraphProperties?.ParagraphStyleId?.Val?.Value;

    private static string OkreslTag(string? styl)
    {
        if (styl == null)
        {
            return "p";
        }

        var naglowek = NaglowekRegex.Match(styl);
        if (naglowek.Success)
        {
            return "h" + naglowek.Groups[2].Value;
        }

        return TytulRegex.IsMatch(styl) ? "h1" : "p";
    }

    private static void AppendAkapit(StringBuilder sb, Paragraph akapit, MainDocumentPart glowny, bool lista = false)
    {
        var tag = lista ? "li" : OkreslTag(StylAkapitu(akapit));
        var wyrownanie = WyrownanieStyl(akapit.ParagraphProperties?.Justification?.Val?.Value);

        var zawartosc = new StringBuilder();
        AppendRuny(zawartosc, akapit, glowny);

        sb.Append('<').Append(tag).Append(wyrownanie).Append('>');
        sb.Append(zawartosc.Length == 0 ? "<br>" : zawartosc.ToString());
        sb.Append("</").Append(tag).Append('>');
    }

    private static string WyrownanieStyl(JustificationValues? wartosc)
    {
        if (wartosc == null)
        {
            return string.Empty;
        }

        var css = wartosc == JustificationValues.Center ? "center"
            : wartosc == JustificationValues.Right ? "right"
            : wartosc == JustificationValues.Both ? "justify"
            : null;

        return css == null ? string.Empty : $" style=\"text-align:{css}\"";
    }

    private static void AppendRuny(StringBuilder sb, Paragraph akapit, MainDocumentPart glowny)
    {
        foreach (var run in akapit.Descendants<Run>())
        {
            var tekst = new StringBuilder();

            foreach (var element in run.Elements())
            {
                switch (element)
                {
                    case Text tekstWord:
                        tekst.Append(WebUtility.HtmlEncode(tekstWord.Text));
                        break;
                    case Break _:
                        tekst.Append("<br>");
                        break;
                    case TabChar _:
                        tekst.Append("&emsp;&emsp;");
                        break;
                    case Drawing rysunek:
                        AppendObraz(tekst, rysunek.Descendants<A.Blip>().FirstOrDefault()?.Embed?.Value, glowny);
                        break;
                    case Picture obrazVml:
                        AppendObraz(tekst, obrazVml.Descendants<V.ImageData>().FirstOrDefault()?.RelationshipId?.Value, glowny);
                        break;
                }
            }

            if (tekst.Length > 0)
            {
                sb.Append(Formatuj(tekst.ToString(), run.RunProperties));
            }
        }
    }

    private static string Formatuj(string tekst, RunProperties? wlasciwosci)
    {
        var wynik = tekst;

        if (wlasciwosci?.Underline?.Val?.Value is { } podkreslenie && podkreslenie != UnderlineValues.None)
        {
            wynik = $"<u>{wynik}</u>";
        }

        if (JestWlaczone(wlasciwosci?.Italic))
        {
            wynik = $"<em>{wynik}</em>";
        }

        if (JestWlaczone(wlasciwosci?.Bold))
        {
            wynik = $"<strong>{wynik}</strong>";
        }

        return wynik;
    }

    // Element bez wartości (np. <w:b/>) oznacza włączenie; <w:b w:val="0"/> oznacza wyłączenie.
    private static bool JestWlaczone(OnOffType? wlasciwosc) => wlasciwosc != null && (wlasciwosc.Val?.Value ?? true);

    private static void AppendObraz(StringBuilder sb, string? relacjaId, MainDocumentPart glowny)
    {
        if (relacjaId == null)
        {
            return;
        }

        var obraz = glowny.Parts.Where(czesc => czesc.RelationshipId == relacjaId)
            .Select(czesc => czesc.OpenXmlPart)
            .OfType<ImagePart>()
            .FirstOrDefault();

        if (obraz == null || !ObslugiwaneObrazy.Contains(obraz.ContentType))
        {
            return;
        }

        using var strumien = obraz.GetStream();
        using var bufor = new MemoryStream();
        strumien.CopyTo(bufor);

        sb.Append("<img src=\"data:").Append(obraz.ContentType).Append(";base64,")
            .Append(Convert.ToBase64String(bufor.ToArray())).Append("\" />");
    }

    private static void AppendTabela(StringBuilder sb, Table tabela, MainDocumentPart glowny)
    {
        sb.Append("<table style=\"border-collapse:collapse\">");

        foreach (var wiersz in tabela.Elements<TableRow>())
        {
            sb.Append("<tr>");

            foreach (var komorka in wiersz.Elements<TableCell>())
            {
                sb.Append("<td style=\"border:1px solid #999;padding:4px\">");

                foreach (var element in komorka.Elements())
                {
                    switch (element)
                    {
                        case Paragraph akapit:
                            AppendAkapit(sb, akapit, glowny);
                            break;
                        case Table zagniezdzona:
                            AppendTabela(sb, zagniezdzona, glowny);
                            break;
                    }
                }

                sb.Append("</td>");
            }

            sb.Append("</tr>");
        }

        sb.Append("</table>");
    }
}
