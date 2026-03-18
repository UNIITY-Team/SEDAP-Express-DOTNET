using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Wire-format helpers for <see cref="DataEncoding"/>.
/// </summary>
public static class DataEncodingExtensions
{
    /// <summary>
    /// Converts this value to its wire string (<c>"BASE64"</c> or <c>"NONE"</c>).
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire string.</returns>
    public static string ToWireString(this DataEncoding value) =>
        value == DataEncoding.Base64 ? "BASE64" : "NONE";

    /// <summary>
    /// Tries to parse a <see cref="DataEncoding"/> from a wire string.
    /// </summary>
    /// <param name="input">Input.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/> if parsing succeeded.</returns>
    public static bool TryFromWireString(ReadOnlySpan<char> input, out DataEncoding result)
    {
        if (MemoryExtensions.Equals(input, "BASE64", StringComparison.OrdinalIgnoreCase))
        {
            result = DataEncoding.Base64;
            return true;
        }
        if (MemoryExtensions.Equals(input, "NONE", StringComparison.OrdinalIgnoreCase))
        {
            result = DataEncoding.None;
            return true;
        }
        result = default;
        return false;
    }
}
