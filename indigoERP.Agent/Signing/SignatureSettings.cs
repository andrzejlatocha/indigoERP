using System.Text.Json;

namespace indigoERP.Agent.Signing;

/// <summary>Ustawienia podpisu z sekcji "Signing" w appsettings.json.</summary>
public sealed class SignatureSettings
{
    public bool ShowPlacementWindow { get; init; } = true;
    public int DefaultSignaturePage { get; init; } = 1;
    public double DefaultSignatureX { get; init; } = 400;
    public double DefaultSignatureY { get; init; } = 60;
    public double DefaultSignatureWidth { get; init; } = 180;
    public double DefaultSignatureHeight { get; init; } = 60;

    public static SignatureSettings Load(string path)
    {
        if (!File.Exists(path))
            return new SignatureSettings();

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        if (!document.RootElement.TryGetProperty("Signing", out var section))
            return new SignatureSettings();

        return new SignatureSettings
        {
            ShowPlacementWindow = section.TryGetProperty(nameof(ShowPlacementWindow), out var show) ? show.GetBoolean() : true,
            DefaultSignaturePage = ReadInt(section, nameof(DefaultSignaturePage), 1),
            DefaultSignatureX = ReadDouble(section, nameof(DefaultSignatureX), 400),
            DefaultSignatureY = ReadDouble(section, nameof(DefaultSignatureY), 60),
            DefaultSignatureWidth = ReadDouble(section, nameof(DefaultSignatureWidth), 180),
            DefaultSignatureHeight = ReadDouble(section, nameof(DefaultSignatureHeight), 60)
        };
    }

    private static int ReadInt(JsonElement section, string name, int fallback) =>
        section.TryGetProperty(name, out var value) ? value.GetInt32() : fallback;

    private static double ReadDouble(JsonElement section, string name, double fallback) =>
        section.TryGetProperty(name, out var value) ? value.GetDouble() : fallback;
}
