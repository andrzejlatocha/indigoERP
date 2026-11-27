using System.Reflection;
using indigoERP.Api.Controllers._Wspolne;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace indigoERP.Api.Tests;

// Trasa usuwania kontaktu musi odpowiadać nazwie parametru (kntID); inaczej DELETE nie wiąże identyfikatora.
public sealed class KontaktControllerRouteTests
{
    [Fact]
    public void DeleteMaTrasePasujacaDoParametruKntID()
    {
        var metoda = typeof(KontaktController).GetMethod(nameof(KontaktController.Delete));
        Assert.NotNull(metoda);
        var atrybut = metoda!.GetCustomAttribute<HttpDeleteAttribute>();
        Assert.NotNull(atrybut);
        Assert.Equal("{kntID}", atrybut!.Template);
    }
}
