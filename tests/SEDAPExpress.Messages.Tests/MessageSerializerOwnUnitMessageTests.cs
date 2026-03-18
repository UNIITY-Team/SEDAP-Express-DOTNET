using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerOwnUnitMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerOwnUnitMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        // Fields after common header: Lat;Lon;Alt;Speed;Course;Heading;Roll;Pitch;Name;Sidc
        var input = "OWNUNIT;18;661D64C0;129E;U;;;48.137154;11.576124;500;10;90";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<OwnUnitMessage>(actualBase);
        var expected = new OwnUnitMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Latitude: 48.137154,
            Longitude: 11.576124,
            Altitude: 500.0,
            Speed: 10.0,
            Course: 90.0,
            Heading: null,
            Roll: null,
            Pitch: null,
            Name: null,
            Sidc: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new OwnUnitMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Latitude: 48.137154,
            Longitude: 11.576124,
            Altitude: 500.0,
            Speed: 10.0,
            Course: 90.0,
            Heading: null,
            Roll: null,
            Pitch: null,
            Name: null,
            Sidc: null
        );
        var actual = _sut.Serialize(message);
        var expected = "OWNUNIT;18;661D64C0;129E;U;;;48.137154;11.576124;500;10;90";
        Assert.Equal(expected, actual);
    }
}
