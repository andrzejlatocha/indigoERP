using System.Security.Principal;

namespace indigoERP.Worker.Services;

// Diagnostyka braku pliku załącznika: konto procesu, każdy katalog po drodze, zawartość katalogu i wynik próby odczytu.
public static class SciezkaDiagnostyka
{
    public static string Opisz(string path)
    {
        var linie = new List<string>
        {
            $"Konto procesu: {WindowsIdentity.GetCurrent().Name}",
            $"Ścieżka: {path}",
            $"File.Exists: {File.Exists(path)}"
        };

        var katalogi = new List<string>();

        for (var katalog = Path.GetDirectoryName(path); katalog != null; katalog = Path.GetDirectoryName(katalog))
        {
            katalogi.Insert(0, katalog);
        }

        foreach (var katalog in katalogi)
        {
            linie.Add($"Katalog {Directory.Exists(katalog)}: {katalog}");
        }

        var folder = Path.GetDirectoryName(path);

        if (folder != null && Directory.Exists(folder))
        {
            try
            {
                var pliki = Directory.GetFileSystemEntries(folder).Take(10).Select(Path.GetFileName);
                linie.Add($"Zawartość katalogu: {string.Join(", ", pliki)}");
            }
            catch (Exception exception)
            {
                linie.Add($"Zawartość katalogu: {exception.GetType().Name}: {exception.Message}");
            }
        }

        try
        {
            using var strumien = File.OpenRead(path);
            linie.Add("Próba odczytu: OK");
        }
        catch (Exception exception)
        {
            linie.Add($"Próba odczytu: {exception.GetType().Name}: {exception.Message}");
        }

        return string.Join(Environment.NewLine, linie);
    }
}
