using indigoERP.Agent.Forms;
using indigoERP.Agent.Signing;

namespace indigoERP.Agent;

internal static class Program
{
    private const string SettingsFile = "appsettings.json";

    /// <summary>Użycie: indigoERP.Agent &lt;wejście.pdf&gt; &lt;wyjście.pdf&gt; &lt;numer seryjny certyfikatu&gt; &lt;obraz podpisu&gt;</summary>
    [STAThread]
    private static int Main(string[] args)
    {
        ApplicationConfiguration.Initialize();

        if (args.Length != 4)
        {
            MessageBox.Show("Użycie: indigoERP.Agent <wejście.pdf> <wyjście.pdf> <numer seryjny> <obraz podpisu>",
                "indigoERP.Agent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return 1;
        }

        var (input, output, serial, image) = (args[0], args[1], args[2], args[3]);
        var settings = SignatureSettings.Load(Path.Combine(AppContext.BaseDirectory, SettingsFile));

        SignaturePlacement? placement;
        string? password = null;

        if (settings.ShowPlacementWindow)
        {
            using var form = new PdfPlacementForm(input, settings);
            if (form.ShowDialog() != DialogResult.OK || form.Placement is null)
                return 2;

            placement = form.Placement;
            password = form.Password;
        }
        else
        {
            placement = DefaultPlacement(settings);
        }

        try
        {
            new PdfSignatureService().Sign(
                input,
                output,
                serial,
                File.ReadAllBytes(image),
                placement,
                reason: "Podpisano",
                location: "Polska, Częstochowa",
                password);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Nie udało się podpisać PDF: {ex.Message}", "indigoERP.Agent", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return 3;
        }

        return 0;
    }

    private static SignaturePlacement DefaultPlacement(SignatureSettings settings) =>
        new(settings.DefaultSignaturePage, new DevExpress.Pdf.PdfRectangle(
            settings.DefaultSignatureX,
            settings.DefaultSignatureY,
            settings.DefaultSignatureX + settings.DefaultSignatureWidth,
            settings.DefaultSignatureY + settings.DefaultSignatureHeight));
}
