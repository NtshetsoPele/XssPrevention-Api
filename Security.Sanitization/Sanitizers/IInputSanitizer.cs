namespace Security.Sanitization.Sanitizers;

public interface IInputSanitizer
{
    string Sanitize(string input);
}