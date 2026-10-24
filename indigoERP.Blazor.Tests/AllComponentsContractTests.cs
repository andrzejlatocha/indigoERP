using Bunit;
using Microsoft.AspNetCore.Components;

namespace indigoERP.Blazor.Tests;

public sealed class AllComponentsContractTests
{
    private static readonly Type[] Components = typeof(Pages.Home).Assembly.GetTypes()
        .Where(type => type is { IsAbstract: false, IsClass: true })
        .Where(type => type.Namespace?.StartsWith("indigoERP.Blazor", StringComparison.Ordinal) == true)
        .Where(type => typeof(IComponent).IsAssignableFrom(type))
        .OrderBy(type => type.FullName)
        .ToArray();

    [Fact]
    public void Application_contains_blazor_components()
    {
        Assert.NotEmpty(Components);
    }

    [Fact]
    public void Page_routes_are_unique()
    {
        var duplicateRoutes = Components
            .SelectMany(component => component.GetCustomAttributes(typeof(RouteAttribute), inherit: true).Cast<RouteAttribute>())
            .Select(attribute => attribute.Template)
            .Where(template => !string.IsNullOrWhiteSpace(template))
            .GroupBy(template => template, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToArray();

        Assert.Empty(duplicateRoutes);
    }
}
