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
            CommandType.Restart => 0x01,
            CommandType.Standby => 0x02,
            CommandType.SyncTime => 0x03,
            CommandType.SendStatus => 0x04,
            CommandType.Move => 0x05,
            CommandType.Rotate => 0x06,
            CommandType.Loiter => 0x07,
            CommandType.ScanArea => 0x08,
            CommandType.TakePhoto => 0x09,
            CommandType.MakeVideo => 0x0A,
            CommandType.LiveVideo => 0x0B,
            CommandType.Engagement => 0x0C,
            CommandType.Sanitize => 0xEE,
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
            case 0x01: result = CommandType.Restart; return true;
            case 0x02: result = CommandType.Standby; return true;
            case 0x03: result = CommandType.SyncTime; return true;
            case 0x04: result = CommandType.SendStatus; return true;
            case 0x05: result = CommandType.Move; return true;
            case 0x06: result = CommandType.Rotate; return true;
            case 0x07: result = CommandType.Loiter; return true;
            case 0x08: result = CommandType.ScanArea; return true;
            case 0x09: result = CommandType.TakePhoto; return true;
            case 0x0A: result = CommandType.MakeVideo; return true;
            case 0x0B: result = CommandType.LiveVideo; return true;
            case 0x0C: result = CommandType.Engagement; return true;
            case 0xEE: result = CommandType.Sanitize; return true;
            case 0xFF: result = CommandType.GenericAction; return true;
            default: result = default; return false;
        }
    }
}
