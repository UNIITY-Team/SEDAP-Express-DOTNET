using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerStatusMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerStatusMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageWithResourceLevelsAndMediaUrlsCanBeDeserialized()
    {
        // Fields after common header: TecState;OpsState;AmmoLevels;FuelLevels;BattLevels;CmdId;CmdState;Hostname;MediaUrls;FreeText
        var input = "STATUS;41;50505050;BB91;C;TRUE;93B37ACC;2;1;#20.3;#30.4;#40.5;;;MTAuOC4wLjY=;cnRzcDovLzEwLjguMC42L3N0cmVhbTE=;U2FtcGxlVGV4dCE=";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<StatusMessage>(actualBase);
        var expected = new StatusMessage(
            Number: 0x41,
            Time: 0x50505050,
            Sender: "BB91",
            Classification: Classification.Confidential,
            Acknowledgement: Acknowledgement.True,
            Mac: "93B37ACC",
            TecState: TechnicalState.Degraded,
            OpsState: OperationalState.Degraded,
            AmmunitionLevelNames: [""],
            AmmunitionLevels: [20.3],
            FuelLevelNames: [""],
            FuelLevels: [30.4],
            BatterieLevelNames: [""],
            BatterieLevels: [40.5],
            CmdId: null,
            CmdState: null,
            Hostname: "10.8.0.6",
            MediaUrls: ["rtsp://10.8.0.6/stream1"],
            FreeText: "SampleText!"
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageWithResourceLevelsAndMediaUrlsCanBeSerialized()
    {
        var message = new StatusMessage(
            Number: 0x41,
            Time: 0x50505050,
            Sender: "BB91",
            Classification: Classification.Confidential,
            Acknowledgement: Acknowledgement.True,
            Mac: "93B37ACC",
            TecState: TechnicalState.Degraded,
            OpsState: OperationalState.Degraded,
            AmmunitionLevelNames: [""],
            AmmunitionLevels: [20.3],
            FuelLevelNames: [""],
            FuelLevels: [30.4],
            BatterieLevelNames: [""],
            BatterieLevels: [40.5],
            CmdId: null,
            CmdState: null,
            Hostname: "10.8.0.6",
            MediaUrls: ["rtsp://10.8.0.6/stream1"],
            FreeText: "SampleText!"
        );
        var actual = _sut.Serialize(message);
        var expected = "STATUS;41;50505050;BB91;C;TRUE;93B37ACC;2;1;#20.3;#30.4;#40.5;;;MTAuOC4wLjY=;cnRzcDovLzEwLjguMC42L3N0cmVhbTE=;U2FtcGxlVGV4dCE=";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MessageWithBatteryLevelAndLongMediaUrlCanBeDeserialized()
    {
        var input = "STATUS;15;66e2d520;LASSY;C;;;3;2;;;MainBattery#100;;;MTkyLjE2OC4xNjguMTA1;aHR0cDovLzE5Mi4xNjguMTY4LjEwNTo4MDgwL3N0cmVhbT90b3BpYz0vYXJndXMvYXIwMjM0X2Zyb250X2xlZnQvaW1hZ2VfcmF3";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<StatusMessage>(actualBase);
        var expected = new StatusMessage(
            Number: 0x15,
            Time: 0x66e2d520,
            Sender: "LASSY",
            Classification: Classification.Confidential,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            TecState: TechnicalState.Operational,
            OpsState: OperationalState.Operational,
            AmmunitionLevelNames: null,
            AmmunitionLevels: null,
            FuelLevelNames: null,
            FuelLevels: null,
            BatterieLevelNames: ["MainBattery"],
            BatterieLevels: [100.0],
            CmdId: null,
            CmdState: null,
            Hostname: "192.168.168.105",
            MediaUrls: ["http://192.168.168.105:8080/stream?topic=/argus/ar0234_front_left/image_raw"],
            FreeText: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageWithBatteryLevelAndLongMediaUrlCanBeSerialized()
    {
        var message = new StatusMessage(
            Number: 0x15,
            Time: 0x66e2d520,
            Sender: "LASSY",
            Classification: Classification.Confidential,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            TecState: TechnicalState.Operational,
            OpsState: OperationalState.Operational,
            AmmunitionLevelNames: null,
            AmmunitionLevels: null,
            FuelLevelNames: null,
            FuelLevels: null,
            BatterieLevelNames: ["MainBattery"],
            BatterieLevels: [100.0],
            CmdId: null,
            CmdState: null,
            Hostname: "192.168.168.105",
            MediaUrls: ["http://192.168.168.105:8080/stream?topic=/argus/ar0234_front_left/image_raw"],
            FreeText: null
        );
        var actual = _sut.Serialize(message);
        var expected = "STATUS;15;66E2D520;LASSY;C;;;3;2;;;MainBattery#100;;;MTkyLjE2OC4xNjguMTA1;aHR0cDovLzE5Mi4xNjguMTY4LjEwNTo4MDgwL3N0cmVhbT90b3BpYz0vYXJndXMvYXIwMjM0X2Zyb250X2xlZnQvaW1hZ2VfcmF3";
        Assert.Equal(expected, actual);
    }
}
