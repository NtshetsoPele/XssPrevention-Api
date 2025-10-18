namespace Security.Sanitization.Middlewares;

public sealed class RequestHeadersSanitizerMiddleware(
    RequestDelegate next, 
    IInputSanitizer sanitizer) : RequestSanitizerMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        SanitizeRequestHeaders(context);
        await next(context);
    }

    private void SanitizeRequestHeaders(HttpContext context)
    {
        var report = EnsureThreatReportFeature(context.Features);
        var request = context.Request;

        foreach (var headerKey in GetHeaderKeys(request))
        {
            var original = GetHeader(request, headerKey);
            var sanitized = sanitizer.Sanitize(original);

            if (NoMalice(original, sanitized))
            {
                continue;
            }

            AddMaliceFinding(report, $"Header:{headerKey}", original);
            request.Headers[headerKey] = sanitized;
        }
    }

    private static List<string> GetHeaderKeys(HttpRequest request) => 
        request.Headers.Keys.ToList();

    private static string GetHeader(HttpRequest request, string key) =>
        request.Headers[key].ToString();
}