using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Wire-format helpers for <see cref="Acknowledgement"/>.
/// </summary>
public static class AcknowledgementExtensions
{
    /// <summary>
    /// Converts this value to its wire string (<c>"TRUE"</c> or empty string).
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire string.</returns>
    public static string ToWireString(this Acknowledgement value) =>
        value == Acknowledgement.True ? "TRUE" : string.Empty;

    /// <summary>
    /// Parses an <see cref="Acknowledgement"/> from a wire string.
    /// </summary>
    /// <param name="input">Input.</param>
    /// <returns>The parsed value.</returns>
    public static Acknowledgement FromWireString(ReadOnlySpan<char> input) =>
        MemoryExtensions.Equals(input, "TRUE", StringComparison.OrdinalIgnoreCase)
            ? Acknowledgement.True
            : Acknowledgement.False;
}
