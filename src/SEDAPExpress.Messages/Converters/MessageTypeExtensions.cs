using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Wire-format helpers for <see cref="MessageType"/>.
/// </summary>
public static class MessageTypeExtensions
{
    /// <summary>
    /// Converts this value to its uppercase wire string.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire string.</returns>
    public static string ToWireString(this MessageType value) =>
        value switch
        {
            MessageType.Acknowledge => "ACKNOWLEDGE",
            MessageType.Command => "COMMAND",
            MessageType.Contact => "CONTACT",
            MessageType.Point => "POINT",
            MessageType.Emission => "EMISSION",
            MessageType.Generic => "GENERIC",
            MessageType.Graphic => "GRAPHIC",
            MessageType.Heartbeat => "HEARTBEAT",
            MessageType.Keyexchange => "KEYEXCHANGE",
            MessageType.Meteo => "METEO",
            MessageType.OwnUnit => "OWNUNIT",
            MessageType.Resend => "RESEND",
            MessageType.Status => "STATUS",
            MessageType.Text => "TEXT",
            MessageType.Timesync => "TIMESYNC",
            _ => value.ToString().ToUpperInvariant(),
        };

    /// <summary>
    /// Tries to parse a <see cref="MessageType"/> from a wire string.
    /// </summary>
    /// <param name="input">Input.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/> if parsing succeeded.</returns>
    public static bool TryFromWireString(ReadOnlySpan<char> input, out MessageType result)
    {
        if (MemoryExtensions.Equals(input, "ACKNOWLEDGE", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Acknowledge;
            return true;
        }
        if (MemoryExtensions.Equals(input, "COMMAND", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Command;
            return true;
        }
        if (MemoryExtensions.Equals(input, "CONTACT", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Contact;
            return true;
        }
        if (MemoryExtensions.Equals(input, "POINT", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Point;
            return true;
        }
        if (MemoryExtensions.Equals(input, "EMISSION", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Emission;
            return true;
        }
        if (MemoryExtensions.Equals(input, "GENERIC", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Generic;
            return true;
        }
        if (MemoryExtensions.Equals(input, "GRAPHIC", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Graphic;
            return true;
        }
        if (MemoryExtensions.Equals(input, "HEARTBEAT", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Heartbeat;
            return true;
        }
        if (MemoryExtensions.Equals(input, "KEYEXCHANGE", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Keyexchange;
            return true;
        }
        if (MemoryExtensions.Equals(input, "METEO", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Meteo;
            return true;
        }
        if (MemoryExtensions.Equals(input, "OWNUNIT", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.OwnUnit;
            return true;
        }
        if (MemoryExtensions.Equals(input, "RESEND", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Resend;
            return true;
        }
        if (MemoryExtensions.Equals(input, "STATUS", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Status;
            return true;
        }
        if (MemoryExtensions.Equals(input, "TEXT", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Text;
            return true;
        }
        if (MemoryExtensions.Equals(input, "TIMESYNC", StringComparison.OrdinalIgnoreCase))
        {
            result = MessageType.Timesync;
            return true;
        }
        result = default;
        return false;
    }
}
