using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class ContactSourceExtensionsTests
{
    [Theory]
    [InlineData(ContactSource.None, ' ')]
    [InlineData(ContactSource.Radar, 'R')]
    [InlineData(ContactSource.Ais, 'A')]
    [InlineData(ContactSource.Iff, 'I')]
    [InlineData(ContactSource.Sonar, 'S')]
    [InlineData(ContactSource.Ew, 'E')]
    [InlineData(ContactSource.Optical, 'O')]
    [InlineData(ContactSource.Synthetic, 'Y')]
    [InlineData(ContactSource.Manual, 'M')]
    public void ToWireCharReturnsExpectedChar(ContactSource input, char expected) =>
        Assert.Equal(expected, input.ToWireChar());

    [Theory]
    [InlineData(' ', ContactSource.None)]
    [InlineData('R', ContactSource.Radar)]
    [InlineData('r', ContactSource.Radar)]
    [InlineData('A', ContactSource.Ais)]
    [InlineData('a', ContactSource.Ais)]
    [InlineData('I', ContactSource.Iff)]
    [InlineData('i', ContactSource.Iff)]
    [InlineData('S', ContactSource.Sonar)]
    [InlineData('s', ContactSource.Sonar)]
    [InlineData('E', ContactSource.Ew)]
    [InlineData('e', ContactSource.Ew)]
    [InlineData('O', ContactSource.Optical)]
    [InlineData('o', ContactSource.Optical)]
    [InlineData('Y', ContactSource.Synthetic)]
    [InlineData('y', ContactSource.Synthetic)]
    [InlineData('M', ContactSource.Manual)]
    [InlineData('m', ContactSource.Manual)]
    public void FromWireCharReturnsExpectedEnum(char input, ContactSource expected) =>
        Assert.Equal(expected, ContactSourceExtensions.FromWireChar(input));

    [Fact]
    public void FromWireCharReturnsNoneForUnknownInput() =>
        Assert.Equal(ContactSource.None, ContactSourceExtensions.FromWireChar('X'));
}
