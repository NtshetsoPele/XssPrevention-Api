namespace Security.Sanitization.Api.Routes;

internal static class Endpoints
{
    internal static string InjectedHeadersRoute => "/test-injected-headers";
    internal static string InjectedTopLevelBodyRoute => "/test-injected-top-level-body";
    internal static string InjectedNestingBodyRoute => "/test-injected-nesting-body";
}