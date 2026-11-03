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

// Kanał wysyłki: na razie wszystko przez nasz serwer SMTP (decyzja o powrocie do Graph odłożona).
public sealed class RoutingWiadomoscMailSender(IWiadomoscMailSender smtp) : IWiadomoscMailSender
{
    private const string DomenaSmtp = "@hutaczestochowa.com.pl";

    public bool IsConfigured => smtp.IsConfigured;

    // Nadawcy z domeny SMTP nie mają skrzynki w M365, więc nie czytamy dla nich odpowiedzi NDR przez Graph.
    public static bool UzywaSmtp(string? nadawca) =>
        nadawca?.EndsWith(DomenaSmtp, StringComparison.OrdinalIgnoreCase) == true;

    public Task SendAsync(GraphMailMessage message, CancellationToken cancellationToken) =>
        smtp.SendAsync(message, cancellationToken);
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
