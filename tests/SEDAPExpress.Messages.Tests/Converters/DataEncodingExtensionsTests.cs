using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class DataEncodingExtensionsTests
{
    [Fact]
    public void ToWireStringBase64ReturnsBASE64() =>
        Assert.Equal("BASE64", DataEncoding.Base64.ToWireString());

    [Fact]
    public void ToWireStringNoneReturnsNONE() =>
        Assert.Equal("NONE", DataEncoding.None.ToWireString());

    [Theory]
    [InlineData("BASE64", DataEncoding.Base64)]
    [InlineData("base64", DataEncoding.Base64)]
    [InlineData("NONE", DataEncoding.None)]
    [InlineData("none", DataEncoding.None)]
    public void TryFromWireStringReturnsExpectedEnum(string input, DataEncoding expected)
    {
        bool result = DataEncodingExtensions.TryFromWireString(input.AsSpan(), out DataEncoding actual);
        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryFromWireStringReturnsFalseForUnknownInput()
    {
        bool result = DataEncodingExtensions.TryFromWireString("UNKNOWN".AsSpan(), out _);
        Assert.False(result);
    }
}
