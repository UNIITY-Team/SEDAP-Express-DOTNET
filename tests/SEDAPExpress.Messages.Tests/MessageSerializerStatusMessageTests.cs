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
    public void MessageCanBeDeserialized()
    {
        // Fields after common header: TecState;OpsState;AmmoLevels;FuelLevels;BattLevels;CmdId;CmdState;Hostname;MediaUrls;FreeText
        var input = "STATUS;18;661D64C0;129E;U;;;2;3";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<StatusMessage>(actualBase);
        var expected = new StatusMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            TecState: TechnicalState.Operational,
            OpsState: OperationalState.OperationalSemiAutonomous,
            AmmunitionLevelNames: null,
            AmmunitionLevels: null,
            FuelLevelNames: null,
            FuelLevels: null,
            BatterieLevelNames: null,
            BatterieLevels: null,
            CmdId: null,
            CmdState: null,
            Hostname: null,
            MediaUrls: null,
            FreeText: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new StatusMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            TecState: TechnicalState.Operational,
            OpsState: OperationalState.OperationalSemiAutonomous,
            AmmunitionLevelNames: null,
            AmmunitionLevels: null,
            FuelLevelNames: null,
            FuelLevels: null,
            BatterieLevelNames: null,
            BatterieLevels: null,
            CmdId: null,
            CmdState: null,
            Hostname: null,
            MediaUrls: null,
            FreeText: null
        );
        var actual = _sut.Serialize(message);
        var expected = "STATUS;18;661D64C0;129E;U;;;2;3";
        Assert.Equal(expected, actual);
    }
}
