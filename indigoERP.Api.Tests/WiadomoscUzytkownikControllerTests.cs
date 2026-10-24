using indigoERP.Api.Controllers._Wspolne;
using indigoERP.Domain.ViewModels._Wspolne;
using Microsoft.AspNetCore.Mvc;

namespace indigoERP.Api.Tests;

public sealed class WiadomoscUzytkownikControllerTests
{
    [Fact]
    public void Select_returns_message_user_view_model()
    {
        var method = typeof(WiadomoscUzytkownikController).GetMethod(nameof(WiadomoscUzytkownikController.Select));
        var parameter = Assert.Single(method!.GetParameters());

        Assert.Equal("wiaStatus", parameter.Name);
        Assert.Equal(
            typeof(Task<ActionResult<IEnumerable<WiadomoscUzytkownikViewModel>>>),
            method.ReturnType);
    }

    [Fact]
    public void Unread_message_endpoints_accept_application_and_user_identifiers()
    {
        var controllerType = typeof(WiadomoscUzytkownikController);
        var countMethod = controllerType.GetMethod(nameof(WiadomoscUzytkownikController.SelectCountByUzytkownik));
        var listMethod = controllerType.GetMethod(nameof(WiadomoscUzytkownikController.SelectBrakOdczytByUzytkownik));

        Assert.Equal(["appID", "usrID"], countMethod!.GetParameters().Select(parameter => parameter.Name));
        Assert.Equal(["appID", "usrID", "wiaStatus"], listMethod!.GetParameters().Select(parameter => parameter.Name));
        Assert.Equal(typeof(Task<ActionResult<int>>), countMethod.ReturnType);
        Assert.Equal(typeof(Task<ActionResult<IEnumerable<WiadomoscUzytkownikViewModel>>>), listMethod.ReturnType);
    }
}
