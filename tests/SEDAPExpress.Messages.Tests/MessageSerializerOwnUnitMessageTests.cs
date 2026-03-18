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
        var input = "OWNUNIT;11;1B351C87;22AA;U;TRUE;4389F10D;77.88;-10.12;5577.0;33.44;55.66;1.1;-2.2;3.3;Ownunit;SFGPIB----H----";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<OwnUnitMessage>(actualBase);
        var expected = new OwnUnitMessage(
            Number: 0x11,
            Time: 0x1B351C87,
            Sender: "22AA",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.True,
            Mac: "4389F10D",
            Latitude: 77.88,
            Longitude: -10.12,
            Altitude: 5577.0,
            Speed: 33.44,
            Course: 55.66,
            Heading: 1.1,
            Roll: -2.2,
            Pitch: 3.3,
            Name: "Ownunit",
            Sidc: "SFGPIB----H----"
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new OwnUnitMessage(
            Number: 0x11,
            Time: 0x1B351C87,
            Sender: "22AA",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.True,
            Mac: "4389F10D",
            Latitude: 77.88,
            Longitude: -10.12,
            Altitude: 5577.0,
            Speed: 33.44,
            Course: 55.66,
            Heading: 1.1,
            Roll: -2.2,
            Pitch: 3.3,
            Name: "Ownunit",
            Sidc: "SFGPIB----H----"
        );
        var actual = _sut.Serialize(message);
        var expected = "OWNUNIT;11;1B351C87;22AA;U;TRUE;4389F10D;77.88;-10.12;5577;33.44;55.66;1.1;-2.2;3.3;Ownunit;SFGPIB----H----";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MessageWithNullOptionalFieldsCanBeDeserialized()
    {
        var input = "OWNUNIT;5E;661D4410;66A3;R;;;53.32;8.11;0;5.5;21;22;;;FGS Bayern;SFSPFCLFF------";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<OwnUnitMessage>(actualBase);
        var expected = new OwnUnitMessage(
            Number: 0x5E,
            Time: 0x661D4410,
            Sender: "66A3",
            Classification: Classification.Restricted,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Latitude: 53.32,
            Longitude: 8.11,
            Altitude: 0.0,
            Speed: 5.5,
            Course: 21.0,
            Heading: 22.0,
            Roll: null,
            Pitch: null,
            Name: "FGS Bayern",
            Sidc: "SFSPFCLFF------"
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageWithNullOptionalFieldsCanBeSerialized()
    {
        var message = new OwnUnitMessage(
            Number: 0x5E,
            Time: 0x661D4410,
            Sender: "66A3",
            Classification: Classification.Restricted,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Latitude: 53.32,
            Longitude: 8.11,
            Altitude: 0.0,
            Speed: 5.5,
            Course: 21.0,
            Heading: 22.0,
            Roll: null,
            Pitch: null,
            Name: "FGS Bayern",
            Sidc: "SFSPFCLFF------"
        );
        var actual = _sut.Serialize(message);
        var expected = "OWNUNIT;5E;661D4410;66A3;R;;;53.32;8.11;0;5.5;21;22;;;FGS Bayern;SFSPFCLFF------";
        Assert.Equal(expected, actual);
    }
}
