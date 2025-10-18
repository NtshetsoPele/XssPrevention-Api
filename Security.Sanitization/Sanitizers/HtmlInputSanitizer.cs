namespace Security.Sanitization.Sanitizers;

public sealed class HtmlInputSanitizer : IInputSanitizer
{
    private readonly HtmlSanitizer _sanitizer = new();

    public string Sanitize(string input) =>
        string.IsNullOrWhiteSpace(input) ? 
            string.Empty : 
            _sanitizer.Sanitize(input);
}