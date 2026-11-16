using System.Data;
using indigoERP.Domain.Entities._Wspolne;
using indigoERP.Worker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace indigoERP.Worker.Services;

public sealed class WiadomoscKolejkaService
{
    private const string StatusAnulowana = "1";
    private const string StatusNowa = "2";
    private const string StatusRozpoczeta = "3";
    private const string StatusZakonczona = "4";
    private const string StatusBlad = "5";
    // Harmonogram: szablon wysyłki (7) jest kopiowany na zwykłą wiadomość (2) w terminie; po zakończeniu cyklu status 8.
    private const string StatusHarmonogram = "7";
    private const string StatusHarmonogramZakonczony = "8";
    private readonly WiadomoscWorkerContextFactory _contextFactory;
    private readonly IMicrosoftGraphMailService _mailService;
    private readonly IWiadomoscMailSender _sender;
    private readonly IKanalNadawcy _kanal;
    private readonly IConfiguration _configuration;
    private readonly IOptions<WorkerOptions> _options;
    private readonly ILogger<WiadomoscKolejkaService> _logger;

    public WiadomoscKolejkaService(
        WiadomoscWorkerContextFactory contextFactory,
        IMicrosoftGraphMailService mailService,
        IWiadomoscMailSender sender,
        IKanalNadawcy kanal,
        IConfiguration configuration,
        IOptions<WorkerOptions> options,
        ILogger<WiadomoscKolejkaService> logger)
    {
        _contextFactory = contextFactory;
        _mailService = mailService;
        _sender = sender;
        _kanal = kanal;
        _configuration = configuration;
        _options = options;
        _logger = logger;
    }

    public async Task<int> ProcessAsync(CancellationToken cancellationToken)
    {
        await ZaplanujHarmonogramyAsync(cancellationToken);

        if (!_sender.IsConfigured)
        {
            _logger.LogError(
                "Wysyłka wstrzymana: żaden kanał poczty nie jest skonfigurowany (MicrosoftGraph_* albo Mail:Smtp z Informacje_Mail_Password).");
            return 0;
        }

        var processed = 0;
        var batchSize = Math.Clamp(_options.Value.BatchSize, 1, 100);

        for (var index = 0; index < batchSize; index++)
        {
            var message = await ClaimNextAsync(cancellationToken);

            if (message == null)
            {
                break;
            }

            await ProcessMessageAsync(message.wiaID, cancellationToken);
            processed++;
        }

        return processed;
    }

