using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class ClassificationExtensionsTests
{
    [Theory]
    [InlineData(Classification.Public, 'P')]
    [InlineData(Classification.Unclas, 'U')]
    [InlineData(Classification.Restricted, 'R')]
    [InlineData(Classification.Confidential, 'C')]
    [InlineData(Classification.Secret, 'S')]
    [InlineData(Classification.TopSecret, 'T')]
    [InlineData(Classification.None, '-')]
    public void ToWireCharReturnsExpectedChar(Classification input, char expected) =>
        Assert.Equal(expected, input.ToWireChar());

    [Theory]
    [InlineData('P', Classification.Public)]
    [InlineData('U', Classification.Unclas)]
    [InlineData('R', Classification.Restricted)]
    [InlineData('C', Classification.Confidential)]
    [InlineData('S', Classification.Secret)]
    [InlineData('T', Classification.TopSecret)]
    [InlineData('-', Classification.None)]
    public void TryFromWireCharReturnsExpectedEnum(char input, Classification expected)
    {
        bool result = ClassificationExtensions.TryFromWireChar(input, out Classification actual);
        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData('p', Classification.Public)]
    [InlineData('u', Classification.Unclas)]
    [InlineData('s', Classification.Secret)]
    public void TryFromWireCharIsCaseInsensitive(char input, Classification expected)
    {
        bool result = ClassificationExtensions.TryFromWireChar(input, out Classification actual);
        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryFromWireCharReturnsFalseForUnknownInput()
    {
        bool result = ClassificationExtensions.TryFromWireChar('X', out _);
        Assert.False(result);
    }
}
