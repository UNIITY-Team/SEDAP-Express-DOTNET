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
    public void AlertMessageCanBeDeserialized()
    {
        var input = "TEXT;13;661D44D2;324E;S;TRUE;;;1;NONE;\"This is an alert!\"";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<TextMessage>(actualBase);
        var expected = new TextMessage(
            Number: 0x13,
            Time: 0x661D44D2,
            Sender: "324E",
            Classification: Classification.Secret,
            Acknowledgement: Acknowledgement.True,
            Mac: null,
            Recipient: null,
            Type: TextType.Alert,
            Encoding: DataEncoding.None,
            TextContent: "\"This is an alert!\"",
            Reference: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void AlertMessageCanBeSerialized()
    {
        var message = new TextMessage(
            Number: 0x13,
            Time: 0x661D44D2,
            Sender: "324E",
            Classification: Classification.Secret,
            Acknowledgement: Acknowledgement.True,
            Mac: null,
            Recipient: null,
            Type: TextType.Alert,
            Encoding: DataEncoding.None,
            TextContent: "\"This is an alert!\"",
            Reference: null
        );
        var actual = _sut.Serialize(message);
        var expected = "TEXT;13;661D44D2;324E;S;TRUE;;;1;NONE;\"This is an alert!\"";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void WarningMessageCanBeDeserialized()
    {
        var input = "TEXT;74;661D458E;324E;C;TRUE;;;2;NONE;\"This is a warning!\"";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<TextMessage>(actualBase);
        var expected = new TextMessage(
            Number: 0x74,
            Time: 0x661D458E,
            Sender: "324E",
            Classification: Classification.Confidential,
            Acknowledgement: Acknowledgement.True,
            Mac: null,
            Recipient: null,
            Type: TextType.Warning,
            Encoding: DataEncoding.None,
            TextContent: "\"This is a warning!\"",
            Reference: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void WarningMessageCanBeSerialized()
    {
        var message = new TextMessage(
            Number: 0x74,
            Time: 0x661D458E,
            Sender: "324E",
            Classification: Classification.Confidential,
            Acknowledgement: Acknowledgement.True,
            Mac: null,
            Recipient: null,
            Type: TextType.Warning,
            Encoding: DataEncoding.None,
            TextContent: "\"This is a warning!\"",
            Reference: null
        );
        var actual = _sut.Serialize(message);
        var expected = "TEXT;74;661D458E;324E;C;TRUE;;;2;NONE;\"This is a warning!\"";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void NoticeMessageWithReferenceCanBeDeserialized()
    {
        // Encoding field is empty in the wire format — parsed as null, serialized back as empty string.
        var input = "TEXT;15;661D6565;324E;R;;;;3;;\"This is a notice!\";1133";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<TextMessage>(actualBase);
        var expected = new TextMessage(
            Number: 0x15,
            Time: 0x661D6565,
            Sender: "324E",
            Classification: Classification.Restricted,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: null,
            Type: TextType.Notice,
            Encoding: null,
            TextContent: "\"This is a notice!\"",
            Reference: "1133"
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void NoticeMessageWithReferenceCanBeSerialized()
    {
        var message = new TextMessage(
            Number: 0x15,
            Time: 0x661D6565,
            Sender: "324E",
            Classification: Classification.Restricted,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: null,
            Type: TextType.Notice,
            Encoding: null,
            TextContent: "\"This is a notice!\"",
            Reference: "1133"
        );
        var actual = _sut.Serialize(message);
        var expected = "TEXT;15;661D6565;324E;R;;;;3;;\"This is a notice!\";1133";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ChatMessageWithBase64EncodingCanBeDeserialized()
    {
        var input = "TEXT;26;661D7032;324E;U;;;E4F1;4;BASE64;IlRoaXMgaXMgYSBjaGF0IG1lc3NhZ2UhIg==";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<TextMessage>(actualBase);
        var expected = new TextMessage(
            Number: 0x26,
            Time: 0x661D7032,
            Sender: "324E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "E4F1",
            Type: TextType.Chat,
            Encoding: DataEncoding.Base64,
            TextContent: "\"This is a chat message!\"",
            Reference: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void ChatMessageWithBase64EncodingCanBeSerialized()
    {
        var message = new TextMessage(
            Number: 0x26,
            Time: 0x661D7032,
            Sender: "324E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "E4F1",
            Type: TextType.Chat,
            Encoding: DataEncoding.Base64,
            TextContent: "\"This is a chat message!\"",
            Reference: null
        );
        var actual = _sut.Serialize(message);
        var expected = "TEXT;26;661D7032;324E;U;;;E4F1;4;BASE64;IlRoaXMgaXMgYSBjaGF0IG1lc3NhZ2UhIg==";
        Assert.Equal(expected, actual);
    }
}
