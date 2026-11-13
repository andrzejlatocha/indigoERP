using Biwen.EFCore.UseRowNumberForPaging;
using indigoERP.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace indigoERP.Api.Tests;

public sealed class LegacySqlPagingTests
{
    [Fact]
    public void Paging_generates_row_number_without_connecting_to_a_database()
    {
        var options = new DbContextOptionsBuilder<indigoERPContext>()
            .UseSqlServer("Server=localhost;Database=unused;Integrated Security=true;TrustServerCertificate=true",
                sql => sql.UseRowNumberForPaging()).Options;
        using var context = new indigoERPContext(options);

        string query = context.Plec.OrderBy(x => x.pleKOD).Skip(10).Take(5).ToQueryString();

        Assert.Contains("ROW_NUMBER()", query, StringComparison.OrdinalIgnoreCase);
    }
}