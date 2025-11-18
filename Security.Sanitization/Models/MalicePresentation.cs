namespace Security.Sanitization.Models;

internal sealed class MalicePresentation
{
    public ICollection<string> Findings { get; } = [];
}