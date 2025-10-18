namespace Security.Sanitization.Extensions;

public static class RequestSanitizerPipelineExtensions
{
    public static IApplicationBuilder UseRequestHeadersSanitizer(this IApplicationBuilder app) =>
        app.UseMiddleware<RequestHeadersSanitizerMiddleware>();

    public static IApplicationBuilder UseRequestBodySanitizer(this IApplicationBuilder app) =>
        app.UseMiddleware<RequestBodySanitizerMiddleware>();

    public static IApplicationBuilder UseRequestHeadersAndBodySanitizers(this IApplicationBuilder app) =>
        app.UseRequestHeadersSanitizer().UseRequestBodySanitizer();

    public static RouteHandlerBuilder MapXssHeaderTestRoute(this WebApplication webApp, string injectedHeadersUrl)
    {
        return 
            webApp.MapGet(injectedHeadersUrl, (HttpRequest request) =>
                request.Headers.ToDictionary(
                    keyValuePair => keyValuePair.Key,
                    keyValuePair => keyValuePair.Value.ToString()));
    }

    public static RouteHandlerBuilder MapXssTopLevelBodyTestRoute(this WebApplication webApp, string injectedBodyUrl)
    {
        return webApp.MapPost(injectedBodyUrl, (HttpRequest request, TopLevelOnly body) => body);
    }

    public static RouteHandlerBuilder MapXssNestingBodyTestRoute(this WebApplication webApp, string injectedBodyUrl)
    {
        return webApp.MapPost(injectedBodyUrl, (HttpRequest request, IncludesNesting body) => body);
    }

    public static void AddXssAttackReportingTestMiddleware(this WebApplication webApp)
    {
        webApp.Run(async (HttpContext context) =>
        {
            var report = context.Features.Get<IXssThreatReport>();
            var presentation = new MalicePresentation();

            if (report?.HasFindings is true)
            {
                foreach (var (location, original) in report.Findings)
                {
                    var entry = $"Potential XSS at {location}, original: {original}";
                    presentation.Findings.Add(entry);
                }
            }

            await context.Response.WriteAsJsonAsync(presentation);
        });
    }
}