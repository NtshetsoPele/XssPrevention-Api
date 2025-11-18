namespace Security.Sanitization.Extensions;

public static class RequestSanitizerRegistrationExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRequestSanitizer(Action<RequestSanitizerOptions>? configure = null)
        {
            var options = new RequestSanitizerOptions();
            configure?.Invoke(options);

            return services
                .AddSingleton(options)
                .AddSingleton<IInputSanitizer, HtmlInputSanitizer>();
        }
    }
}