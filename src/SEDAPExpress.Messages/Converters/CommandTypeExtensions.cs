using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Wire-format helpers for <see cref="CommandType"/>.
/// </summary>
public static class CommandTypeExtensions
{
    /// <summary>
    /// Converts this value to its wire byte.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire byte.</returns>
    public static byte ToWireByte(this CommandType value) =>
        value switch
        {
            CommandType.Poweroff => 0x00,
            CommandType.Reboot => 0x01,
            CommandType.Shutdown => 0x02,
            CommandType.Start => 0x03,
            CommandType.Stop => 0x04,
            CommandType.Pause => 0x05,
            CommandType.Resume => 0x06,
            CommandType.Reset => 0x07,
            CommandType.Configure => 0x08,
            CommandType.Update => 0x09,
            CommandType.GenericAction => 0xFF,
            _ => 0x00,
        };

    /// <summary>
    /// Tries to parse a <see cref="CommandType"/> from a wire byte.
    /// </summary>
    /// <param name="input">Input byte.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/> if parsing succeeded.</returns>
    public static bool TryFromWireByte(byte input, out CommandType result)
    {
        switch (input)
        {
            case 0x00: result = CommandType.Poweroff; return true;
            case 0x01: result = CommandType.Reboot; return true;
            case 0x02: result = CommandType.Shutdown; return true;
            case 0x03: result = CommandType.Start; return true;
            case 0x04: result = CommandType.Stop; return true;
            case 0x05: result = CommandType.Pause; return true;
            case 0x06: result = CommandType.Resume; return true;
            case 0x07: result = CommandType.Reset; return true;
            case 0x08: result = CommandType.Configure; return true;
            case 0x09: result = CommandType.Update; return true;
            case 0xFF: result = CommandType.GenericAction; return true;
            default: result = default; return false;
        }
    }
}
