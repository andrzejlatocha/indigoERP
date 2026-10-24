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
    private readonly WiadomoscWorkerContextFactory _contextFactory;
    private readonly IMicrosoftGraphMailService _mailService;
    private readonly IConfiguration _configuration;
    private readonly IOptions<WorkerOptions> _options;
    private readonly ILogger<WiadomoscKolejkaService> _logger;

    public WiadomoscKolejkaService(
        WiadomoscWorkerContextFactory contextFactory,
        IMicrosoftGraphMailService mailService,
        IConfiguration configuration,
        IOptions<WorkerOptions> options,
        ILogger<WiadomoscKolejkaService> logger)
    {
        _contextFactory = contextFactory;
        _mailService = mailService;
        _configuration = configuration;
        _options = options;
        _logger = logger;
    }

    public async Task<int> ProcessAsync(CancellationToken cancellationToken)
    {
        if (!_mailService.IsConfigured)
        {
            _logger.LogError(
                "Wysyłka wstrzymana: nie ustawiono MicrosoftGraph_TenantId, MicrosoftGraph_ClientId lub MicrosoftGraph_ClientSecret.");
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

    private async Task<Wiadomosc?> ClaimNextAsync(CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
        var retryBefore = DateTime.Now.AddMinutes(-Math.Max(1, _options.Value.RetryIntervalMinutes));

        var message = await context.Wiadomosci
            .FromSqlInterpolated($"""
                SELECT TOP (1) *
                FROM dbo.Wiadomosc WITH (UPDLOCK, READPAST, ROWLOCK)
                WHERE ISNULL(Usuniety, 0) = 0
                    AND wiaWzorzecID IS NOT NULL
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

        var template = await context.WiadomoscWzorce.SingleOrDefaultAsync(
            item => item.msgID == message.wiaWzorzecID && item.msgStatus == StatusNowa,
            cancellationToken);

        if (template == null)
        {
            _logger.LogError("Wiadomość {MessageId} nie ma aktywnego wzorca {TemplateId}.", messageId, message.wiaWzorzecID);
            return;
        }

        WiadomoscTresc content;
        IReadOnlyCollection<GraphMailAttachment> attachments;

        try
        {
            content = WiadomoscTrescFactory.Prepare(message, template);
            attachments = await LoadAttachmentsAsync(context, messageId, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Nie można przygotować wiadomości {MessageId}.", messageId);
            return;
        }

        var recipients = await (
            from recipient in context.WiadomoscUzytkownicy
            join user in context.Uzytkownicy on recipient.wuzUzytkownikID equals user.usrID
            where recipient.wuzWiadomoscID == messageId
                && recipient.wuzStatus == StatusNowa
                && user.usrStatus == StatusNowa
                && user.Usuniety != true
                && !string.IsNullOrWhiteSpace(user.usrEmail)
            select new QueueRecipient(user.usrPracownikID, user.usrEmail!))
            .ToListAsync(cancellationToken);

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

            try
            {
                await _mailService.SendAsync(
                    new GraphMailMessage(message.wiaEmail ?? string.Empty, [recipient.Email], content.Temat, content.Tresc, attachments),
                    cancellationToken);

                context.WiadomoscWysylki.Add(CreateDispatch(messageId, recipient, attachmentNames, StatusNowa));
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                hasError = true;
                context.WiadomoscWysylki.Add(CreateDispatch(messageId, recipient, attachmentNames, StatusAnulowana));
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
            if (string.IsNullOrWhiteSpace(document.wdoKatalog) || string.IsNullOrWhiteSpace(document.wdoPlik))
            {
                throw new InvalidOperationException($"Załącznik {document.wdoID} nie ma wskazanej ścieżki.");
            }

            var relativePath = Path.Combine(document.wdoKatalog, document.wdoPlik)
                .Replace('/', Path.DirectorySeparatorChar);
            var path = Path.GetFullPath(Path.Combine(root, relativePath));

            if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !File.Exists(path))
            {
                throw new FileNotFoundException($"Nie znaleziono załącznika wiadomości: {document.wdoNazwa}.", path);
            }

            attachments.Add(GraphMailAttachment.Create(document.wdoNazwa ?? document.wdoPlik, await File.ReadAllBytesAsync(path, cancellationToken)));
        }

        return attachments;
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

    private sealed record QueueRecipient(int? EmployeeId, string Email);
}
