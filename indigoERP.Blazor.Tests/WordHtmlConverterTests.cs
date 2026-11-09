using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using indigoERP.Blazor.Utils;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace indigoERP.Blazor.Tests;

public class WordHtmlConverterTests
{
    [Fact]
    public void Konwertuje_pogrubienie_i_zamienia_znaki_specjalne()
    {
        var html = Konwertuj(body => body.Append(
            new Paragraph(
                new Run(new RunProperties(new Bold()), new Text("Pogrubione")),
                new Run(new Text(" & zwykły <tekst>")))));

        Assert.Equal("<p><strong>Pogrubione</strong> &amp; zwykły &lt;tekst&gt;</p>", html);
    }

    [Fact]
    public void Konwertuje_naglowek_wedlug_stylu()
    {
        var html = Konwertuj(body => body.Append(
            new Paragraph(
                new ParagraphProperties(new ParagraphStyleId { Val = "Nagwek2" }),
                new Run(new Text("Tytuł sekcji")))));

        Assert.Equal("<h2>Tytuł sekcji</h2>", html);
    }

    [Fact]
    public void Konwertuje_tabele()
    {
        var html = Konwertuj(body => body.Append(
            new Table(
                new TableRow(
                    new TableCell(new Paragraph(new Run(new Text("A")))),
                    new TableCell(new Paragraph(new Run(new Text("B"))))))));

        Assert.Contains("<table style=\"border-collapse:collapse\">", html);
        Assert.Contains("<td style=\"border:1px solid #999;padding:4px\"><p>A</p></td>", html);
        Assert.Contains("<td style=\"border:1px solid #999;padding:4px\"><p>B</p></td>", html);
    }

    [Fact]
    public void Osadza_obraz_jako_data_uri()
    {
        using var stream = new MemoryStream();
        using (var dokument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
        {
            var glowny = dokument.AddMainDocumentPart();
            glowny.Document = new Document(new Body());
            var obraz = glowny.AddImagePart(ImagePartType.Png);
            obraz.FeedData(new MemoryStream(new byte[] { 1, 2, 3 }));
            var relacja = glowny.GetIdOfPart(obraz);

            var rysunek = new Drawing(new DW.Inline(new A.Graphic(new A.GraphicData(
                new PIC.Picture(new PIC.BlipFill(new A.Blip { Embed = relacja }))))));
            glowny.Document.Body!.Append(new Paragraph(new Run(rysunek)));
            glowny.Document.Save();
        }

        stream.Position = 0;
        var html = WordHtmlConverter.KonwertujNaHtml(stream);

        Assert.Contains("<img src=\"data:image/png;base64,AQID\" />", html);
    }

    private static string Konwertuj(Action<Body> dodajTresc)
    {
        using var stream = new MemoryStream();
        using (var dokument = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
        {
            var glowny = dokument.AddMainDocumentPart();
            var body = new Body();
            dodajTresc(body);
            glowny.Document = new Document(body);
            glowny.Document.Save();
        }

        stream.Position = 0;

        return WordHtmlConverter.KonwertujNaHtml(stream);
    }
}
