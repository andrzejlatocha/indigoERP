var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var indigoBlazor = builder.AddProject<Projects.indigoERP_Blazor>("indigoerp-blazor");
var indigoGateway = builder.AddProject<Projects.indigoERP_Gateway>("indigoerp-gateway");

builder.AddProject<Projects.indigoERP_Api>("indigoerp-api").WithReference(indigoBlazor).WithReference(indigoGateway).WithReference(cache);

builder.Build().Run();
