using System.Collections;

namespace Bundeswehr.Uniity.SEDAPExpress.TestExtensions;

/// <summary>
/// Base class for xunit ClassData test data providers.
/// </summary>
public abstract class TestDataProvider<T> : IEnumerable<object[]>
{
    private Dictionary<string, T>? _data;

    private Dictionary<string, T> Data =>
        _data ??= Fill().ToDictionary(
            kvp => kvp.Key.Value,
            kvp => kvp.Value,
            StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Returns the test case identified by <paramref name="key"/>.
    /// </summary>
    public T Get(TestKey key)
    {
        ArgumentNullException.ThrowIfNull(key);
        return Data[key.Value];
    }

    /// <summary>
    /// Returns all test cases. Called once on first access.
    /// </summary>
    protected abstract IReadOnlyCollection<KeyValuePair<TestKey, T>> Fill();

    /// <inheritdoc/>
    public IEnumerator<object[]> GetEnumerator() =>
        Data.Keys
            .Select(k => new object[] { new TestKey(k) })
            .GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
