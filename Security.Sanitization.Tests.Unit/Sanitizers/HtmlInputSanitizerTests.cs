namespace Security.Sanitization.Tests.Unit.Sanitizers;

public sealed class HtmlInputSanitizerTests
{
    private readonly HtmlInputSanitizer _sut = new(); // system under test

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    //          Subject__Condition_____________Result
    public void Sanitize_NullEmptyOrBlankInput_EmptyString(string? input)
    {
        // Arrange
        // Act
        var result = _sut.Sanitize(input!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("Hello world!")]
    [InlineData("<b>Bold Text</b>")]
    public void Sanitize_CleanInput_CleanInputUnchanged(string input)
    {
        // Arrange
        // Act
        var result = _sut.Sanitize(input);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void Sanitize_InjectedInput_UnsafeHtmlStripped()
    {
        // Arrange
        const string input = "<script>alert('xss');</script><b>Safe</b>";

        // Act
        var result = _sut.Sanitize(input);

        // Assert
        Assert.Equal("<b>Safe</b>", result);
    }

    [Fact]
    public void Sanitize_BadAttribute_AttributeRemoved()
    {
        // Arrange
        const string input = "<img src='test.jpg' onerror='alert(1)' />";

        // Act
        var result = _sut.Sanitize(input);

        // Assert
        Assert.Equal("<img src=\"test.jpg\">", result);
    }
}