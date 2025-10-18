namespace Security.Sanitization.Models;

public interface IXssThreatReport
{
    IReadOnlyCollection<XssFinding> Findings { get; }
    bool HasFindings { get; }

    void AddFinding(XssFinding finding);
}