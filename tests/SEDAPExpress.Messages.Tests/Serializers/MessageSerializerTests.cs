using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;
using Bundeswehr.Uniity.SEDAPExpress.TestExtensions;
using Xunit.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests.Serializers;

public sealed class MessageSerializerTests
{
    private static readonly MessageSerializerDeserializeData DeserializeData = new();
    private static readonly MessageSerializerSerializeData SerializeData = new();
    private static readonly MessageSerializerRejectData RejectData = new();

    private readonly MessageSerializer _sut;

    public MessageSerializerTests(ITestOutputHelper output)
    {
        _sut = new MessageSerializer(new TestOutputLogger<MessageSerializer>(output));
    }

    [Theory]
    [ClassData(typeof(MessageSerializerDeserializeData))]
    public void MessageCanBeDeserialized(TestKey key)
    {
        (string wireFormat, ISedapExpressMessage expected) = DeserializeData.Get(key);
        ISedapExpressMessage actual = _sut.Deserialize(wireFormat);
        Assert.Equivalent(expected, actual);
    }

    [Theory]
    [ClassData(typeof(MessageSerializerSerializeData))]
    public void MessageCanBeSerialized(TestKey key)
    {
        (string wireFormat, ISedapExpressMessage message) = SerializeData.Get(key);
        string actual = _sut.Serialize(message);
        Assert.Equal(wireFormat, actual);
    }

    [Theory]
    [ClassData(typeof(MessageSerializerRejectData))]
    public void MessageCannotBeDeserialized(TestKey key)
    {
        string wireFormat = RejectData.Get(key);
        Assert.False(_sut.TryDeserialize(wireFormat, out ISedapExpressMessage? _));
        Assert.Throws<MessageParseException>(() => _sut.Deserialize(wireFormat));
    }
}
