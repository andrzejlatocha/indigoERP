using indigoERP.Worker.Services;
using Microsoft.Extensions.Configuration;

namespace indigoERP.Worker.Tests;

public class WiadomoscMailSenderTests
{
    [Fact]
    public void Smtp_IsNotConfigured_WithoutHostOrPassword()
    {
        var sender = new SmtpWiadomoscMailSender(new ConfigurationBuilder().Build());

        Assert.False(sender.IsConfigured);
    }

    [Fact]
    public void Graph_Sender_ReportsConfigurationOfGraphService()
    {
        var sender = new GraphWiadomoscMailSender(new FakeGraph(configured: true));

        Assert.True(sender.IsConfigured);
    }

    [Fact]
    public async Task Graph_Sender_DelegatesSendToGraphService()
    {
        var graph = new FakeGraph(configured: true);
        var sender = new GraphWiadomoscMailSender(graph);

        await sender.SendAsync(new GraphMailMessage("nadawca@huta-czestochowa.pl", ["a@huta-czestochowa.pl"], "Temat", "Treść", []), CancellationToken.None);

        var wyslana = Assert.Single(graph.Wyslane);
        Assert.Equal("a@huta-czestochowa.pl", Assert.Single(wyslana.Odbiorcy));
    }

    private static GraphMailMessage Wiadomosc(string? nadawca) => new(nadawca ?? string.Empty, ["a@huta-czestochowa.pl"], "T", "B", []);

    private static (RoutingWiadomoscMailSender Routing, FakeSender Smtp, FakeSender Graph) Routing(
        MailRoutingOptions? opcje = null, bool graphConfigured = true, bool smtpConfigured = true)
    {
        var smtp = new FakeSender(smtpConfigured);
        var graph = new FakeSender(graphConfigured);

        return (new RoutingWiadomoscMailSender(smtp, graph, opcje ?? new MailRoutingOptions { Wlaczony = true }), smtp, graph);
    }

    [Theory]
    [InlineData("informacje@hutaczestochowa.com.pl")]
    [InlineData("epicaHR@huta-czestochowa.pl")]
    [InlineData("k.jocz@interbalt.pl")]
    [InlineData("")]
    public async Task Routing_Disabled_KeepsEverySenderOnSmtp(string nadawca)
    {
        var (routing, smtp, graph) = Routing(new MailRoutingOptions());

        await routing.SendAsync(Wiadomosc(nadawca), CancellationToken.None);

        Assert.Single(smtp.Wyslane);
        Assert.Empty(graph.Wyslane);
        Assert.False(routing.UzywaGraph(nadawca));
    }

    [Fact]
    public async Task Routing_Enabled_SendsM365DomainThroughGraph()
    {
        var (routing, smtp, graph) = Routing();

        await routing.SendAsync(Wiadomosc("epicaHR@huta-czestochowa.pl"), CancellationToken.None);

        Assert.Single(graph.Wyslane);
        Assert.Empty(smtp.Wyslane);
    }

    [Theory]
    [InlineData("informacje@hutaczestochowa.com.pl")]
    [InlineData("k.jocz@interbalt.pl")]
    [InlineData("odpady@tkmrp.pl")]
    [InlineData("a@sub.huta-czestochowa.pl")]
    [InlineData("a@xhuta-czestochowa.pl")]
    [InlineData("")]
    [InlineData(null)]
    public async Task Routing_Enabled_KeepsOtherSendersOnSmtp(string? nadawca)
    {
        var (routing, smtp, graph) = Routing();

        await routing.SendAsync(Wiadomosc(nadawca), CancellationToken.None);

        Assert.Single(smtp.Wyslane);
        Assert.Empty(graph.Wyslane);
        Assert.False(routing.UzywaGraph(nadawca));
    }

    [Fact]
    public async Task Routing_Enabled_FallsBackToSmtpWhenGraphIsNotConfigured()
    {
        var (routing, smtp, graph) = Routing(graphConfigured: false);

        await routing.SendAsync(Wiadomosc("epicaHR@huta-czestochowa.pl"), CancellationToken.None);

        Assert.Single(smtp.Wyslane);
        Assert.Empty(graph.Wyslane);
    }

    [Fact]
    public async Task Routing_SenderList_LimitsGraphToListedSenders()
    {
        var (routing, smtp, graph) = Routing(new MailRoutingOptions { Wlaczony = true, Nadawcy = ["epicaHR@huta-czestochowa.pl"] });

        await routing.SendAsync(Wiadomosc("EPICAHR@huta-czestochowa.pl"), CancellationToken.None);
        await routing.SendAsync(Wiadomosc("itsupport@huta-czestochowa.pl"), CancellationToken.None);

        Assert.Single(graph.Wyslane);
        Assert.Single(smtp.Wyslane);
        Assert.Equal("itsupport@huta-czestochowa.pl", smtp.Wyslane[0].Nadawca);
    }

    [Theory]
    [InlineData(true, false, false, true)]
    [InlineData(false, true, false, false)]
    [InlineData(false, true, true, true)]
    [InlineData(false, false, true, false)]
    public void Routing_IsConfigured_WhenSmtpOrEnabledGraphIsConfigured(bool smtpConfigured, bool graphConfigured, bool graphEnabled, bool oczekiwany)
    {
        var (routing, _, _) = Routing(new MailRoutingOptions { Wlaczony = graphEnabled }, graphConfigured, smtpConfigured);

        Assert.Equal(oczekiwany, routing.IsConfigured);
    }
    private sealed class FakeSender(bool configured = true) : IWiadomoscMailSender
    {
        public List<GraphMailMessage> Wyslane { get; } = [];

        public bool IsConfigured => configured;

        public Task SendAsync(GraphMailMessage message, CancellationToken cancellationToken)
        {
            Wyslane.Add(message);

            return Task.CompletedTask;
        }
    }

    private sealed class FakeGraph(bool configured) : IMicrosoftGraphMailService
    {
        public List<GraphMailMessage> Wyslane { get; } = [];

        public bool IsConfigured => configured;

        public Task SendAsync(GraphMailMessage message, CancellationToken cancellationToken)
        {
            Wyslane.Add(message);

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<UndeliveryNotice>> GetUndeliveryNoticesAsync(string sender, DateTimeOffset since, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<UndeliveryNotice>>([]);
    }
}
