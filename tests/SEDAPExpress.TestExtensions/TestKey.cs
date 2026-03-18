namespace Bundeswehr.Uniity.SEDAPExpress.TestExtensions;

/// <summary>
/// Identifies a test case in a <see cref="TestDataProvider{T}"/>.
/// </summary>
public sealed record TestKey
{
    /// <summary>
    /// Gets the key string.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new <see cref="TestKey"/>.
    /// </summary>
    public TestKey(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        if (value.Length > 50)
        {
            throw new ArgumentException("Key must not exceed 50 characters.", nameof(value));
        }

        if (value != value.Trim())
        {
            throw new ArgumentException("Key must not have leading or trailing whitespace.", nameof(value));
        }

        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString() => Value;
}
