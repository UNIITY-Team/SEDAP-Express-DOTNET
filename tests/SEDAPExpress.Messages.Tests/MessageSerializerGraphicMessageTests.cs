using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerGraphicMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerGraphicMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        // Fields: GraphicType;LineWidth;LineColor;FillColor;TextColor;Encoding;Annotation
        var input = "GRAPHIC;18;661D64C0;129E;U;;;4;2;FF0000FF;00FF00FF;0000FFFF;NONE;label";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<GraphicMessage>(actualBase);
        var expected = new GraphicMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            GraphicType: GraphicType.Circle,
            LineWidth: 2.0,
            LineColor: unchecked((int)0xFF0000FF),
            FillColor: unchecked((int)0x00FF00FF),
            TextColor: unchecked((int)0x0000FFFF),
            Encoding: DataEncoding.None,
            Annotation: "label"
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new GraphicMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            GraphicType: GraphicType.Circle,
            LineWidth: 2.0,
            LineColor: unchecked((int)0xFF0000FF),
            FillColor: unchecked((int)0x00FF00FF),
            TextColor: unchecked((int)0x0000FFFF),
            Encoding: DataEncoding.None,
            Annotation: "label"
        );
        var actual = _sut.Serialize(message);
        var expected = "GRAPHIC;18;661D64C0;129E;U;;;4;2;FF0000FF;00FF00FF;0000FFFF;NONE;label";
        Assert.Equal(expected, actual);
    }
}
