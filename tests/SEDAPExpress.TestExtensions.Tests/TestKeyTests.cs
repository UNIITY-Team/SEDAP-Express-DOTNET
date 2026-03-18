namespace Bundeswehr.Uniity.SEDAPExpress.TestExtensions.Tests;

public sealed class TestKeyTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(" Leading")]
    [InlineData("Trailing ")]
    [InlineData("123456789012345678901234567890123456789012345678901")] // 51 chars
    public void ConstructorThrowsForInvalidValue(string value)
    {
        Assert.Throws<ArgumentException>(() => new TestKey(value!));
    }

    [Fact]
    public void ConstructorThrowsArgumentNullForNullValueInvalidValue()
    {
        Assert.Throws<ArgumentNullException>(() => new TestKey(null!));
    }

    [Fact]
    public void ConstructorAcceptsMaxLengthValue()
    {
        string value = new string('A', 50);
        TestKey key = new TestKey(value);
        Assert.Equal(value, key.Value);
    }

    [Fact]
    public void ConstructorAcceptsSingleCharValue()
    {
        TestKey key = new TestKey("A");
        Assert.Equal("A", key.Value);
    }

    [Fact]
    public void ToStringReturnsValue()
    {
        TestKey key = new TestKey("MyKey");
        Assert.Equal("MyKey", key.ToString());
    }

    [Fact]
    public void EqualityHoldsForSameValue()
    {
        TestKey a = new TestKey("SameKey");
        TestKey b = new TestKey("SameKey");
        Assert.Equal(a, b);
    }

    [Fact]
    public void EqualityFailsForDifferentValues()
    {
        TestKey a = new TestKey("KeyOne");
        TestKey b = new TestKey("KeyTwo");
        Assert.NotEqual(a, b);
    }
}
