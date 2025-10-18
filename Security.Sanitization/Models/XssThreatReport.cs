namespace Security.Sanitization.Models;

public sealed class XssThreatReport : IXssThreatReport
{
    private readonly ConcurrentBag<XssFinding> _findings = [];

    public IReadOnlyCollection<XssFinding> Findings => _findings.ToArray();
    public bool HasFindings => !_findings.IsEmpty;

    public void AddFinding(XssFinding finding) => _findings.Add(finding);
}