namespace Security.Sanitization.Tests.Integration.Api;

public sealed class RouteTests(WebApplicationFactory<Program> factory) :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private const string ScriptInjection = "<script>alert('xss');</script>";
    private const string SafeMarkup = "<b>Safe</b>";

    [Fact]
    //                Subject________Condition_______Result     
    public async Task InjectedHeader_MaliciousHeader_SanitizedHeader()
    {
        // Arrange
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("X-Malicious-Header", ScriptInjection);

        // Act
        var responseText = await GetResponseTextAsync(() =>
            _client.GetAsync(Endpoints.InjectedHeadersRoute));

        // Assert
        responseText.Should().NotContain(ScriptInjection);
    }

    [Fact]
    public async Task InjectedTopLevelRequestBody_MaliciousProperty_SanitizedProperty()
    {
        var topLevelRequest = CreateTopLevelRequest();
        var content = CreateJsonContent(topLevelRequest);

        var responseText = await GetResponseTextAsync(() =>
            _client.PostAsync(Endpoints.InjectedTopLevelBodyRoute, content));

        responseText.Should()
            .Contain(SafeMarkup)
            .And.NotContain($"{ScriptInjection}{SafeMarkup}");
    }

    [Fact]
    public async Task InjectedNestedRequestBody_MaliciousNestedProperties_SanitizedProperties()
    {
        var nestedRequest = CreateNestedRequest();
        var content = CreateJsonContent(nestedRequest);

        var responseText = await GetResponseTextAsync(() =>
            _client.PostAsync(Endpoints.InjectedNestingBodyRoute, content));

        responseText.Should()
            .Contain(SafeMarkup)
            .And.Contain("<img src=\\\"test.jpg\\\">")
            .And.NotContain($"{ScriptInjection}{SafeMarkup}");
    }

    private static StringContent CreateJsonContent(object payload) =>
        new(JsonSerializer.Serialize(payload), Encoding.UTF8, MediaTypeNames.Application.Json);

    private static TopLevelOnly CreateTopLevelRequest() =>
        new()
        {
            TopLevelMalice = $"{ScriptInjection}{SafeMarkup}"
        };

    private static IncludesNesting CreateNestedRequest() =>
        new()
        {
            TopLevelMalice = $"{ScriptInjection}{SafeMarkup}",
            NestedLevelMalice = ["<img src='test.jpg' onerror='alert(1)' />"]
        };

    private static async Task<string> GetResponseTextAsync(Func<Task<HttpResponseMessage>> sendAsync)
    {
        var response = await sendAsync();
        return await response.Content.ReadAsStringAsync();
    }
}