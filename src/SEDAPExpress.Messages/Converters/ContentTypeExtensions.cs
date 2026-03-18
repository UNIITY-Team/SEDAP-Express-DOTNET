using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Wire-format helpers for <see cref="ContentType"/>.
/// </summary>
public static class ContentTypeExtensions
{
    /// <summary>
    /// Converts this value to its wire string.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire string.</returns>
    public static string ToWireString(this ContentType value) =>
        value switch
        {
            ContentType.Sedap => "SEDAP",
            ContentType.Ascii => "ASCII",
            ContentType.Nmea => "NMEA",
            ContentType.Xml => "XML",
            ContentType.Json => "JSON",
            ContentType.Binary => "BINARY",
            _ => value.ToString().ToUpperInvariant(),
        };

    /// <summary>
    /// Tries to parse a <see cref="ContentType"/> from a wire string.
    /// </summary>
    /// <param name="input">Input.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/> if parsing succeeded.</returns>
    public static bool TryFromWireString(ReadOnlySpan<char> input, out ContentType result)
    {
        if (MemoryExtensions.Equals(input, "SEDAP", StringComparison.OrdinalIgnoreCase))
        {
            result = ContentType.Sedap;
            return true;
        }
        if (MemoryExtensions.Equals(input, "ASCII", StringComparison.OrdinalIgnoreCase))
        {
            result = ContentType.Ascii;
            return true;
        }
        if (MemoryExtensions.Equals(input, "NMEA", StringComparison.OrdinalIgnoreCase))
        {
            result = ContentType.Nmea;
            return true;
        }
        if (MemoryExtensions.Equals(input, "XML", StringComparison.OrdinalIgnoreCase))
        {
            result = ContentType.Xml;
            return true;
        }
        if (MemoryExtensions.Equals(input, "JSON", StringComparison.OrdinalIgnoreCase))
        {
            result = ContentType.Json;
            return true;
        }
        if (MemoryExtensions.Equals(input, "BINARY", StringComparison.OrdinalIgnoreCase))
        {
            result = ContentType.Binary;
            return true;
        }
        result = default;
        return false;
    }
}
