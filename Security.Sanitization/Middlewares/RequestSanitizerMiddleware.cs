namespace Security.Sanitization.Middlewares;

public abstract class RequestSanitizerMiddleware
{
    protected internal static IXssThreatReport EnsureThreatReportFeature(IFeatureCollection features)
    {
        if (features.Get<IXssThreatReport>() is { } existing)
        {
            return existing;
        }

        return SetNewReport(features);
    }

    protected internal static bool NoMalice(string original, string sanitized) =>
        string.Equals(original, sanitized, StringComparison.OrdinalIgnoreCase);

    protected internal static void AddMaliceFinding(IXssThreatReport report, string location, string original) =>
        report.AddFinding(new XssFinding(location, original));

    private static XssThreatReport SetNewReport(IFeatureCollection features)
    {
        var report = new XssThreatReport();
        features.Set<IXssThreatReport>(report);
        return report;
    }
}