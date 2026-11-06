using System.Net;
using System.Net.Mail;
using System.Text;

namespace indigoERP.Worker.Services;

// Wysyłka jednej wiadomości (jeden adresat na wywołanie). Dostawca wybierany w Mail:Provider: Graph (domyślnie) albo Smtp.
public interface IWiadomoscMailSender
{
    bool IsConfigured { get; }
    Task SendAsync(GraphMailMessage message, CancellationToken cancellationToken);
}

public sealed class GraphWiadomoscMailSender(IMicrosoftGraphMailService graph) : IWiadomoscMailSender
{
    public bool IsConfigured => graph.IsConfigured;

    public Task SendAsync(GraphMailMessage message, CancellationToken cancellationToken) =>
        graph.SendAsync(message, cancellationToken);
}

// Ustawienia wyboru kanału (sekcja Mail:Graph). Wlaczony = false (domyślnie) zostawia całą wysyłkę na SMTP, tak jak dotychczas.
public sealed class MailRoutingOptions
{
    /// <summary>Włącza wysyłkę przez Graph dla nadawców z domeny <see cref="Domena"/>.</summary>
    public bool Wlaczony { get; init; }

    /// <summary>Domena skrzynek M365; nadawcy z tej domeny wysyłają przez Graph.</summary>
    public string Domena { get; init; } = "huta-czestochowa.pl";

    /// <summary>Opcjonalne zawężenie: gdy lista nie jest pusta, przez Graph wysyłają tylko wymienieni nadawcy z domeny.</summary>
    public string[] Nadawcy { get; init; } = [];
}

// Który kanał obsługuje danego nadawcę (potrzebne też do odczytu odpowiedzi o niedostarczeniu, która wraca do skrzynki nadawcy w M365).
public interface IKanalNadawcy
{
    bool UzywaGraph(string? nadawca);
}

// Kanał wysyłki wybierany po nadawcy: domena M365 (domyślnie huta-czestochowa.pl) -> Graph, wszystko inne (nasza domena SMTP
// hutaczestochowa.com.pl, domeny zewnętrzne, brak nadawcy) -> SMTP. Graph tylko po włączeniu w Mail:Graph:Wlaczony i przy pełnej konfiguracji.
public sealed class RoutingWiadomoscMailSender(IWiadomoscMailSender smtp, IWiadomoscMailSender graph, MailRoutingOptions opcje)
    : IWiadomoscMailSender, IKanalNadawcy
{
    public bool IsConfigured => smtp.IsConfigured || (opcje.Wlaczony && graph.IsConfigured);

    public bool UzywaGraph(string? nadawca)
    {
        if (!opcje.Wlaczony || !graph.IsConfigured || string.IsNullOrWhiteSpace(nadawca))
        {
            return false;
        }

        var adres = nadawca.Trim();
        var domena = "@" + opcje.Domena.Trim().TrimStart('@');

        if (!adres.EndsWith(domena, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return opcje.Nadawcy.Length == 0 || opcje.Nadawcy.Contains(adres, StringComparer.OrdinalIgnoreCase);
    }

    public Task SendAsync(GraphMailMessage message, CancellationToken cancellationToken) =>
        UzywaGraph(message.Nadawca) ? graph.SendAsync(message, cancellationToken) : smtp.SendAsync(message, cancellationToken);
}
public sealed class SmtpWiadomoscMailSender(IConfiguration configuration) : IWiadomoscMailSender
{
    // Hasło konta nadawcy to zmienna systemowa, nie wpis w appsettings.
    public const string PasswordVariable = "Informacje_Mail_Password";

    private string? Host => configuration["Mail:Smtp:Host"];
    private string? Nadawca => configuration["Mail:Smtp:From"];
    private string? Uzytkownik => configuration["Mail:Smtp:User"];
    private int Port => configuration.GetValue<int?>("Mail:Smtp:Port") ?? 25;

    private static string? Haslo =>
        Environment.GetEnvironmentVariable(PasswordVariable)
        ?? Environment.GetEnvironmentVariable(PasswordVariable, EnvironmentVariableTarget.Machine);

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Host)
        && !string.IsNullOrWhiteSpace(Nadawca)
        && !string.IsNullOrWhiteSpace(Uzytkownik)
        && !string.IsNullOrWhiteSpace(Haslo);

    public async Task SendAsync(GraphMailMessage message, CancellationToken cancellationToken)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException(
                $"Brak konfiguracji SMTP: Mail:Smtp:Host, Mail:Smtp:From, Mail:Smtp:User i zmienna {PasswordVariable}.");
        }

        if (message.Odbiorcy.Count == 0)
        {
            throw new InvalidOperationException("Wiadomość nie ma odbiorców.");
        }

        // Nadawca to konto skonfigurowane dla SMTP; adres z wiaEmail (Graph) serwer SMTP mógłby odrzucić.
        using var wiadomosc = new MailMessage
        {
            From = new MailAddress(Nadawca!),
            Subject = message.Temat,
            SubjectEncoding = Encoding.UTF8,
            Body = message.Tresc,
            BodyEncoding = Encoding.UTF8,
            IsBodyHtml = true
        };

        foreach (var odbiorca in message.Odbiorcy)
        {
            wiadomosc.To.Add(odbiorca);
        }

        foreach (var zalacznik in message.Zalaczniki)
        {
            wiadomosc.Attachments.Add(new Attachment(new MemoryStream(zalacznik.Zawartosc), zalacznik.Nazwa, zalacznik.ContentType));
        }

        using var klient = new SmtpClient(Host!, Port)
        {
            EnableSsl = false,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(Uzytkownik, Haslo),
            Timeout = 60000
        };

        await klient.SendMailAsync(wiadomosc, cancellationToken);
    }
}
