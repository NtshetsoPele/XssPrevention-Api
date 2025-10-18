namespace Security.Sanitization.Middlewares;

public sealed class RequestBodySanitizerMiddleware(
    RequestDelegate next, 
    IInputSanitizer sanitizer,
    RequestSanitizerOptions options) : RequestSanitizerMiddleware
{
    private const int StreamResetPosition = 0;

    public async Task InvokeAsync(HttpContext context)
    {
        await SanitizeRequestBodyAsync(context);
        await next(context);
    }

    private async Task SanitizeRequestBodyAsync(HttpContext context)
    {
        var request = context.Request;
        if (!IsJson(request.ContentType))
        {
            return;
        }

        request.EnableBuffering();

        var bodyText = await ReadBodyAsync(request);
        if (string.IsNullOrWhiteSpace(bodyText))
        {
            return;
        }

        var report = EnsureThreatReportFeature(context.Features);

        using var parsedDocument = JsonDocument.Parse(bodyText);
        var sanitizedDocument = SanitizeDocument(parsedDocument, report);

        var sanitizedJson = JsonSerializer.Serialize(sanitizedDocument);
        ReplaceBody(request, sanitizedJson);
    }

    private static bool IsJson(string? contentType) =>
        contentType is { } type && 
        type.Contains(Constants.JsonContent, StringComparison.OrdinalIgnoreCase);

    private static async Task<string> ReadBodyAsync(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = StreamResetPosition;
        return body;
    }

    private object? SanitizeDocument(JsonDocument document, IXssThreatReport report)
    {
        return options.DeepSanitization
            ? SanitizeJsonElement(document.RootElement, report, "$")
            : SanitizeTopLevel(document.RootElement, report);
    }

    private object SanitizeJsonElement(JsonElement element, IXssThreatReport report, string path)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => SanitizeString(element.GetString()!, report, path),
            JsonValueKind.Object =>
                element
                    .EnumerateObject()
                    .ToDictionary(
                        jsonProperty => jsonProperty.Name,
                        jsonProperty => SanitizeJsonElement(jsonProperty.Value, report, $"{path}.{jsonProperty.Name}")),
            JsonValueKind.Array =>
                element
                    .EnumerateArray()
                    .Select((item, index) => SanitizeJsonElement(item, report, $"{path}[{index}]"))
                    .ToList(),
            _ => element.Clone()
        };
    }

    private string SanitizeString(string original, IXssThreatReport report, string path)
    {
        var sanitized = sanitizer.Sanitize(original);
        if (NoMalice(original, sanitized))
        {
            return sanitized;
        }

        AddMaliceFinding(report, $"Body:{path}", original);
        return sanitized;
    }

    private Dictionary<string, object?> SanitizeTopLevel(JsonElement element, IXssThreatReport report)
    {
        var sanitizingDict = new Dictionary<string, object?>();
        foreach (var jsonProperty in element.EnumerateObject())
        {
            sanitizingDict[jsonProperty.Name] = 
                jsonProperty.Value.ValueKind is JsonValueKind.String
                    ? SanitizeString(jsonProperty.Value.GetString()!, report, $"$.{jsonProperty.Name}")
                    : jsonProperty.Value.Clone();
        }
        return sanitizingDict;
    }

    private static void ReplaceBody(HttpRequest request, string sanitizedJson)
    {
        var bytes = Encoding.UTF8.GetBytes(sanitizedJson);
        request.Body = new MemoryStream(bytes);
    }
}