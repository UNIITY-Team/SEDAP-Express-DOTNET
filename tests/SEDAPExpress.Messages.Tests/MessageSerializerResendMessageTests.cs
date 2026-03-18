using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerResendMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerResendMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        var input = "RESEND;18;661D64C0;129E;U;;;LASSY;COMMAND;2B";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<ResendMessage>(actualBase);
        var expected = new ResendMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY",
            MissingMessageType: MessageType.Command,
            MissingMessageNumber: 0x2B
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new ResendMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY",
            MissingMessageType: MessageType.Command,
            MissingMessageNumber: 0x2B
        );
        var actual = _sut.Serialize(message);
        var expected = "RESEND;18;661D64C0;129E;U;;;LASSY;COMMAND;2B";
        Assert.Equal(expected, actual);
    }
}
