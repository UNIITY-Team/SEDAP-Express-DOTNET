using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerPointMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerPointMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        // Fields after common header: ContactId;DeleteMode;Lat;Lon;Alt;RelX;RelY;RelZ;Speed;Course;Heading;Roll;Pitch;Width;Length;Height;Name;Sidc;Media;Comment
        var input = "POINT;18;661D64C0;129E;U;;;PT001;FALSE;52.1;13.4;50";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<PointMessage>(actualBase);
        var expected = new PointMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            ContactId: "PT001",
            DeleteMode: DeleteMode.False,
            Latitude: 52.1,
            Longitude: 13.4,
            Altitude: 50.0,
            RelativeXDistance: null,
            RelativeYDistance: null,
            RelativeZDistance: null,
            Speed: null,
            Course: null,
            Heading: null,
            Roll: null,
            Pitch: null,
            Width: null,
            Length: null,
            Height: null,
            Name: null,
            Sidc: null,
            MultimediaData: null,
            Comment: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new PointMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            ContactId: "PT001",
            DeleteMode: DeleteMode.False,
            Latitude: 52.1,
            Longitude: 13.4,
            Altitude: 50.0,
            RelativeXDistance: null,
            RelativeYDistance: null,
            RelativeZDistance: null,
            Speed: null,
            Course: null,
            Heading: null,
            Roll: null,
            Pitch: null,
            Width: null,
            Length: null,
            Height: null,
            Name: null,
            Sidc: null,
            MultimediaData: null,
            Comment: null
        );
        var actual = _sut.Serialize(message);
        var expected = "POINT;18;661D64C0;129E;U;;;PT001;FALSE;52.1;13.4;50";
        Assert.Equal(expected, actual);
    }
}
