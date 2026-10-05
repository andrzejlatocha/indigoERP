var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var indigoBlazor = builder.AddProject<Projects.indigoERP_Blazor>("indigoerp-blazor");

builder.AddProject<Projects.indigoERP_Api>("indigoerp-api").WithReference(indigoBlazor).WithReference(cache);

builder.Build().Run();
