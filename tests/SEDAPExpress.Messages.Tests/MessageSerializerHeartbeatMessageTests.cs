using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerHeartbeatMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerHeartbeatMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        var input = "HEARTBEAT;18;661D64C0;129E;U;;;LASSY";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<HeartbeatMessage>(actualBase);
        var expected = new HeartbeatMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY"
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new HeartbeatMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY"
        );
        var actual = _sut.Serialize(message);
        var expected = "HEARTBEAT;18;661D64C0;129E;U;;;LASSY";
        Assert.Equal(expected, actual);
    }
}
