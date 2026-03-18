using System.Numerics;
using Microsoft.Extensions.Logging;
using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using FakeItEasy;
using Xunit;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class MessageSerializerKeyexchangeMessageTests
{
    private readonly MessageSerializer _sut;
    private readonly ILogger<MessageSerializer> _fakeLogger;

    public MessageSerializerKeyexchangeMessageTests()
    {
        _fakeLogger = A.Fake<ILogger<MessageSerializer>>();
        _sut = new MessageSerializer(_fakeLogger);
    }

    [Fact]
    public void MessageCanBeDeserialized()
    {
        // Fields: Recipient;AlgorithmType;Phase;KeyLenSecret;KeyLenDhKem;PrimeNumber;NaturalNumber;Iv;PublicKey
        var input = "KEYEXCHANGE;18;661D64C0;129E;U;;;LASSY;0;1;256;2048;DEADBEEF;CAFEBABE;0000000066AABBCC";
        var actualBase = _sut.Deserialize(input);
        var actual = Assert.IsType<KeyexchangeMessage>(actualBase);
        var expected = new KeyexchangeMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY",
            AlgorithmType: AlgorithmType.DiffieHellmanMerkle,
            Phase: 1,
            KeyLengthSharedSecret: 256,
            KeyLengthDhKem: 2048,
            PrimeNumber: new BigInteger(0xDEADBEEF),
            NaturalNumber: new BigInteger(0xCAFEBABE),
            Iv: 0x0000000066AABBCCL,
            PublicKey: null
        );
        Assert.Equivalent(expected, actual);
    }

    [Fact]
    public void MessageCanBeSerialized()
    {
        var message = new KeyexchangeMessage(
            Number: 0x18,
            Time: 0x661D64C0,
            Sender: "129E",
            Classification: Classification.Unclas,
            Acknowledgement: Acknowledgement.False,
            Mac: null,
            Recipient: "LASSY",
            AlgorithmType: AlgorithmType.DiffieHellmanMerkle,
            Phase: 1,
            KeyLengthSharedSecret: 256,
            KeyLengthDhKem: 2048,
            PrimeNumber: new BigInteger(0xDEADBEEF),
            NaturalNumber: new BigInteger(0xCAFEBABE),
            Iv: 0x0000000066AABBCCL,
            PublicKey: null
        );
        var actual = _sut.Serialize(message);
        var expected = "KEYEXCHANGE;18;661D64C0;129E;U;;;LASSY;0;1;256;2048;DEADBEEF;CAFEBABE;0000000066AABBCC";
        Assert.Equal(expected, actual);
    }
}
