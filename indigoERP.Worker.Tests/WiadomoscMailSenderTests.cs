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

    [Fact]
    public async Task Routing_SendsInternalSenderDomainThroughSmtp()
    {
        var graph = new FakeSender();
        var smtp = new FakeSender();
        var sender = new RoutingWiadomoscMailSender(graph, smtp);

        await sender.SendAsync(new GraphMailMessage("informacje@hutaczestochowa.com.pl", ["a@huta-czestochowa.pl"], "T", "B", []), CancellationToken.None);

        Assert.Single(smtp.Wyslane);
        Assert.Empty(graph.Wyslane);
    }

    [Fact]
    public async Task Routing_SendsOtherSendersThroughGraph()
    {
        var graph = new FakeSender();
        var smtp = new FakeSender();
        var sender = new RoutingWiadomoscMailSender(graph, smtp);

        await sender.SendAsync(new GraphMailMessage("itsupport@huta-czestochowa.pl", ["a@huta-czestochowa.pl"], "T", "B", []), CancellationToken.None);

        Assert.Single(graph.Wyslane);
        Assert.Empty(smtp.Wyslane);
    }

    [Theory]
    [InlineData("informacje@hutaczestochowa.com.pl", true)]
    [InlineData("itsupport@huta-czestochowa.pl", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void UzywaSmtp_ChoosesSmtpOnlyForInternalSmtpDomain(string? nadawca, bool oczekiwany)
    {
        Assert.Equal(oczekiwany, RoutingWiadomoscMailSender.UzywaSmtp(nadawca));
    }

    private sealed class FakeSender : IWiadomoscMailSender
    {
        public List<GraphMailMessage> Wyslane { get; } = [];

        public bool IsConfigured => true;

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
