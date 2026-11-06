using DevExpress.Pdf;
using DevExpress.XtraPdfViewer;
using indigoERP.Agent.Signing;

namespace indigoERP.Agent.Forms;

/// <summary>
/// Podgląd PDF: kliknięcie ustawia lewy dolny róg podpisu, prostokąt ma rozmiar z konfiguracji.
/// Operator może podać hasło do PDF (puste = bez zabezpieczenia). Wynik w <see cref="Placement"/> i <see cref="Password"/>.
/// </summary>
public sealed class PdfPlacementForm : Form
{
    private readonly PdfViewer _viewer = new() { Dock = DockStyle.Fill };
    private readonly TextBox _passwordBox = new() { UseSystemPasswordChar = true, Width = 200 };
    private readonly double _width;
    private readonly double _height;

    public SignaturePlacement? Placement { get; private set; }

    public string? Password => string.IsNullOrEmpty(_passwordBox.Text) ? null : _passwordBox.Text;

    public PdfPlacementForm(string pdfPath, SignatureSettings settings)
    {
        _width = settings.DefaultSignatureWidth;
        _height = settings.DefaultSignatureHeight;

        Text = "Wybierz miejsce podpisu - kliknij lewy dolny róg";
        Width = 900;
        Height = 1000;
        StartPosition = FormStartPosition.CenterScreen;

        var passwordLabel = new Label { Text = "Hasło do PDF (puste = bez zabezpieczenia):", AutoSize = true };
        var panel = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 36, Padding = new Padding(6) };
        panel.Controls.Add(passwordLabel);
        panel.Controls.Add(_passwordBox);

        var hint = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 28,
            Text = "Kliknij w dokumencie lewy dolny róg podpisu. Esc - anuluj."
        };

        _viewer.LoadDocument(pdfPath);
        _viewer.MouseClick += OnViewerMouseClick;
        KeyPreview = true;
        KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) DialogResult = DialogResult.Cancel; };

        Controls.Add(_viewer);
        Controls.Add(hint);
        Controls.Add(panel);
    }

    private void OnViewerMouseClick(object? sender, MouseEventArgs e)
    {
        PdfDocumentPosition position = _viewer.GetDocumentPosition(new PointF(e.X, e.Y));
        if (position.PageNumber < 1)
            return;

        var left = position.Point.X;
        var bottom = position.Point.Y;
        Placement = new SignaturePlacement(position.PageNumber, new PdfRectangle(left, bottom, left + _width, bottom + _height));
        DialogResult = DialogResult.OK;
    }
}
