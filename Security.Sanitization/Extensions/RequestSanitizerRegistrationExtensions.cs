namespace Security.Sanitization.Extensions;

public static class RequestSanitizerRegistrationExtensions
{
    public static IServiceCollection AddRequestSanitizer(
        this IServiceCollection services,
        Action<RequestSanitizerOptions>? configure = null)
    {
        var options = new RequestSanitizerOptions();
        configure?.Invoke(options);

        return services
            .AddSingleton(options)
            .AddSingleton<IInputSanitizer, HtmlInputSanitizer>();
    }
}