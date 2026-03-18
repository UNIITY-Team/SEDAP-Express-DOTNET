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
    public void MessageCanBeDeserialized()
    {
        var input = "COMMAND;18;661D64C0;129E;U;;;LASSY;0001;00;FF";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<CommandMessage>(actualBase);
        var expected = new CommandMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY",
            CmdId: 0x0001,
            CmdFlag: CommandMode.Add,
            CmdType: CommandType.GenericAction,
            CmdTypeDependentParameters: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new CommandMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY",
            CmdId: 0x0001,
            CmdFlag: CommandMode.Add,
            CmdType: CommandType.GenericAction,
            CmdTypeDependentParameters: null
        );
        var actual = _sut.Serialize(message);
        var expected = "COMMAND;18;661D64C0;129E;U;;;LASSY;0001;00;FF";
        Assert.Equal(expected, actual);
    }
}
