using System.Text.RegularExpressions;

namespace indigoERP.Worker.Services;

// Rozróżnia błędy przejściowe (limit Graph, timeout, niedostępność usługi), po których adresat zostaje w kolejce,
// od trwałych (np. nieprawidłowy nadawca lub adres), po których adresat dostaje status błędu.
public static partial class BledyWysylki
{
    [GeneratedRegex(@"\((429|500|502|503|504)\)")]
    private static partial Regex KodPrzejsciowy();

    public static bool Przejsciowy(Exception exception)
    {
        for (Exception? biezacy = exception; biezacy != null; biezacy = biezacy.InnerException)
        {
            // Polly zgłasza TimeoutRejectedException (osobny typ, nie dziedziczy po TimeoutException).
            if (biezacy is TimeoutException or TaskCanceledException or HttpRequestException
                || biezacy.GetType().Name == "TimeoutRejectedException")
            {
                return true;
            }

            if (KodPrzejsciowy().IsMatch(biezacy.Message))
            {
                return true;
            }
        }

        return false;
    }
}
