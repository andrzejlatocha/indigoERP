using EntityFrameworkCore.UseRowNumberForPaging;
using indigoERP.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var configuration = builder.Configuration;
var services = builder.Services;
var connectionString = builder.Configuration.GetConnectionString("indigoERPConnection");

// Add services to the container.

services.AddControllers();
int serverVersion = configuration.GetSection("appSettings").GetValue<int>(key: "serverVer");

if (serverVersion > 2008)
{
    services.AddDbContext<indigoERPContext>(options => options.UseSqlServer(connectionString));
}
else
{
    services.AddDbContext<indigoERPContext>(options => options.UseSqlServer(connectionString, o => o.UseRowNumberForPaging()));
}

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer("Bearer", options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidIssuer = configuration.GetSection("Jwt").GetValue<string>("Issuer"),
            ValidateAudience = false,
            ValidAudience = configuration.GetSection("Jwt").GetValue<string>("Audience"),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Jwt").GetValue<string>(key: "Key")))
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.ResolveConflictingActions(descriptions => descriptions.First());
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "indigoERP.API",
        Description = "API dla aplikacji indigoERP"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "Jwt",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });

    //options.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        },
    //        new List<string>()
    //    }
    //});
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
