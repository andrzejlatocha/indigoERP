using System.Reflection;
using indigoERP.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace indigoERP.Api.Tests;

public sealed class AllControllersContractTests
{
    private static readonly Type[] Controllers = typeof(HomeController).Assembly.GetTypes()
        .Where(type => !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type))
        .Where(type => type.Name.EndsWith("Controller", StringComparison.Ordinal))
        .OrderBy(type => type.FullName)
        .ToArray();

    [Fact]
    public void All_controllers_expose_the_project_route_convention()
    {
        Assert.NotEmpty(Controllers);

        foreach (var controller in Controllers)
        {
            Assert.NotNull(controller.GetCustomAttribute<ApiControllerAttribute>());
            var route = Assert.IsType<RouteAttribute>(controller.GetCustomAttribute<RouteAttribute>());
            Assert.Equal("api/[controller]/[action]", route.Template);
        }
    }

    [Fact]
    public void Every_public_controller_action_declares_an_http_method()
    {
        foreach (var controller in Controllers)
        {
            var actions = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(method => !method.IsSpecialName)
                .ToArray();

            Assert.NotEmpty(actions);
            Assert.All(actions, action => Assert.NotEmpty(action.GetCustomAttributes<HttpMethodAttribute>(inherit: true)));
        }
    }
}
