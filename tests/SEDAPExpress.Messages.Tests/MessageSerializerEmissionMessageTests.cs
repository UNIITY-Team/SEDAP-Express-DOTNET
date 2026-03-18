using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerEmissionMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerEmissionMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        // Fields after common header: EmissionId;DeleteMode;SensorLat;SensorLon;SensorAlt;EmitterLat;EmitterLon;EmitterAlt;Bearing;Freqs;BW;Power;FreqAgility;PrfAgility;Function;Spot;Sidc;Comment
        var input = "EMISSION;18;661D64C0;129E;U;;;EM001;FALSE;48.5;11.2;;;;;90.5;100.5#200.3";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<EmissionMessage>(actualBase);
        var expected = new EmissionMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            EmissionId: "EM001",
            DeleteMode: DeleteMode.False,
            SensorLatitude: 48.5,
            SensorLongitude: 11.2,
            SensorAltitude: null,
            EmitterLatitude: null,
            EmitterLongitude: null,
            EmitterAltitude: null,
            Bearing: 90.5,
            Frequencies: [100.5, 200.3],
            Bandwidth: null,
            Power: null,
            FreqAgility: null,
            PrfAgility: null,
            Function: null,
            SpotNumber: null,
            Sidc: null,
            Comment: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new EmissionMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            EmissionId: "EM001",
            DeleteMode: DeleteMode.False,
            SensorLatitude: 48.5,
            SensorLongitude: 11.2,
            SensorAltitude: null,
            EmitterLatitude: null,
            EmitterLongitude: null,
            EmitterAltitude: null,
            Bearing: 90.5,
            Frequencies: [100.5, 200.3],
            Bandwidth: null,
            Power: null,
            FreqAgility: null,
            PrfAgility: null,
            Function: null,
            SpotNumber: null,
            Sidc: null,
            Comment: null
        );
        var actual = _sut.Serialize(message);
        var expected = "EMISSION;18;661D64C0;129E;U;;;EM001;FALSE;48.5;11.2;;;;;90.5;100.5#200.3";
        Assert.Equal(expected, actual);
    }
}
