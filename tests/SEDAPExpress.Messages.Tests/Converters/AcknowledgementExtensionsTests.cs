using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class AcknowledgementExtensionsTests
{
    [Fact]
    public void ToWireStringTrueReturnsTRUE() =>
        Assert.Equal("TRUE", Acknowledgement.True.ToWireString());

    [Fact]
    public void ToWireStringFalseReturnsEmpty() =>
        Assert.Equal(string.Empty, Acknowledgement.False.ToWireString());

    [Theory]
    [InlineData("TRUE", Acknowledgement.True)]
    [InlineData("true", Acknowledgement.True)]
    [InlineData("True", Acknowledgement.True)]
    [InlineData("FALSE", Acknowledgement.False)]
    [InlineData("", Acknowledgement.False)]
    public void FromWireStringReturnsExpectedEnum(string input, Acknowledgement expected) =>
        Assert.Equal(expected, AcknowledgementExtensions.FromWireString(input.AsSpan()));
}
