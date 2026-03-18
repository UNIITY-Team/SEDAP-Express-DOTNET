namespace Bundeswehr.Uniity.SEDAPExpress.TestExtensions.Tests;

public sealed class TestDataProviderTests
{
    private sealed class StringProvider : TestDataProvider<string>
    {
        protected override IReadOnlyCollection<KeyValuePair<TestKey, string>> Fill() =>
        [
            new(new TestKey("Alpha"), "first"),
            new(new TestKey("Beta"),  "second"),
        ];
    }

    private sealed class EmptyProvider : TestDataProvider<string>
    {
        protected override IReadOnlyCollection<KeyValuePair<TestKey, string>> Fill() => [];
    }

    [Fact]
    public void GetReturnsCorrectValueForKnownKey()
    {
        StringProvider provider = new StringProvider();
        Assert.Equal("first", provider.Get(new TestKey("Alpha")));
    }

    [Fact]
    public void GetIsCaseInsensitive()
    {
        StringProvider provider = new StringProvider();
        Assert.Equal("first", provider.Get(new TestKey("alpha")));
    }

    [Fact]
    public void GetThrowsForNullKey()
    {
        StringProvider provider = new StringProvider();
        Assert.Throws<ArgumentNullException>(() => provider.Get(null!));
    }

    [Fact]
    public void GetThrowsForUnknownKey()
    {
        StringProvider provider = new StringProvider();
        Assert.Throws<KeyNotFoundException>(() => provider.Get(new TestKey("Unknown")));
    }

    [Fact]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "CA1861: Avoid constant arrays as arguments",
        Justification = "Prefer clarity over performance in tests.")]
    public void EnumerationReturnsAllKeys()
    {
        StringProvider provider = new StringProvider();
        List<object[]> items = provider.ToList();

        Assert.Equal(2, items.Count);
        foreach (object[] item in items)
        {
            Assert.Single(item);
            Assert.IsType<TestKey>(item[0]);
        }

        string first = provider.Get((TestKey)items[0][0]);
        string second = provider.Get((TestKey)items[1][0]);
        Assert.Contains(first, new[] { "first", "second" });
        Assert.Contains(second, new[] { "first", "second" });
        Assert.NotEqual(first, second);
    }

    [Fact]
    public void EnumerationOfEmptyProviderIsEmpty()
    {
        EmptyProvider provider = new EmptyProvider();
        Assert.Empty(provider);
    }
}
