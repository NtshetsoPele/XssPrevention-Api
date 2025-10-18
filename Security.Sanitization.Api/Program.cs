var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRequestSanitizer(
    sanitizerOptions => sanitizerOptions.DeepSanitization = true);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRequestHeadersAndBodySanitizers();

app.MapXssHeaderTestRoute(Endpoints.InjectedHeadersRoute);
app.MapXssTopLevelBodyTestRoute(Endpoints.InjectedTopLevelBodyRoute);
app.MapXssNestingBodyTestRoute(Endpoints.InjectedNestingBodyRoute);

// Disable when running RouteTests.cs tests
#if MALICE_REPORTING
app.AddXssAttackReportingTestMiddleware();
#endif

app.Run();