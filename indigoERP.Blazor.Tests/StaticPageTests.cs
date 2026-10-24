using System.Security.Claims;
using System.Reflection;
using Bunit;
using indigoERP.Domain.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace indigoERP.Blazor.Tests;

public sealed class StaticPageTests : BunitContext
{
    public StaticPageTests()
    {
        Services.AddLogging();
        Services.AddScoped<DialogService>();
        Services.AddSingleton<AuthenticationStateProvider>(new TestAuthenticationStateProvider(
        [
            new Claim("usrImie", "Jan"),
            new Claim("usrNazwisko", "Kowalski"),
            new Claim("usrEmail", "jan.kowalski@example.test")
        ]));
        Services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment());
    }

    [Fact]
    public void Administrator_index_renders_all_administration_areas()
    {
        var cut = Render<Pages.Administrator.Index>();

        Assert.Contains("Administracja", cut.Markup);
        Assert.Contains("/administrator/aplikacja", cut.Markup);
        Assert.Contains("/administrator/rola", cut.Markup);
        Assert.Contains("/administrator/grupa", cut.Markup);
        Assert.Contains("/administrator/uzytkownik", cut.Markup);
    }

    [Fact]
    public void Footer_renders_contact_information()
    {
        Services.AddSingleton<indigoERP.Service.Interfaces._Administrator.ISerwerService>(new SerwerServiceStub());
        var cut = Render<Shared.Footer>();

        Assert.Contains("Huta Częstochowa", cut.Markup);
        Assert.Contains("+48 34 323 8000", cut.Markup);
        Assert.Contains("itsupport@huta-czestochowa.pl", cut.Markup);
    }

    [Fact]
    public void Footer_shows_api_version_server_and_database_when_available()
    {
        Services.AddSingleton<indigoERP.Service.Interfaces._Administrator.ISerwerService>(new SerwerServiceStub());
        var cut = Render<Shared.Footer>();

        cut.WaitForAssertion(() => Assert.Contains("Wersja API: 3.26.1015.11388", cut.Markup));
        Assert.Contains("Serwer: JAPET", cut.Markup);
        Assert.Contains("Baza: indigoERP", cut.Markup);
        Assert.Contains("footer-serwer-panel", cut.Markup);
    }

    [Fact]
    public void Footer_hides_the_server_panel_when_the_service_returns_nothing()
    {
        Services.AddSingleton<indigoERP.Service.Interfaces._Administrator.ISerwerService>(new SerwerServiceStub(null));
        var cut = Render<Shared.Footer>();

        Assert.Contains("+48 34 323 8000", cut.Markup);
        Assert.DoesNotContain("footer-serwer-panel", cut.Markup);
    }

    private sealed class SerwerServiceStub(indigoERP.Domain.ViewModels._Administracja.SerwerInfoViewModel? info)
        : indigoERP.Service.Interfaces._Administrator.ISerwerService
    {
        public SerwerServiceStub() : this(new indigoERP.Domain.ViewModels._Administracja.SerwerInfoViewModel
        {
            WersjaApi = "3.26.1015.11388",
            SerwerSql = "JAPET",
            BazaDanych = "indigoERP"
        })
        {
        }

        public Task<indigoERP.Domain.ViewModels._Administracja.SerwerInfoViewModel?> GetInfo() => Task.FromResult(info);

        public Task<IEnumerable<indigoERP.Domain.ViewModels._Administracja.SerwerViewModel>> Select(string srvStatus) => throw new NotSupportedException();
        public Task<indigoERP.Domain.Dtos._Administrator.SerwerDto> Get(int srvID) => throw new NotSupportedException();
        public Task<indigoERP.Domain.Models._Wspolne.ResponseMessage> Insert(indigoERP.Domain.Dtos._Administrator.SerwerDto serwer)
            => throw new NotSupportedException();
        public Task<indigoERP.Domain.Models._Wspolne.ResponseMessage> Update(int srvID, indigoERP.Domain.Dtos._Administrator.SerwerDto serwer)
            => throw new NotSupportedException();
        public Task<indigoERP.Domain.Models._Wspolne.ResponseMessage> Delete(int srvID) => throw new NotSupportedException();
        public Task<IEnumerable<indigoERP.Domain.ViewModels._Administracja.MonitorSerwerStatusViewModel>> SelectStatus() => throw new NotSupportedException();
        public Task<indigoERP.Domain.ViewModels._Administracja.MonitorSerwerStatusViewModel?> Check(int srvID) => throw new NotSupportedException();
        public Task<IEnumerable<indigoERP.Domain.ViewModels._Administracja.SqlAgentJobViewModel>> SelectJoby(int srvID) => throw new NotSupportedException();
    }

    [Fact]
    public void Support_form_prefills_the_authenticated_users_email_address()
    {
        var cut = Render<Pages.Support>();

        cut.WaitForAssertion(() => Assert.Contains("Zgłoszenie do IT Support", cut.Markup));
        var field = typeof(Pages.Support).GetField("supportMessage", BindingFlags.Instance | BindingFlags.NonPublic);
        var supportMessage = Assert.IsType<SupportMessage>(field!.GetValue(cut.Instance));

        Assert.Equal("jan.kowalski@example.test", supportMessage.msgAdresEmailOd);
        Assert.Contains("Wiadomość", cut.Markup);
    }
}
