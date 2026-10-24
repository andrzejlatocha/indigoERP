namespace indigoERP.Worker;

public sealed class WorkerOptions
{
    public const string SectionName = "Worker";

    public int PollingIntervalSeconds { get; init; } = 60;
    public int RetryIntervalMinutes { get; init; } = 15;
    public int BatchSize { get; init; } = 10;
}
