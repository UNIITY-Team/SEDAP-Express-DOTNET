using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageTypeExtensionsTests
{
    [Theory]
    [InlineData(MessageType.Acknowledge, "ACKNOWLEDGE")]
    [InlineData(MessageType.Command, "COMMAND")]
    [InlineData(MessageType.Contact, "CONTACT")]
    [InlineData(MessageType.Point, "POINT")]
    [InlineData(MessageType.Emission, "EMISSION")]
    [InlineData(MessageType.Generic, "GENERIC")]
    [InlineData(MessageType.Graphic, "GRAPHIC")]
    [InlineData(MessageType.Heartbeat, "HEARTBEAT")]
    [InlineData(MessageType.Keyexchange, "KEYEXCHANGE")]
    [InlineData(MessageType.Meteo, "METEO")]
    [InlineData(MessageType.OwnUnit, "OWNUNIT")]
    [InlineData(MessageType.Resend, "RESEND")]
    [InlineData(MessageType.Status, "STATUS")]
    [InlineData(MessageType.Text, "TEXT")]
    [InlineData(MessageType.Timesync, "TIMESYNC")]
    public void ToWireStringReturnsExpectedString(MessageType input, string expected) =>
        Assert.Equal(expected, input.ToWireString());

    [Theory]
    [InlineData("ACKNOWLEDGE", MessageType.Acknowledge)]
    [InlineData("acknowledge", MessageType.Acknowledge)]
    [InlineData("COMMAND", MessageType.Command)]
    [InlineData("CONTACT", MessageType.Contact)]
    [InlineData("POINT", MessageType.Point)]
    [InlineData("EMISSION", MessageType.Emission)]
    [InlineData("GENERIC", MessageType.Generic)]
    [InlineData("GRAPHIC", MessageType.Graphic)]
    [InlineData("HEARTBEAT", MessageType.Heartbeat)]
    [InlineData("KEYEXCHANGE", MessageType.Keyexchange)]
    [InlineData("METEO", MessageType.Meteo)]
    [InlineData("OWNUNIT", MessageType.OwnUnit)]
    [InlineData("RESEND", MessageType.Resend)]
    [InlineData("STATUS", MessageType.Status)]
    [InlineData("TEXT", MessageType.Text)]
    [InlineData("TIMESYNC", MessageType.Timesync)]
    public void TryFromWireStringReturnsExpectedEnum(string input, MessageType expected)
    {
        bool result = MessageTypeExtensions.TryFromWireString(input.AsSpan(), out MessageType actual);
        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryFromWireStringReturnsFalseForUnknownInput()
    {
        bool result = MessageTypeExtensions.TryFromWireString("UNKNOWN".AsSpan(), out _);
        Assert.False(result);
    }
}
