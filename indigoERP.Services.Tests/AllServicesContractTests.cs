using indigoERP.Service.Services;

namespace indigoERP.Services.Tests;

public sealed class AllServicesContractTests
{
    private static readonly Type[] Services = typeof(HomeService).Assembly.GetTypes()
        .Where(type => type is { IsAbstract: false, IsClass: true })
        .Where(type => type.Namespace?.StartsWith("indigoERP.Service.Services", StringComparison.Ordinal) == true)
        .Where(type => type.Name.EndsWith("Service", StringComparison.Ordinal))
        .OrderBy(type => type.FullName)
        .ToArray();

    [Fact]
    public void Every_application_service_implements_its_matching_interface()
    {
        Assert.NotEmpty(Services);

        foreach (var service in Services)
        {
            var expectedInterface = $"I{service.Name}";
            Assert.Contains(service.GetInterfaces(), item => item.Name == expectedInterface);
        }
    }

    [Fact]
    public void Every_application_service_uses_the_shared_http_client_factory()
    {
        foreach (var service in Services)
        {
            Assert.Contains(
                service.GetConstructors().SelectMany(constructor => constructor.GetParameters()),
                parameter => parameter.ParameterType == typeof(IHttpClientFactory));
        }
    }
}
