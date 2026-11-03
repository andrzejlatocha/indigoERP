namespace indigoERP.Worker;

public sealed class WorkerOptions
{
    public const string SectionName = "Worker";

    public int PollingIntervalSeconds { get; init; } = 60;
    public int BatchSize { get; init; } = 10;

    /// <summary>Liczba adresatów wysyłanych w jednym cyklu; 0 = bez limitu.</summary>
    public int MailSendCount { get; init; } = 0;

    /// <summary>Przerwa w sekundach między kolejnymi wysyłkami do adresatów; 0 = bez przerwy.</summary>
    public int MailDelay { get; init; } = 0;

    /// <summary>Przerwa w sekundach między cyklami wiadomości (także po błędzie przejściowym). Nazwa zostaje z historii.</summary>
    public int RetryIntervalMinutes { get; init; } = 900;
}
