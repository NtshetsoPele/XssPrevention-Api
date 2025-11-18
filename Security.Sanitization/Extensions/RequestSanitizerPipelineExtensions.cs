namespace Security.Sanitization.Extensions;

public static class RequestSanitizerPipelineExtensions
{
    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseRequestHeadersSanitizer() =>
            app.UseMiddleware<RequestHeadersSanitizerMiddleware>();

        public IApplicationBuilder UseRequestBodySanitizer() =>
            app.UseMiddleware<RequestBodySanitizerMiddleware>();

        public IApplicationBuilder UseRequestHeadersAndBodySanitizers() =>
            app.UseRequestHeadersSanitizer().UseRequestBodySanitizer();
    }

    extension(WebApplication webApp)
    {
        public RouteHandlerBuilder MapXssHeaderTestRoute(string injectedHeadersUrl)
        {
            return 
                webApp.MapGet(injectedHeadersUrl, (HttpRequest request) =>
                    request.Headers.ToDictionary(
                        keyValuePair => keyValuePair.Key,
                        keyValuePair => keyValuePair.Value.ToString()));
        }

        public RouteHandlerBuilder MapXssTopLevelBodyTestRoute(string injectedBodyUrl)
        {
            return webApp.MapPost(injectedBodyUrl, (HttpRequest request, TopLevelOnly body) => body);
        }

        public RouteHandlerBuilder MapXssNestingBodyTestRoute(string injectedBodyUrl)
        {
            return webApp.MapPost(injectedBodyUrl, (HttpRequest request, IncludesNesting body) => body);
        }

        public void AddXssAttackReportingTestMiddleware()
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
}