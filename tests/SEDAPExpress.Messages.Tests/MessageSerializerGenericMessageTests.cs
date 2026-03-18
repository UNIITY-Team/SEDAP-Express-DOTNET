using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerGenericMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerGenericMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        var input = "GENERIC;18;661D64C0;129E;U;;;SEDAP;NONE;some content here";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<GenericMessage>(actualBase);
        var expected = new GenericMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            ContentType: ContentType.Sedap,
            Encoding: DataEncoding.None,
            Content: "some content here"
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new GenericMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            ContentType: ContentType.Sedap,
            Encoding: DataEncoding.None,
            Content: "some content here"
        );
        var actual = _sut.Serialize(message);
        var expected = "GENERIC;18;661D64C0;129E;U;;;SEDAP;NONE;some content here";
        Assert.Equal(expected, actual);
    }
}
