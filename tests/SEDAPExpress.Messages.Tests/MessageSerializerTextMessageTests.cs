using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerTextMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerTextMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        var input = "TEXT;18;661D64C0;129E;U;;;LASSY;4;NONE;Hello World";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<TextMessage>(actualBase);
        var expected = new TextMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY",
            Type: TextType.Chat,
            Encoding: DataEncoding.None,
            TextContent: "Hello World",
            Reference: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new TextMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY",
            Type: TextType.Chat,
            Encoding: DataEncoding.None,
            TextContent: "Hello World",
            Reference: null
        );
        var actual = _sut.Serialize(message);
        var expected = "TEXT;18;661D64C0;129E;U;;;LASSY;4;NONE;Hello World";
        Assert.Equal(expected, actual);
    }
}
