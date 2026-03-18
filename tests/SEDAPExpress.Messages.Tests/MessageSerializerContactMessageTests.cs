using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerContactMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerContactMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        // Fields after common header: ContactId;DeleteMode;Lat;Lon;Alt;RelX;RelY;RelZ;Speed;Course;Heading;Roll;Pitch;Width;Length;Height;Name;Source;Sidc;Mmsi;Icao;Media;Comment
        var input = "CONTACT;18;661D64C0;129E;U;;;CTK001;FALSE;48.5;11.2;100";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<ContactMessage>(actualBase);
        var expected = new ContactMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            ContactId: "CTK001",
            DeleteMode: DeleteMode.False,
            Latitude: 48.5,
            Longitude: 11.2,
            Altitude: 100.0,
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
            Source: null,
            Sidc: null,
            Mmsi: null,
            Icao: null,
            MultimediaData: null,
            Comment: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new ContactMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            ContactId: "CTK001",
            DeleteMode: DeleteMode.False,
            Latitude: 48.5,
            Longitude: 11.2,
            Altitude: 100.0,
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
            Source: null,
            Sidc: null,
            Mmsi: null,
            Icao: null,
            MultimediaData: null,
            Comment: null
        );
        var actual = _sut.Serialize(message);
        var expected = "CONTACT;18;661D64C0;129E;U;;;CTK001;FALSE;48.5;11.2;100";
        Assert.Equal(expected, actual);
    }
}
