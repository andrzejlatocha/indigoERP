using Microsoft.EntityFrameworkCore;

namespace indigoERP.Worker.Data;

public sealed class WiadomoscWorkerContextFactory
{
    private readonly IConfiguration _configuration;

    public WiadomoscWorkerContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public WiadomoscWorkerContext CreateDbContext()
    {
        var connectionString = _configuration.GetConnectionString("indigoERPConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Brak ConnectionStrings__indigoERPConnection dla indigoERP.Worker.");
        }

        var options = new DbContextOptionsBuilder<WiadomoscWorkerContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new WiadomoscWorkerContext(options);
    }
}
