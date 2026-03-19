using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Wire-format helpers for <see cref="DeleteFlagType"/>.
/// </summary>
public static class DeleteFlagTypeExtensions
{
    /// <summary>
    /// Converts this value to its wire string (<c>"TRUE"</c> or <c>"FALSE"</c>).
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire string.</returns>
    public static string ToWireString(this DeleteFlagType value) =>
        value == DeleteFlagType.True ? "TRUE" : "FALSE";

    /// <summary>
    /// Tries to parse a <see cref="DeleteFlagType"/> from a wire string.
    /// </summary>
    /// <param name="input">Input.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/> if parsing succeeded.</returns>
    public static bool TryFromWireString(ReadOnlySpan<char> input, out DeleteFlagType result)
    {
        if (MemoryExtensions.Equals(input, "TRUE", StringComparison.OrdinalIgnoreCase))
        {
            result = DeleteFlagType.True;
            return true;
        }
        // Empty string or "FALSE" both map to False (mirrors Java behaviour where an
        // unparseable value is silently coerced to FALSE).
        result = DeleteFlagType.False;
        return input.IsEmpty || MemoryExtensions.Equals(input, "FALSE", StringComparison.OrdinalIgnoreCase);
    }
}
