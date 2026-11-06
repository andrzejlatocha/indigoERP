using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace indigoERP.Api.Middleware;

public static class ApiErrorHandlingExtensions
{
    /// <summary>
    /// Nieobsłużony wyjątek → log błędu z TraceId i odpowiedź 500 (ProblemDetails); odpowiedzi 4xx/5xx bez treści → ostrzeżenie w logu
    /// i ProblemDetails. Szczegóły wyjątku zwracamy tylko w Development.
    /// </summary>
    public static IApplicationBuilder UseApiErrorHandling(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalExceptionHandler");
                var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();

                if (exception is not null)
                    logger.LogError(exception, "Nieobsłużony błąd API: {Method} {Path}. TraceId: {TraceId}", context.Request.Method,
                        context.Request.Path, GetTraceId(context));

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await WriteProblemDetailsAsync(context, StatusCodes.Status500InternalServerError, "Wystąpił błąd podczas przetwarzania żądania.",
                    environment.IsDevelopment() ? exception?.ToString() : "Szczegóły błędu zostały zapisane w logach.");
            });
        });

        // Każda odpowiedź 4xx/5xx (także z treścią z kontrolera, np. BadRequest("komunikat")) trafia do logu jako ostrzeżenie.
        app.Use(async (context, next) =>
        {
            await next();
            if (context.Response.StatusCode < StatusCodes.Status400BadRequest) return;

            context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("ApiResponse").LogWarning(
                "Odpowiedź z błędem. Status: {StatusCode}, Metoda: {Method}, Ścieżka: {Path}, Użytkownik: {User}, TraceId: {TraceId}",
                context.Response.StatusCode, context.Request.Method, context.Request.Path, context.User.FindFirst("usrNazwa")?.Value ?? context.User.Identity?.Name ?? "-",
                GetTraceId(context));
        });

        app.UseStatusCodePages(async statusCodeContext =>
        {
            var context = statusCodeContext.HttpContext;
            if (context.Response.HasStarted || context.Response.StatusCode < StatusCodes.Status400BadRequest) return;

            await WriteProblemDetailsAsync(context, context.Response.StatusCode, ReasonPhrases.GetReasonPhrase(context.Response.StatusCode),
                GetDefaultStatusMessage(context.Response.StatusCode));
        });

        return app;
    }

    private static Task WriteProblemDetailsAsync(HttpContext context, int statusCode, string title, string? detail)
    {
        var problemDetails = new ProblemDetails { Status = statusCode, Title = title, Detail = detail, Instance = context.Request.Path };
        problemDetails.Extensions["traceId"] = GetTraceId(context);

        context.Response.ContentType = "application/problem+json";
        return context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static string GetTraceId(HttpContext context) => Activity.Current?.Id ?? context.TraceIdentifier;

    private static string GetDefaultStatusMessage(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Nieprawidłowe żądanie.",
        StatusCodes.Status401Unauthorized => "Wymagane jest uwierzytelnienie.",
        StatusCodes.Status403Forbidden => "Brak uprawnień do wykonania operacji.",
        StatusCodes.Status404NotFound => "Nie znaleziono zasobu.",
        StatusCodes.Status500InternalServerError => "Wystąpił wewnętrzny błąd serwera.",
        _ => "Żądanie nie zostało zakończone poprawnie."
    };
}
