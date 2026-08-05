namespace indigoERP.Api.Services
{
    public interface IServerCheckService
    {
        Task<(bool IsAlive, int? ResponseMs, string? Error)> CheckAsync(string typKOD, string ip, int? port, CancellationToken cancellationToken);
    }
}
