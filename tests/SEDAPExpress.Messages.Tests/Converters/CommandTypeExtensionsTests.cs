using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class CommandTypeExtensionsTests
{
    [Theory]
    [InlineData(CommandType.Poweroff, (byte)0x00)]
    [InlineData(CommandType.Restart, (byte)0x01)]
    [InlineData(CommandType.Standby, (byte)0x02)]
    [InlineData(CommandType.SyncTime, (byte)0x03)]
    [InlineData(CommandType.SendStatus, (byte)0x04)]
    [InlineData(CommandType.Move, (byte)0x05)]
    [InlineData(CommandType.Rotate, (byte)0x06)]
    [InlineData(CommandType.Loiter, (byte)0x07)]
    [InlineData(CommandType.ScanArea, (byte)0x08)]
    [InlineData(CommandType.TakePhoto, (byte)0x09)]
    [InlineData(CommandType.MakeVideo, (byte)0x0A)]
    [InlineData(CommandType.LiveVideo, (byte)0x0B)]
    [InlineData(CommandType.Engagement, (byte)0x0C)]
    [InlineData(CommandType.Sanitize, (byte)0xEE)]
    [InlineData(CommandType.GenericAction, (byte)0xFF)]
    public void ToWireByteReturnsExpectedByte(CommandType input, byte expected) =>
        Assert.Equal(expected, input.ToWireByte());

    [Theory]
    [InlineData((byte)0x00, CommandType.Poweroff)]
    [InlineData((byte)0x01, CommandType.Restart)]
    [InlineData((byte)0x02, CommandType.Standby)]
    [InlineData((byte)0x03, CommandType.SyncTime)]
    [InlineData((byte)0x04, CommandType.SendStatus)]
    [InlineData((byte)0x05, CommandType.Move)]
    [InlineData((byte)0x06, CommandType.Rotate)]
    [InlineData((byte)0x07, CommandType.Loiter)]
    [InlineData((byte)0x08, CommandType.ScanArea)]
    [InlineData((byte)0x09, CommandType.TakePhoto)]
    [InlineData((byte)0x0A, CommandType.MakeVideo)]
    [InlineData((byte)0x0B, CommandType.LiveVideo)]
    [InlineData((byte)0x0C, CommandType.Engagement)]
    [InlineData((byte)0xEE, CommandType.Sanitize)]
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
