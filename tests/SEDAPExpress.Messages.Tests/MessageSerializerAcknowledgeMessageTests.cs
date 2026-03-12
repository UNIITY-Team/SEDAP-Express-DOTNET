using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerAcknowledgeMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerAcknowledgeMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        var input = "ACKNOWLEDGE;18;661D64C0;129E;R;;;LASSY;COMMAND;2B";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<AcknowledgeMessage>(actualBase);
        var expected = new AcknowledgeMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Restricted,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY",
            AckedMessageType: MessageType.Command,
            AckedMessageNumber: 0x2B
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new AcknowledgeMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Restricted,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY",
            AckedMessageType: MessageType.Command,
            AckedMessageNumber: 0x2B
        );
        var actual = _sut.Serialize(message);
        var expected = "ACKNOWLEDGE;18;661D64C0;129E;R;;;LASSY;COMMAND;2B";
        Assert.Equal(expected, actual);
    }
}
