namespace Security.Sanitization.RequestBodyExamples;

public sealed record IncludesNesting : TopLevelOnly
{
    public required string[] NestedLevelMalice { get; init; }
}