using System.Reflection;
using indigoERP.Api.Controllers._Wspolne;
using System.ComponentModel.DataAnnotations.Schema;
using indigoERP.Domain.Enumerators._Wspolne;
using Microsoft.AspNetCore.Mvc;

namespace indigoERP.Api.Tests;

public sealed class WiadomoscZakresControllerTests
{
    [Fact]
    public void Select_accepts_app_id_and_returns_scope_enumerator_list()
    {
        var method = typeof(WiadomoscZakresController).GetMethod(nameof(WiadomoscZakresController.Select));
        var parameter = Assert.Single(method!.GetParameters());

        Assert.Equal("appID", parameter.Name);
        Assert.Equal(typeof(Task<ActionResult<IEnumerable<WiadomoscZakresEnumerator>>>), method.ReturnType);
        Assert.NotNull(typeof(WiadomoscZakresEnumerator).GetProperty(nameof(WiadomoscZakresEnumerator.wzaNazwa)));
        var table = typeof(WiadomoscZakresEnumerator).GetCustomAttribute<TableAttribute>();

        Assert.Equal("WiadomoscZakres", table?.Name);
        Assert.Equal("enum", table?.Schema);
    }
}
