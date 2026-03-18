using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerCommandMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerCommandMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageWithParametersCanBeDeserialized()
    {
        var input = "COMMAND;55;1B351C87;5BCD;S;TRUE;4389F10D;7D31;1221;01;0C;hold-engagement;1000";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<CommandMessage>(actualBase);
        var expected = new CommandMessage(
            Number: 0x55,
            Time: 0x1B351C87,
            Sender: "5BCD",
            Classification: Classification.Secret,
            Acknowledgement: Acknowledgement.True,
            Mac: "4389F10D",
            Recipient: "7D31",
            CmdId: 0x1221,
            CmdFlag: CommandMode.Replace,
            CmdType: CommandType.Engagement,
            CmdTypeDependentParameters: ["hold-engagement", "1000"]
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageWithParametersCanBeSerialized()
    {
        var message = new CommandMessage(
            Number: 0x55,
            Time: 0x1B351C87,
            Sender: "5BCD",
            Classification: Classification.Secret,
            Acknowledgement: Acknowledgement.True,
            Mac: "4389F10D",
            Recipient: "7D31",
            CmdId: 0x1221,
            CmdFlag: CommandMode.Replace,
            CmdType: CommandType.Engagement,
            CmdTypeDependentParameters: ["hold-engagement", "1000"]
        );
        var actual = _sut.Serialize(message);
        var expected = "COMMAND;55;1B351C87;5BCD;S;TRUE;4389F10D;7D31;1221;01;0C;hold-engagement;1000";
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void MessageWithNullCmdIdCanBeDeserialized()
    {
        var input = "COMMAND;29;661D44C0;E4B3;C;TRUE;;Drone1;;00;FF;OPEN_BAY";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<CommandMessage>(actualBase);
        var expected = new CommandMessage(
            Number: 0x29,
            Time: 0x661D44C0,
            Sender: "E4B3",
            Classification: Classification.Confidential,
            Acknowledgement: Acknowledgement.True,
            Mac: null,
            Recipient: "Drone1",
            CmdId: null,
            CmdFlag: CommandMode.Add,
            CmdType: CommandType.GenericAction,
            CmdTypeDependentParameters: ["OPEN_BAY"]
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageWithNullCmdIdCanBeSerialized()
    {
        var message = new CommandMessage(
            Number: 0x29,
            Time: 0x661D44C0,
            Sender: "E4B3",
            Classification: Classification.Confidential,
            Acknowledgement: Acknowledgement.True,
            Mac: null,
            Recipient: "Drone1",
            CmdId: null,
            CmdFlag: CommandMode.Add,
            CmdType: CommandType.GenericAction,
            CmdTypeDependentParameters: ["OPEN_BAY"]
        );
        var actual = _sut.Serialize(message);
        var expected = "COMMAND;29;661D44C0;E4B3;C;TRUE;;Drone1;;00;FF;OPEN_BAY";
        Assert.Equal(expected, actual);
    }
}
