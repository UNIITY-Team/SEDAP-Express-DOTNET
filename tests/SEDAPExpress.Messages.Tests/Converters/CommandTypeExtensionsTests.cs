using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class CommandTypeExtensionsTests
{
    [Theory]
    [InlineData(CommandType.Poweroff, (byte)0x00)]
    [InlineData(CommandType.Reboot, (byte)0x01)]
    [InlineData(CommandType.Shutdown, (byte)0x02)]
    [InlineData(CommandType.Start, (byte)0x03)]
    [InlineData(CommandType.Stop, (byte)0x04)]
    [InlineData(CommandType.Pause, (byte)0x05)]
    [InlineData(CommandType.Resume, (byte)0x06)]
    [InlineData(CommandType.Reset, (byte)0x07)]
    [InlineData(CommandType.Configure, (byte)0x08)]
    [InlineData(CommandType.Update, (byte)0x09)]
    [InlineData(CommandType.GenericAction, (byte)0xFF)]
    public void ToWireByteReturnsExpectedByte(CommandType input, byte expected) =>
        Assert.Equal(expected, input.ToWireByte());

    [Theory]
    [InlineData((byte)0x00, CommandType.Poweroff)]
    [InlineData((byte)0x01, CommandType.Reboot)]
    [InlineData((byte)0x02, CommandType.Shutdown)]
    [InlineData((byte)0x03, CommandType.Start)]
    [InlineData((byte)0x04, CommandType.Stop)]
    [InlineData((byte)0x05, CommandType.Pause)]
    [InlineData((byte)0x06, CommandType.Resume)]
    [InlineData((byte)0x07, CommandType.Reset)]
    [InlineData((byte)0x08, CommandType.Configure)]
    [InlineData((byte)0x09, CommandType.Update)]
    [InlineData((byte)0xFF, CommandType.GenericAction)]
    public void TryFromWireByteReturnsExpectedEnum(byte input, CommandType expected)
    {
        bool result = CommandTypeExtensions.TryFromWireByte(input, out CommandType actual);
        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryFromWireByteReturnsFalseForUnknownInput()
    {
        bool result = CommandTypeExtensions.TryFromWireByte(0x10, out _);
        Assert.False(result);
    }
}
