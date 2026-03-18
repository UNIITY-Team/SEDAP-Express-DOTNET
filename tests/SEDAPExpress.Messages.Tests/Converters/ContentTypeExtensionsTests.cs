using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class ContentTypeExtensionsTests
{
    [Theory]
    [InlineData(ContentType.Sedap, "SEDAP")]
    [InlineData(ContentType.Ascii, "ASCII")]
    [InlineData(ContentType.Nmea, "NMEA")]
    [InlineData(ContentType.Xml, "XML")]
    [InlineData(ContentType.Json, "JSON")]
    [InlineData(ContentType.Binary, "BINARY")]
    public void ToWireStringReturnsExpectedString(ContentType input, string expected) =>
        Assert.Equal(expected, input.ToWireString());

    [Theory]
    [InlineData("SEDAP", ContentType.Sedap)]
    [InlineData("sedap", ContentType.Sedap)]
    [InlineData("ASCII", ContentType.Ascii)]
    [InlineData("NMEA", ContentType.Nmea)]
    [InlineData("XML", ContentType.Xml)]
    [InlineData("JSON", ContentType.Json)]
    [InlineData("BINARY", ContentType.Binary)]
    public void TryFromWireStringReturnsExpectedEnum(string input, ContentType expected)
    {
        bool result = ContentTypeExtensions.TryFromWireString(input.AsSpan(), out ContentType actual);
        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryFromWireStringReturnsFalseForUnknownInput()
    {
        bool result = ContentTypeExtensions.TryFromWireString("UNKNOWN".AsSpan(), out _);
        Assert.False(result);
    }
}