    // Odpowiedzi o niedostarczeniu wracają do skrzynki nadawcy. Adresat, którego adres występuje w takiej odpowiedzi
    // po wysłaniu wiadomości, dostaje status StatusBlad. Sprawdzamy wiadomości wysłane w ostatnich 7 dniach.
    public async Task OznaczNiedostarczoneAsync(CancellationToken cancellationToken)
    {
        if (!_mailService.IsConfigured)
        {
            return;
        }

        await using var context = _contextFactory.CreateDbContext();
        var od = DateTime.Now.AddDays(-7);

        var wyslane = await context.Wiadomosci
            .Where(item => item.wiaStatus == StatusZakonczona && item.wiaEmail != null && item.wiaEmail != ""
                && item.wiaData != null && item.wiaData >= od && (item.Usuniety == null || item.Usuniety == false))
            .ToListAsync(cancellationToken);

        // Odpowiedź o niedostarczeniu wraca do skrzynki nadawcy tylko przy wysyłce z jego konta (Graph); przy SMTP trafia gdzie indziej.
        wyslane = wyslane.Where(item => _kanal.UzywaGraph(item.wiaEmail)).ToList();

        if (wyslane.Count == 0)
        {
            return;
        }

        var wiadomoscIDs = wyslane.Select(item => item.wiaID).ToList();
        var odbiorcy = await context.WiadomoscPracownicy
            .Where(item => item.wprWiadomoscID != null && wiadomoscIDs.Contains(item.wprWiadomoscID.Value)
                && item.wprStatus == StatusZakonczona && item.wprBlad == null && item.wprEmail != null && item.wprEmail != "")
            .ToListAsync(cancellationToken);

        var oznaczone = 0;

        foreach (var grupa in wyslane.Where(item => odbiorcy.Any(o => o.wprWiadomoscID == item.wiaID)).GroupBy(item => item.wiaEmail!))
        {
            var najstarsza = grupa.Min(item => item.wiaData!.Value);
            IReadOnlyList<UndeliveryNotice> powiadomienia;

            try
            {
                powiadomienia = await _mailService.GetUndeliveryNoticesAsync(grupa.Key, new DateTimeOffset(najstarsza), cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Nie udało się odczytać odpowiedzi o niedostarczeniu z konta {Nadawca}.", grupa.Key);
                continue;
            }

            foreach (var wiadomosc in grupa)
            {
                var adresaci = odbiorcy.Where(item => item.wprWiadomoscID == wiadomosc.wiaID).ToList();
                var wyslano = new DateTimeOffset(wiadomosc.wiaData!.Value);

                foreach (var powiadomienie in powiadomienia.Where(item => item.Received >= wyslano))
                {
                    var dopasowane = NdrDopasowanie.AdresyWTresci(
                        powiadomienie.Tresc,
                        adresaci.Select(item => item.wprEmail!).Distinct(StringComparer.OrdinalIgnoreCase));

                    foreach (var adres in dopasowane)
                    {
                        var wpr = adresaci.FirstOrDefault(item =>
                            string.Equals(item.wprEmail, adres, StringComparison.OrdinalIgnoreCase) && item.wprStatus == StatusZakonczona);
                        if (wpr == null)
                        {
                            continue;
                        }

                        wpr.wprStatus = StatusBlad;
                        wpr.wprBlad = $"Odpowiedź o niedostarczeniu z {powiadomienie.Received.LocalDateTime:dd.MM.yyyy HH:mm}";
                        oznaczone++;
                    }
                }
            }
        }

        if (oznaczone > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("Oznaczono {Ile} adresatów jako niedostarczone na podstawie odpowiedzi o niedostarczeniu.", oznaczone);
        }
    }

    // Kopiuje szablony harmonogramu w terminie na zwykłe wiadomości kolejki (z odbiorcami i załącznikami)
    // i przelicza następny termin. Przegapione terminy (np. po przestoju Workera) są pomijane, bez zalewu wysyłek.
    private async Task<int> ZaplanujHarmonogramyAsync(CancellationToken cancellationToken)
    {
        var teraz = DateTime.Now;
        var utworzone = 0;

        await using var context = _contextFactory.CreateDbContext();
        var szablony = await context.Wiadomosci
            .Where(w => w.wiaStatus == StatusHarmonogram && w.wiaHarmonogramAktywny
                && w.wiaDataNastepnejWysylki <= teraz && w.Usuniety != true)
            .ToListAsync(cancellationToken);

        foreach (var szablon in szablony)
        {
            await using var transakcja = await context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            var kopia = new Wiadomosc
            {
                wiaWiadomoscID = szablon.wiaID,
                wiaAplikacjaID = szablon.wiaAplikacjaID,
                wiaUzytkownikID = szablon.wiaUzytkownikID,
                wiaEmail = szablon.wiaEmail,
                wiaTemat = szablon.wiaTemat,
                wiaTresc = szablon.wiaTresc,
                wiaWzorzecID = szablon.wiaWzorzecID,
                wiaGrupa = szablon.wiaGrupa,
                wiaSesja = szablon.wiaSesja,
                wiaPubliczna = szablon.wiaPubliczna,
                wiaStatus = StatusNowa,
                DataUtworzenie = teraz,
                UtworzenieID = 0,
                DataModyfikacja = teraz,
                ModyfikacjaID = 0,
                Usuniety = false,
            };
            context.Wiadomosci.Add(kopia);
            await context.SaveChangesAsync(cancellationToken);

            // Odbiorcy ręczni (wiadomości publiczne) są w WiadomoscPracownik; wysyłkę (WiadomoscWysylka) Worker tworzy sam przy wysyłce.
            var odbiorcy = await context.WiadomoscPracownicy
                .Where(w => w.wprWiadomoscID == szablon.wiaID && w.wprStatus == StatusHarmonogram)
                .ToListAsync(cancellationToken);

            foreach (var odbiorca in odbiorcy)
            {
                context.WiadomoscPracownicy.Add(new WiadomoscPracownik
                {
                    wprWiadomoscID = kopia.wiaID,
                    wprPracownikID = odbiorca.wprPracownikID,
                    wprEmail = odbiorca.wprEmail,
                    wprStatus = StatusNowa,
                });
            }

            var zalaczniki = await context.WiadomoscDokumenty
                .Where(d => d.wdoWiadomoscID == szablon.wiaID)
                .ToListAsync(cancellationToken);

            foreach (var zalacznik in zalaczniki)
            {
                context.WiadomoscDokumenty.Add(new WiadomoscDokument
                {
                    wdoWiadomoscID = kopia.wiaID,
                    wdoTyp = zalacznik.wdoTyp,
                    wdoNazwa = zalacznik.wdoNazwa,
                    wdoPlik = zalacznik.wdoPlik,
                    wdoKatalog = zalacznik.wdoKatalog,
                    wdoPlikID = zalacznik.wdoPlikID,
                    wdoLink = zalacznik.wdoLink,
                    wdoStatus = zalacznik.wdoStatus,
                });
            }

            var nastepny = szablon.wiaDataNastepnejWysylki!.Value;
            var kod = szablon.wiaKodHarmonogram ?? "J";

            do
            {
                nastepny = NastepnyTermin(kod, nastepny);
            }
            while (nastepny <= teraz && nastepny != DateTime.MaxValue);

            var koniec = szablon.wiaDataHarmonogramKoniec;

            if (nastepny == DateTime.MaxValue || (koniec != null && nastepny > koniec))
            {
                szablon.wiaHarmonogramAktywny = false;
                szablon.wiaStatus = StatusHarmonogramZakonczony;
                szablon.wiaDataNastepnejWysylki = null;
            }
            else
            {
                szablon.wiaDataNastepnejWysylki = nastepny;
            }

            szablon.DataModyfikacja = teraz;
            szablon.ModyfikacjaID = 0;

            await context.SaveChangesAsync(cancellationToken);
            await transakcja.CommitAsync(cancellationToken);
            utworzone++;

            _logger.LogInformation("Harmonogram wiadomości {WiaID}: utworzono wysyłkę {KopiaID}.", szablon.wiaID, kopia.wiaID);
        }

        return utworzone;
    }

    // Cykl: J = jednorazowo (brak kolejnego terminu), D = dziennie, T = tygodniowo, M = miesięcznie.
    private static DateTime NastepnyTermin(string kod, DateTime poprzedni)
    {
        return kod switch
        {
            "D" => poprzedni.AddDays(1),
            "T" => poprzedni.AddDays(7),
            "M" => poprzedni.AddMonths(1),
            _ => DateTime.MaxValue,
        };
    }

    private async Task<Wiadomosc?> ClaimNextAsync(CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        var retryBefore = DateTime.Now.AddSeconds(-Math.Max(1, _options.Value.RetryIntervalMinutes));

        var message = await context.Wiadomosci
            .FromSqlInterpolated($"""
                SELECT TOP (1) *
                FROM dbo.Wiadomosc WITH (UPDLOCK, READPAST, ROWLOCK)
                WHERE ISNULL(Usuniety, 0) = 0
                    AND (wiaStatus = {StatusNowa} OR (wiaStatus = {StatusRozpoczeta} AND wiaData <= {retryBefore}))
                ORDER BY CASE WHEN wiaStatus = {StatusNowa} THEN 0 ELSE 1 END, wiaID
                """)
            .SingleOrDefaultAsync(cancellationToken);

        if (message == null)
        {
            await transaction.CommitAsync(cancellationToken);
            return null;
        }

        message.wiaStatus = StatusRozpoczeta;
        message.wiaData = DateTime.Now;
        message.DataModyfikacja = DateTime.Now;
        message.ModyfikacjaID = 0;
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return message;
    }

    private async Task ProcessMessageAsync(int messageId, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.CreateDbContext();
        var message = await context.Wiadomosci.SingleOrDefaultAsync(item => item.wiaID == messageId, cancellationToken);

        if (message == null || message.wiaStatus == StatusAnulowana)
        {
            return;
        }

        // Wiadomość z formularza (Nowy numer / wysyłka roboczego) nie ma wzorca: idzie jako treść z formularza.
        WiadomoscWzorzec? template = null;

        if (message.wiaWzorzecID != null)
        {
            template = await context.WiadomoscWzorce.SingleOrDefaultAsync(
                item => item.msgID == message.wiaWzorzecID && item.msgStatus == StatusNowa,
                cancellationToken);

            if (template == null)
            {
                _logger.LogError("Wiadomość {MessageId} nie ma aktywnego wzorca {TemplateId}.", messageId, message.wiaWzorzecID);
                return;
            }
        }

        WiadomoscTresc content;
        IReadOnlyCollection<GraphMailAttachment> attachments;

        try
        {
            content = template == null
                ? WiadomoscTrescFactory.Prepare(message)
                : WiadomoscTrescFactory.Prepare(message, template);
            attachments = await LoadAttachmentsAsync(context, messageId, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Nie można przygotować wiadomości {MessageId}.", messageId);
            return;
        }

        var userRecipients = await (
            from recipient in context.WiadomoscUzytkownicy
            join user in context.Uzytkownicy on recipient.wuzUzytkownikID equals user.usrID
            where recipient.wuzWiadomoscID == messageId
                && recipient.wuzStatus == StatusNowa
                && user.usrStatus == StatusNowa
                && user.Usuniety != true
                && !string.IsNullOrWhiteSpace(user.usrEmail)
            select new QueueRecipient(user.usrPracownikID, user.usrEmail!))
            .ToListAsync(cancellationToken);

        // Adresaci wpisani ręcznie (bez konta w Uzytkownik) są w WiadomoscPracownik; ich status wysyłki zapisujemy w tym samym wierszu.
        var manualRecipients = await context.WiadomoscPracownicy
            .Where(item => item.wprWiadomoscID == messageId && item.wprStatus == StatusNowa && !string.IsNullOrWhiteSpace(item.wprEmail))
            .ToListAsync(cancellationToken);

        var recipients = userRecipients
            .Concat(manualRecipients.Select(item => new QueueRecipient(item.wprPracownikID, item.wprEmail!)))
            .DistinctBy(item => item.Email, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (recipients.Count == 0)
        {
            _logger.LogWarning("Wiadomość {MessageId} nie ma aktywnych odbiorców z adresem e-mail.", messageId);
            await CompleteAsync(context, message, cancellationToken);
            return;
        }

        var sentRecipientEmails = await context.WiadomoscWysylki
            .Where(item => item.wwyWiadomoscID == messageId && item.wwyStatus == StatusNowa && item.wwyEmail != null)
            .Select(item => item.wwyEmail!)
            .ToListAsync(cancellationToken);
        var attachmentNames = string.Join("; ", attachments.Select(item => item.Nazwa));
        var hasError = false;

        var wyslanoWCyklu = 0;
        var limit = _options.Value.MailSendCount;

        foreach (var recipient in recipients.Where(item => !sentRecipientEmails.Contains(item.Email, StringComparer.OrdinalIgnoreCase)))
        {
            var status = await context.Wiadomosci
                .Where(item => item.wiaID == messageId)
                .Select(item => item.wiaStatus)
                .SingleAsync(cancellationToken);

            if (status == StatusAnulowana)
            {
                _logger.LogInformation("Wiadomość {MessageId} została anulowana podczas wysyłki.", messageId);
                return;
            }

            // Limit cyklu: reszta adresatów zostaje w kolejce; wiadomość wróci po MailDelay minutach.
            if (limit > 0 && wyslanoWCyklu >= limit)
            {
                _logger.LogInformation(
                    "Wiadomość {MessageId}: wysłano {Ile} adresatów w cyklu, kolejna partia po {Sekundy} s.",
                    messageId, wyslanoWCyklu, _options.Value.RetryIntervalMinutes);
                hasError = true;
                break;
            }

            try
            {
                await _sender.SendAsync(
                    new GraphMailMessage(message.wiaEmail ?? string.Empty, [recipient.Email], content.Temat, content.Tresc, attachments),
                    cancellationToken);

                context.WiadomoscWysylki.Add(CreateDispatch(messageId, recipient, attachmentNames, StatusNowa));
                wyslanoWCyklu++;

                // Przerwa między wysyłkami do kolejnych adresatów (MailDelay w sekundach).
                if (_options.Value.MailDelay > 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(_options.Value.MailDelay), cancellationToken);
                }
                var wpr = FindManualRecipient(manualRecipients, recipient.Email);
                if (wpr != null)
                {
                    wpr.wprStatus = StatusZakonczona;
                }
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception) when (BledyWysylki.Przejsciowy(exception))
            {
                // Błąd przejściowy (np. limit Graph): adresat zostaje w kolejce, a partia wraca przy następnej próbie.
                _logger.LogWarning(exception, "Przejściowy błąd wysyłki wiadomości {MessageId} do {Email}; wysyłka wstrzymana do kolejnej próby.", messageId, recipient.Email);
                hasError = true;
                break;
            }
            catch (Exception exception)
            {
                hasError = true;
                context.WiadomoscWysylki.Add(CreateDispatch(messageId, recipient, attachmentNames, StatusAnulowana));
                var wpr = FindManualRecipient(manualRecipients, recipient.Email);
                if (wpr != null)
                {
                    wpr.wprStatus = StatusBlad;
                    wpr.wprBlad = exception.Message.Length > 4000 ? exception.Message[..4000] : exception.Message;
                }
                await context.SaveChangesAsync(cancellationToken);
                _logger.LogError(exception, "Nie udało się wysłać wiadomości {MessageId} do {Email}.", messageId, recipient.Email);
            }
        }

        if (!hasError)
        {
            await CompleteAsync(context, message, cancellationToken);
        }
    }

    private async Task<IReadOnlyCollection<GraphMailAttachment>> LoadAttachmentsAsync(
        WiadomoscWorkerContext context,
        int messageId,
        CancellationToken cancellationToken)
    {
        var documents = await context.WiadomoscDokumenty
            .Where(item => item.wdoWiadomoscID == messageId && item.wdoStatus == StatusNowa)
            .OrderBy(item => item.wdoID)
            .ToListAsync(cancellationToken);
        var rootPath = _configuration["FileStorage:Path"];

        if (documents.Count > 0 && string.IsNullOrWhiteSpace(rootPath))
        {
            throw new InvalidOperationException("Brak FileStorage:Path dla załączników wiadomości.");
        }

        var root = rootPath == null ? string.Empty : Path.GetFullPath(rootPath);
        var attachments = new List<GraphMailAttachment>();

        foreach (var document in documents)
        {
            // Nowe załączniki wskazują rekord dbo.Plik (wzorzec epicaHR); stare mają ścieżkę w samym dokumencie.
            var (katalog, plik) = await ResolveStorageAsync(context, document, cancellationToken);

            if (string.IsNullOrWhiteSpace(katalog) || string.IsNullOrWhiteSpace(plik))
            {
                throw new InvalidOperationException($"Załącznik {document.wdoID} nie ma wskazanej ścieżki.");
            }

            var relativePath = Path.Combine(katalog, plik)
                .Replace('/', Path.DirectorySeparatorChar);
            var path = Path.GetFullPath(Path.Combine(root, relativePath));

            if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !File.Exists(path))
            {
                throw new FileNotFoundException(
                    $"Nie znaleziono załącznika wiadomości: {document.wdoNazwa}.{Environment.NewLine}{SciezkaDiagnostyka.Opisz(path)}",
                    path);
            }

            attachments.Add(GraphMailAttachment.Create(document.wdoNazwa ?? document.wdoPlik, await File.ReadAllBytesAsync(path, cancellationToken)));
        }

        return attachments;
    }

    private static async Task<(string? Katalog, string? Plik)> ResolveStorageAsync(
        WiadomoscWorkerContext context,
        WiadomoscDokument document,
        CancellationToken cancellationToken)
    {
        if (document.wdoPlikID is not int plikID)
        {
            return (document.wdoKatalog, document.wdoPlik);
        }

        var rekord = await context.Pliki
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.plkID == plikID, cancellationToken);

        if (rekord == null || string.IsNullOrWhiteSpace(rekord.plkFolder) || string.IsNullOrWhiteSpace(rekord.plkPlik))
        {
            throw new InvalidOperationException($"Załącznik {document.wdoID} wskazuje rekord Plik {plikID} bez katalogu lub nazwy pliku.");
        }

        return (rekord.plkFolder, rekord.plkPlik);
    }

    private static WiadomoscWysylka CreateDispatch(int messageId, QueueRecipient recipient, string attachments, string status) =>
        new()
        {
            wwyWiadomoscID = messageId,
            wwyPracownikID = recipient.EmployeeId,
            wwyData = DateTime.Now,
            wwyEmail = recipient.Email,
            wwyZalacznik = attachments,
            wwyStatus = status
        };

    private static async Task CompleteAsync(WiadomoscWorkerContext context, Wiadomosc message, CancellationToken cancellationToken)
    {
        message.wiaStatus = StatusZakonczona;
        message.DataModyfikacja = DateTime.Now;
        message.ModyfikacjaID = 0;
        await context.SaveChangesAsync(cancellationToken);
    }

    private static WiadomoscPracownik? FindManualRecipient(IEnumerable<WiadomoscPracownik> manualRecipients, string email) =>
        manualRecipients.FirstOrDefault(item => string.Equals(item.wprEmail, email, StringComparison.OrdinalIgnoreCase));

    private sealed record QueueRecipient(int? EmployeeId, string Email);
}
