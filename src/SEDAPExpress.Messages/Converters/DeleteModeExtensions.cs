using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Wire-format helpers for <see cref="DeleteMode"/>.
/// </summary>
public static class DeleteModeExtensions
{
    /// <summary>
    /// Converts this value to its wire string (<c>"TRUE"</c> or <c>"FALSE"</c>).
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire string.</returns>
    public static string ToWireString(this DeleteMode value) =>
        value == DeleteMode.True ? "TRUE" : "FALSE";

    /// <summary>
    /// Tries to parse a <see cref="DeleteMode"/> from a wire string.
    /// </summary>
    /// <param name="input">Input.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/> if parsing succeeded.</returns>
    public static bool TryFromWireString(ReadOnlySpan<char> input, out DeleteMode result)
    {
        if (MemoryExtensions.Equals(input, "TRUE", StringComparison.OrdinalIgnoreCase))
        {
            result = DeleteMode.True;
            return true;
        }
        if (MemoryExtensions.Equals(input, "FALSE", StringComparison.OrdinalIgnoreCase))
        {
            result = DeleteMode.False;
            return true;
        }
        result = default;
        return false;
    }
}
