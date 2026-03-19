using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class DeleteFlagTypeExtensionsTests
{
    [Fact]
    public void ToWireStringTrueReturnsTRUE() =>
        Assert.Equal("TRUE", DeleteFlagType.True.ToWireString());

    [Fact]
    public void ToWireStringFalseReturnsFALSE() =>
        Assert.Equal("FALSE", DeleteFlagType.False.ToWireString());

    [Theory]
    [InlineData("TRUE", DeleteFlagType.True)]
    [InlineData("true", DeleteFlagType.True)]
    [InlineData("False", DeleteFlagType.False)]
    [InlineData("FALSE", DeleteFlagType.False)]
    public void TryFromWireStringReturnsExpectedEnum(string input, DeleteFlagType expected)
    {
        bool result = DeleteFlagTypeExtensions.TryFromWireString(input.AsSpan(), out DeleteFlagType actual);
        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryFromWireStringReturnsFalseForUnknownInput()
    {
        bool result = DeleteFlagTypeExtensions.TryFromWireString("UNKNOWN".AsSpan(), out _);
        Assert.False(result);
    }
}
