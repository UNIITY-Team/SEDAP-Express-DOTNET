using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class DeleteModeExtensionsTests
{
    [Fact]
    public void ToWireStringTrueReturnsTRUE() =>
        Assert.Equal("TRUE", DeleteMode.True.ToWireString());

    [Fact]
    public void ToWireStringFalseReturnsFALSE() =>
        Assert.Equal("FALSE", DeleteMode.False.ToWireString());

    [Theory]
    [InlineData("TRUE", DeleteMode.True)]
    [InlineData("true", DeleteMode.True)]
    [InlineData("False", DeleteMode.False)]
    [InlineData("FALSE", DeleteMode.False)]
    public void TryFromWireStringReturnsExpectedEnum(string input, DeleteMode expected)
    {
        bool result = DeleteModeExtensions.TryFromWireString(input.AsSpan(), out DeleteMode actual);
        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryFromWireStringReturnsFalseForUnknownInput()
    {
        bool result = DeleteModeExtensions.TryFromWireString("UNKNOWN".AsSpan(), out _);
        Assert.False(result);
    }
}
