using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerMeteoMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerMeteoMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        // Fields after common header (offset=7): SpeedThroughWater(7);WaterSpeed(8);WaterDir(9);WaterTemp(10);WaterDepth(11);AirTemp(12);DewPoint(13);Humidity(14);Pressure(15);...
        // Common header ends at field 6 with trailing ";", then 5 nulls (fields 7-11), AirTemp at 12
        var input = "METEO;18;661D64C0;129E;U;;;;;;;;22.5;;75;1013.25";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<MeteoMessage>(actualBase);
        var expected = new MeteoMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            SpeedThroughWater: null,
            WaterSpeed: null,
            WaterDirection: null,
            WaterTemperature: null,
            WaterDepth: null,
            AirTemperature: 22.5,
            DewPoint: null,
            HumidityRel: 75.0,
            Pressure: 1013.25,
            WindSpeed: null,
            WindDirection: null,
            Visibility: null,
            CloudHeight: null,
            CloudCover: null,
            Reference: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new MeteoMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            SpeedThroughWater: null,
            WaterSpeed: null,
            WaterDirection: null,
            WaterTemperature: null,
            WaterDepth: null,
            AirTemperature: 22.5,
            DewPoint: null,
            HumidityRel: 75.0,
            Pressure: 1013.25,
            WindSpeed: null,
            WindDirection: null,
            Visibility: null,
            CloudHeight: null,
            CloudCover: null,
            Reference: null
        );
        var actual = _sut.Serialize(message);
        var expected = "METEO;18;661D64C0;129E;U;;;;;;;;22.5;;75;1013.25";
        Assert.Equal(expected, actual);
    }
}
