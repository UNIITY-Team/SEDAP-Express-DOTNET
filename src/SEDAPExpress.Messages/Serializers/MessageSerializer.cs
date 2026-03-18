using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Utilities;
using Microsoft.Extensions.Logging;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Serializers;

/// <summary>
/// Standard message serializer.
/// </summary>
public sealed class MessageSerializer : IMessageSerializer
{
    private const char FieldSeparator = ';';
    private const char HashSeparator = '#';
    private const int CommonFieldCount = 7;

    private readonly ILogger<MessageSerializer> _logger;

    private readonly record struct CommonHeader(
        byte? Number,
        long? Time,
        string? Sender,
        Classification Classification,
        Acknowledgement Acknowledgement,
        string? Mac);

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="logger">Logger. If no logger is passed, logs go to the console.</param>
    public MessageSerializer(ILogger<MessageSerializer>? logger = null)
    {
        _logger = logger ?? new ConsoleLogger<MessageSerializer>();
    }

    /// <inheritdoc/>
    public ISedapExpressMessage Deserialize(string input)
    {
        if (!TryDeserialize(input, out ISedapExpressMessage? message))
        {
            throw new MessageParseException(
                $"Parsing message '{LogSafe(input)}' failed.");
        }

        return message;
    }

    /// <inheritdoc/>
    public bool TryDeserialize(
        ReadOnlySpan<char> input,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;
        string inputString = input.ToString();
        string[] fields = inputString.Split(FieldSeparator);
        Lazy<string> sanitized = new(() => LogSafe(inputString));

        if (fields.Length < CommonFieldCount)
        {
            Log.ParseFailureTooFewFields(_logger, fields.Length, sanitized.Value);
            return false;
        }

        if (!MessageTypeExtensions.TryFromWireString(fields[0].AsSpan(), out MessageType messageType))
        {
            Log.ParseFailureUnknownMessageType(_logger, "fields[0]", sanitized.Value);
            return false;
        }

        if (!TryParseCommonHeader(fields, 1, out CommonHeader header))
        {
            Log.ParseFailure(_logger, "Failed to parse common header", sanitized.Value);
            return false;
        }

        return messageType switch
        {
            MessageType.Acknowledge => TryParseAcknowledge(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Command => TryParseCommand(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Contact => TryParseContact(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Point => TryParsePoint(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Emission => TryParseEmission(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Generic => TryParseGeneric(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Graphic => TryParseGraphic(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Heartbeat => TryParseHeartbeat(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Keyexchange => TryParseKeyexchange(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Meteo => TryParseMeteo(fields, CommonFieldCount, header, _logger, out message),
            MessageType.OwnUnit => TryParseOwnUnit(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Resend => TryParseResend(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Status => TryParseStatus(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Text => TryParseText(fields, CommonFieldCount, header, _logger, out message),
            MessageType.Timesync => TryParseTimesync(fields, CommonFieldCount, header, _logger, out message),
            _ => false,
        };
    }

    /// <inheritdoc/>
    public string Serialize(ISedapExpressMessage message)
    {
        return message switch
        {
            AcknowledgeMessage m => SerializeAcknowledge(m),
            CommandMessage m => SerializeCommand(m),
            ContactMessage m => SerializeContact(m),
            PointMessage m => SerializePoint(m),
            EmissionMessage m => SerializeEmission(m),
            GenericMessage m => SerializeGeneric(m),
            GraphicMessage m => SerializeGraphic(m),
            HeartbeatMessage m => SerializeHeartbeat(m),
            KeyexchangeMessage m => SerializeKeyexchange(m),
            MeteoMessage m => SerializeMeteo(m),
            OwnUnitMessage m => SerializeOwnUnit(m),
            ResendMessage m => SerializeResend(m),
            StatusMessage m => SerializeStatus(m),
            TextMessage m => SerializeText(m),
            TimesyncMessage m => SerializeTimesync(m),
            _ => throw new ArgumentException($"Unknown message type: {message.GetType().Name}", nameof(message)),
        };
    }

    // -------------------------------------------------------------------------
    // Parsing helpers
    // -------------------------------------------------------------------------

    // offset: index of the Number field (fields[1] after message type at fields[0])
    private static bool TryParseCommonHeader(string[] fields, int offset, out CommonHeader header)
    {
        // Number (optional)
        byte? number = null;
        ReadOnlySpan<char> numberSpan = fields[offset].AsSpan();
        if (numberSpan.Length > 0)
        {
            if (!TryParseHexByte(numberSpan, out byte parsedNumber))
            {
                header = default;
                return false;
            }
            number = parsedNumber;
        }

        // Time (optional)
        long? time = null;
        ReadOnlySpan<char> timeSpan = fields[offset + 1].AsSpan();
        if (timeSpan.Length > 0)
        {
            if (!TryParseHexLong(timeSpan, out long parsedTime))
            {
                header = default;
                return false;
            }
            time = parsedTime;
        }

        string? sender = fields[offset + 2].Length > 0 ? fields[offset + 2] : null;

        ReadOnlySpan<char> classSpan = fields[offset + 3].AsSpan();
        Classification classification = classSpan.Length == 1
            && ClassificationExtensions.TryFromWireChar(classSpan[0], out Classification cls)
            ? cls
            : Classification.None;

        Acknowledgement acknowledgement = AcknowledgementExtensions.FromWireString(fields[offset + 4].AsSpan());
        string? mac = fields[offset + 5].Length > 0 ? fields[offset + 5] : null;

        header = new CommonHeader(number, time, sender, classification, acknowledgement, mac);
        return true;
    }

    private static bool TryParseHexByte(ReadOnlySpan<char> input, out byte result)
    {
        // Valid: 2-char hex, first digit 0-7 (range 00-7F)
        if (input.Length != 2)
        {
            result = 0;
            return false;
        }
        char high = char.ToUpperInvariant(input[0]);
        char low = char.ToUpperInvariant(input[1]);
        if (high is < '0' or > '7')
        {
            result = 0;
            return false;
        }
        if (!IsHexChar(low))
        {
            result = 0;
            return false;
        }
        result = (byte)((HexVal(high) << 4) | HexVal(low));
        return true;
    }

    private static bool TryParseFullHexByte(ReadOnlySpan<char> input, out byte result)
    {
        // Valid: exactly 2 hex chars, full range 00-FF
        if (input.Length != 2)
        {
            result = 0;
            return false;
        }
        if (!IsHexChar(input[0]) || !IsHexChar(input[1]))
        {
            result = 0;
            return false;
        }
        result = (byte)((HexVal(input[0]) << 4) | HexVal(input[1]));
        return true;
    }

    private static bool TryParseHexShort(ReadOnlySpan<char> input, out short result)
    {
        // Valid: exactly 4 hex chars
        if (input.Length != 4)
        {
            result = 0;
            return false;
        }
        foreach (char c in input)
        {
            if (!IsHexChar(c))
            {
                result = 0;
                return false;
            }
        }
        result = (short)int.Parse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return true;
    }

    private static bool TryParseHexLong(ReadOnlySpan<char> input, out long result)
    {
        // Valid: 8-16 hex digits
        if (input.Length is < 8 or > 16)
        {
            result = 0;
            return false;
        }
        foreach (char c in input)
        {
            if (!IsHexChar(c))
            {
                result = 0;
                return false;
            }
        }
        result = long.Parse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return true;
    }

    private static bool TryParseHexLong16(ReadOnlySpan<char> input, out long result)
    {
        // Valid: exactly 16 hex chars
        if (input.Length != 16)
        {
            result = 0;
            return false;
        }
        foreach (char c in input)
        {
            if (!IsHexChar(c))
            {
                result = 0;
                return false;
            }
        }
        result = long.Parse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return true;
    }

    private static bool TryParseDouble(ReadOnlySpan<char> input, out double result) =>
        double.TryParse(input, NumberStyles.Float | NumberStyles.AllowLeadingSign,
            CultureInfo.InvariantCulture, out result);

    private static bool TryParseDecimalInt(ReadOnlySpan<char> input, out int result) =>
        int.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out result);

    private static bool TryParseRgbaHex(ReadOnlySpan<char> input, out int result)
    {
        if (input.Length != 8)
        {
            result = 0;
            return false;
        }
        foreach (char c in input)
        {
            if (!IsHexChar(c))
            {
                result = 0;
                return false;
            }
        }
        result = (int)uint.Parse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        return true;
    }

    private static bool TryFromBase64String(string value, out string text)
    {
        try
        {
            byte[] bytes = Convert.FromBase64String(value);
            text = Encoding.UTF8.GetString(bytes);
            return true;
        }
        catch (FormatException)
        {
            text = string.Empty;
            return false;
        }
    }

    private static bool TryFromBase64Bytes(string value, out byte[] data)
    {
        try
        {
            data = Convert.FromBase64String(value);
            return true;
        }
        catch (FormatException)
        {
            data = [];
            return false;
        }
    }

    private static bool TryParseDoubleList(string value, out IReadOnlyList<double> result)
    {
        string[] parts = value.Split(HashSeparator);
        double[] values = new double[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            if (!TryParseDouble(parts[i].AsSpan(), out values[i]))
            {
                result = [];
                return false;
            }
        }
        result = values;
        return true;
    }

    private static bool TryParseResourceLevels(string value,
        out IReadOnlyList<string> names,
        out IReadOnlyList<double> levels)
    {
        string[] parts = value.Split(HashSeparator);
        if (parts.Length % 2 != 0)
        {
            names = [];
            levels = [];
            return false;
        }
        string[] nameList = new string[parts.Length / 2];
        double[] levelList = new double[parts.Length / 2];
        for (int i = 0; i < parts.Length; i += 2)
        {
            nameList[i / 2] = parts[i];
            if (!TryParseDouble(parts[i + 1].AsSpan(), out levelList[i / 2]))
            {
                names = [];
                levels = [];
                return false;
            }
        }
        names = nameList;
        levels = levelList;
        return true;
    }

    // -------------------------------------------------------------------------
    // Formatting helpers
    // -------------------------------------------------------------------------

    private static string FormatDouble(double value) =>
        value.ToString("##.############", CultureInfo.InvariantCulture);

    private static string FormatRgbaHex(int value) =>
        ((uint)value).ToString("X8", CultureInfo.InvariantCulture);

    private static string ToBase64(byte[] data) => Convert.ToBase64String(data);

    private static string ToBase64(string text) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

    private static string FormatDoubleList(IReadOnlyList<double> values) =>
        string.Join(HashSeparator, values.Select(FormatDouble));

    private static string FormatResourceLevels(IReadOnlyList<string> names, IReadOnlyList<double> levels)
    {
        StringBuilder sb = new();
        for (int i = 0; i < names.Count; i++)
        {
            if (i > 0)
            {
                sb.Append(HashSeparator);
            }
            sb.Append(names[i]);
            sb.Append(HashSeparator);
            sb.Append(FormatDouble(levels[i]));
        }
        return sb.ToString();
    }

    private static string FormatBigIntegerHex(BigInteger value)
    {
        string hex = value.ToString("X", CultureInfo.InvariantCulture).TrimStart('0');
        return hex.Length == 0 ? "0" : hex;
    }

    // -------------------------------------------------------------------------
    // Field access helpers
    // -------------------------------------------------------------------------

    private static string Field(string[] fields, int index) =>
        index < fields.Length ? fields[index] : string.Empty;

    private static bool HasField(string[] fields, int index) =>
        index < fields.Length && fields[index].Length > 0;

    // -------------------------------------------------------------------------
    // ACKNOWLEDGE
    // -------------------------------------------------------------------------

    private static bool TryParseAcknowledge(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        string? recipient = offset < fields.Length && fields[offset].Length > 0
            ? fields[offset]
            : null;

        if (offset + 1 >= fields.Length || fields[offset + 1].Length == 0)
        {
            Log.ParseFailureInvalidField(logger, "AckedMessageType", string.Empty);
            return false;
        }

        if (!MessageTypeExtensions.TryFromWireString(fields[offset + 1].AsSpan(), out MessageType ackedMessageType))
        {
            Log.ParseFailureInvalidField(logger, "AckedMessageType", fields[offset + 1]);
            return false;
        }

        if (offset + 2 >= fields.Length || fields[offset + 2].Length == 0)
        {
            Log.ParseFailureInvalidField(logger, "AckedMessageNumber", string.Empty);
            return false;
        }

        if (!TryParseHexByte(fields[offset + 2].AsSpan(), out byte ackedMessageNumber))
        {
            Log.ParseFailureInvalidField(logger, "AckedMessageNumber", fields[offset + 2]);
            return false;
        }

        message = new AcknowledgeMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            Recipient: recipient,
            AckedMessageType: ackedMessageType,
            AckedMessageNumber: ackedMessageNumber);
        return true;
    }

    private static string SerializeAcknowledge(AcknowledgeMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.Recipient ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.AckedMessageType.ToWireString());
        sb.Append(FieldSeparator);
        sb.Append(m.AckedMessageNumber.ToString("X2", CultureInfo.InvariantCulture));
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // COMMAND
    // -------------------------------------------------------------------------

    private static bool TryParseCommand(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        string? recipient = HasField(fields, offset) ? fields[offset] : null;

        // CmdId (optional hex short)
        short? cmdId = null;
        if (HasField(fields, offset + 1))
        {
            if (!TryParseHexShort(fields[offset + 1].AsSpan(), out short parsedCmdId))
            {
                Log.ParseFailureInvalidField(logger, "CmdId", fields[offset + 1]);
                return false;
            }
            cmdId = parsedCmdId;
        }

        // CmdFlag (mandatory hex byte)
        if (!HasField(fields, offset + 2))
        {
            Log.ParseFailureInvalidField(logger, "CmdFlag", string.Empty);
            return false;
        }
        if (!TryParseFullHexByte(fields[offset + 2].AsSpan(), out byte cmdFlagByte))
        {
            Log.ParseFailureInvalidField(logger, "CmdFlag", fields[offset + 2]);
            return false;
        }

        // CmdType (mandatory hex byte)
        if (!HasField(fields, offset + 3))
        {
            Log.ParseFailureInvalidField(logger, "CmdType", string.Empty);
            return false;
        }
        if (!TryParseFullHexByte(fields[offset + 3].AsSpan(), out byte cmdTypeByte))
        {
            Log.ParseFailureInvalidField(logger, "CmdType", fields[offset + 3]);
            return false;
        }
        if (!CommandTypeExtensions.TryFromWireByte(cmdTypeByte, out CommandType cmdType))
        {
            Log.ParseFailureInvalidField(logger, "CmdType", fields[offset + 3]);
            return false;
        }

        _ = IntEnumExtensions.TryFromWireInt(cmdFlagByte, out CommandMode cmdFlag);

        // Remaining parameters
        IReadOnlyList<string>? cmdTypeDependentParameters = null;
        if (offset + 4 < fields.Length)
        {
            List<string> remaining = [];
            for (int i = offset + 4; i < fields.Length; i++)
            {
                remaining.Add(fields[i]);
            }
            if (remaining.Count > 0)
            {
                cmdTypeDependentParameters = remaining;
            }
        }

        message = new CommandMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            Recipient: recipient,
            CmdId: cmdId,
            CmdFlag: cmdFlag,
            CmdType: cmdType,
            CmdTypeDependentParameters: cmdTypeDependentParameters);
        return true;
    }

    private static string SerializeCommand(CommandMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.Recipient ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.CmdId.HasValue
            ? m.CmdId.Value.ToString("X4", CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.CmdFlag.ToWireInt().ToString("X2", CultureInfo.InvariantCulture));
        sb.Append(FieldSeparator);
        sb.Append(m.CmdType.ToWireByte().ToString("X2", CultureInfo.InvariantCulture));
        if (m.CmdTypeDependentParameters is { Count: > 0 })
        {
            foreach (string param in m.CmdTypeDependentParameters)
            {
                sb.Append(FieldSeparator);
                sb.Append(param);
            }
        }
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // CONTACT
    // -------------------------------------------------------------------------

    private static bool TryParseContact(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        if (!HasField(fields, offset))
        {
            Log.ParseFailureInvalidField(logger, "ContactId", string.Empty);
            return false;
        }
        string contactId = fields[offset];

        string deleteModeField = Field(fields, offset + 1);
        if (!HasField(fields, offset + 1) || !DeleteModeExtensions.TryFromWireString(deleteModeField.AsSpan(), out DeleteMode deleteMode))
        {
            Log.ParseFailureInvalidField(logger, "DeleteMode", deleteModeField);
            return false;
        }

        double? latitude = ParseOptionalDouble(fields, offset + 2);
        double? longitude = ParseOptionalDouble(fields, offset + 3);
        double? altitude = ParseOptionalDouble(fields, offset + 4);
        double? relX = ParseOptionalDouble(fields, offset + 5);
        double? relY = ParseOptionalDouble(fields, offset + 6);
        double? relZ = ParseOptionalDouble(fields, offset + 7);
        double? speed = ParseOptionalDouble(fields, offset + 8);
        double? course = ParseOptionalDouble(fields, offset + 9);
        double? heading = ParseOptionalDouble(fields, offset + 10);
        double? roll = ParseOptionalDouble(fields, offset + 11);
        double? pitch = ParseOptionalDouble(fields, offset + 12);
        double? width = ParseOptionalDouble(fields, offset + 13);
        double? length = ParseOptionalDouble(fields, offset + 14);
        double? height = ParseOptionalDouble(fields, offset + 15);
        string? name = HasField(fields, offset + 16) ? fields[offset + 16] : null;

        // Source: concatenated chars
        IReadOnlySet<ContactSource>? source = null;
        if (HasField(fields, offset + 17))
        {
            HashSet<ContactSource> srcSet = [];
            foreach (char c in fields[offset + 17])
            {
                srcSet.Add(ContactSourceExtensions.FromWireChar(c));
            }
            source = srcSet;
        }

        string? sidc = HasField(fields, offset + 18) ? fields[offset + 18] : null;
        string? mmsi = HasField(fields, offset + 19) ? fields[offset + 19] : null;
        string? icao = HasField(fields, offset + 20) ? fields[offset + 20] : null;

        byte[]? multimediaData = null;
        if (HasField(fields, offset + 21))
        {
            TryFromBase64Bytes(fields[offset + 21], out multimediaData);
        }

        string? comment = null;
        if (HasField(fields, offset + 22))
        {
            TryFromBase64String(fields[offset + 22], out comment);
        }

        message = new ContactMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            ContactId: contactId,
            DeleteMode: deleteMode,
            Latitude: latitude,
            Longitude: longitude,
            Altitude: altitude,
            RelativeXDistance: relX,
            RelativeYDistance: relY,
            RelativeZDistance: relZ,
            Speed: speed,
            Course: course,
            Heading: heading,
            Roll: roll,
            Pitch: pitch,
            Width: width,
            Length: length,
            Height: height,
            Name: name,
            Source: source,
            Sidc: sidc,
            Mmsi: mmsi,
            Icao: icao,
            MultimediaData: multimediaData,
            Comment: comment);
        return true;
    }

    private static string SerializeContact(ContactMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.ContactId);
        sb.Append(FieldSeparator);
        sb.Append(m.DeleteMode.ToWireString());
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Latitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Longitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Altitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.RelativeXDistance);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.RelativeYDistance);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.RelativeZDistance);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Speed);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Course);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Heading);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Roll);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Pitch);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Width);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Length);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Height);
        sb.Append(FieldSeparator);
        sb.Append(m.Name ?? string.Empty);
        sb.Append(FieldSeparator);
        if (m.Source is { Count: > 0 })
        {
            foreach (ContactSource src in m.Source)
            {
                sb.Append(src.ToWireChar());
            }
        }
        sb.Append(FieldSeparator);
        sb.Append(m.Sidc ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Mmsi ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Icao ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.MultimediaData != null ? ToBase64(m.MultimediaData.ToArray()) : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Comment != null ? ToBase64(m.Comment) : string.Empty);
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // POINT
    // -------------------------------------------------------------------------

    private static bool TryParsePoint(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        if (!HasField(fields, offset))
        {
            Log.ParseFailureInvalidField(logger, "ContactId", string.Empty);
            return false;
        }
        string contactId = fields[offset];

        string deleteModeField = Field(fields, offset + 1);
        if (!HasField(fields, offset + 1) || !DeleteModeExtensions.TryFromWireString(deleteModeField.AsSpan(), out DeleteMode deleteMode))
        {
            Log.ParseFailureInvalidField(logger, "DeleteMode", deleteModeField);
            return false;
        }

        double? latitude = ParseOptionalDouble(fields, offset + 2);
        double? longitude = ParseOptionalDouble(fields, offset + 3);
        double? altitude = ParseOptionalDouble(fields, offset + 4);
        double? relX = ParseOptionalDouble(fields, offset + 5);
        double? relY = ParseOptionalDouble(fields, offset + 6);
        double? relZ = ParseOptionalDouble(fields, offset + 7);
        double? speed = ParseOptionalDouble(fields, offset + 8);
        double? course = ParseOptionalDouble(fields, offset + 9);
        double? heading = ParseOptionalDouble(fields, offset + 10);
        double? roll = ParseOptionalDouble(fields, offset + 11);
        double? pitch = ParseOptionalDouble(fields, offset + 12);
        double? width = ParseOptionalDouble(fields, offset + 13);
        double? length = ParseOptionalDouble(fields, offset + 14);
        double? height = ParseOptionalDouble(fields, offset + 15);
        string? name = HasField(fields, offset + 16) ? fields[offset + 16] : null;
        string? sidc = HasField(fields, offset + 17) ? fields[offset + 17] : null;

        byte[]? multimediaData = null;
        if (HasField(fields, offset + 18))
        {
            TryFromBase64Bytes(fields[offset + 18], out multimediaData);
        }

        string? comment = null;
        if (HasField(fields, offset + 19))
        {
            TryFromBase64String(fields[offset + 19], out comment);
        }

        message = new PointMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            ContactId: contactId,
            DeleteMode: deleteMode,
            Latitude: latitude,
            Longitude: longitude,
            Altitude: altitude,
            RelativeXDistance: relX,
            RelativeYDistance: relY,
            RelativeZDistance: relZ,
            Speed: speed,
            Course: course,
            Heading: heading,
            Roll: roll,
            Pitch: pitch,
            Width: width,
            Length: length,
            Height: height,
            Name: name,
            Sidc: sidc,
            MultimediaData: multimediaData,
            Comment: comment);
        return true;
    }

    private static string SerializePoint(PointMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.ContactId);
        sb.Append(FieldSeparator);
        sb.Append(m.DeleteMode.ToWireString());
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Latitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Longitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Altitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.RelativeXDistance);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.RelativeYDistance);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.RelativeZDistance);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Speed);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Course);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Heading);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Roll);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Pitch);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Width);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Length);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Height);
        sb.Append(FieldSeparator);
        sb.Append(m.Name ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Sidc ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.MultimediaData != null ? ToBase64(m.MultimediaData.ToArray()) : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Comment != null ? ToBase64(m.Comment) : string.Empty);
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // EMISSION
    // -------------------------------------------------------------------------

    private static bool TryParseEmission(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        if (!HasField(fields, offset))
        {
            Log.ParseFailureInvalidField(logger, "EmissionId", string.Empty);
            return false;
        }
        string emissionId = fields[offset];

        string deleteModeField = Field(fields, offset + 1);
        if (!HasField(fields, offset + 1) || !DeleteModeExtensions.TryFromWireString(deleteModeField.AsSpan(), out DeleteMode deleteMode))
        {
            Log.ParseFailureInvalidField(logger, "DeleteMode", deleteModeField);
            return false;
        }

        double? sensorLat = ParseOptionalDouble(fields, offset + 2);
        double? sensorLon = ParseOptionalDouble(fields, offset + 3);
        double? sensorAlt = ParseOptionalDouble(fields, offset + 4);
        double? emitterLat = ParseOptionalDouble(fields, offset + 5);
        double? emitterLon = ParseOptionalDouble(fields, offset + 6);
        double? emitterAlt = ParseOptionalDouble(fields, offset + 7);
        double? bearing = ParseOptionalDouble(fields, offset + 8);

        IReadOnlyList<double>? frequencies = null;
        if (HasField(fields, offset + 9))
        {
            _ = TryParseDoubleList(fields[offset + 9], out frequencies);
        }

        double? bandwidth = ParseOptionalDouble(fields, offset + 10);
        double? power = ParseOptionalDouble(fields, offset + 11);

        FreqAgility? freqAgility = null;
        if (HasField(fields, offset + 12) && TryParseDecimalInt(fields[offset + 12].AsSpan(), out int freqAgilityInt))
        {
            _ = IntEnumExtensions.TryFromWireInt(freqAgilityInt, out FreqAgility fa);
            freqAgility = fa;
        }

        PrfAgility? prfAgility = null;
        if (HasField(fields, offset + 13) && TryParseDecimalInt(fields[offset + 13].AsSpan(), out int prfAgilityInt))
        {
            _ = IntEnumExtensions.TryFromWireInt(prfAgilityInt, out PrfAgility pa);
            prfAgility = pa;
        }

        EmissionFunction? function = null;
        if (HasField(fields, offset + 14) && TryParseDecimalInt(fields[offset + 14].AsSpan(), out int functionInt))
        {
            _ = IntEnumExtensions.TryFromWireInt(functionInt, out EmissionFunction ef);
            function = ef;
        }

        int? spotNumber = null;
        if (HasField(fields, offset + 15) && TryParseDecimalInt(fields[offset + 15].AsSpan(), out int spotNumberInt))
        {
            spotNumber = spotNumberInt;
        }

        string? sidc = HasField(fields, offset + 16) ? fields[offset + 16] : null;
        string? comment = HasField(fields, offset + 17) ? fields[offset + 17] : null;

        message = new EmissionMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            EmissionId: emissionId,
            DeleteMode: deleteMode,
            SensorLatitude: sensorLat,
            SensorLongitude: sensorLon,
            SensorAltitude: sensorAlt,
            EmitterLatitude: emitterLat,
            EmitterLongitude: emitterLon,
            EmitterAltitude: emitterAlt,
            Bearing: bearing,
            Frequencies: frequencies,
            Bandwidth: bandwidth,
            Power: power,
            FreqAgility: freqAgility,
            PrfAgility: prfAgility,
            Function: function,
            SpotNumber: spotNumber,
            Sidc: sidc,
            Comment: comment);
        return true;
    }

    private static string SerializeEmission(EmissionMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.EmissionId);
        sb.Append(FieldSeparator);
        sb.Append(m.DeleteMode.ToWireString());
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.SensorLatitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.SensorLongitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.SensorAltitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.EmitterLatitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.EmitterLongitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.EmitterAltitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Bearing);
        sb.Append(FieldSeparator);
        sb.Append(m.Frequencies != null ? FormatDoubleList(m.Frequencies) : string.Empty);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Bandwidth);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Power);
        sb.Append(FieldSeparator);
        sb.Append(m.FreqAgility.HasValue
            ? m.FreqAgility.Value.ToWireInt().ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.PrfAgility.HasValue
            ? m.PrfAgility.Value.ToWireInt().ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Function.HasValue
            ? m.Function.Value.ToWireInt().ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.SpotNumber.HasValue
            ? m.SpotNumber.Value.ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Sidc ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Comment ?? string.Empty);
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // GENERIC
    // -------------------------------------------------------------------------

    private static bool TryParseGeneric(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        ContentType? contentType = null;
        if (HasField(fields, offset))
        {
            if (ContentTypeExtensions.TryFromWireString(fields[offset].AsSpan(), out ContentType ct))
            {
                contentType = ct;
            }
        }

        DataEncoding? encoding = null;
        if (HasField(fields, offset + 1))
        {
            if (DataEncodingExtensions.TryFromWireString(fields[offset + 1].AsSpan(), out DataEncoding enc))
            {
                encoding = enc;
            }
        }

        if (!HasField(fields, offset + 2))
        {
            Log.ParseFailureInvalidField(logger, "Content", string.Empty);
            return false;
        }
        string content = fields[offset + 2];

        message = new GenericMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            ContentType: contentType,
            Encoding: encoding,
            Content: content);
        return true;
    }

    private static string SerializeGeneric(GenericMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.ContentType.HasValue ? m.ContentType.Value.ToWireString() : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Encoding.HasValue ? m.Encoding.Value.ToWireString() : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Content);
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // GRAPHIC
    // -------------------------------------------------------------------------

    private static bool TryParseGraphic(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        GraphicType? graphicType = null;
        if (HasField(fields, offset) && TryParseDecimalInt(fields[offset].AsSpan(), out int graphicTypeInt))
        {
            _ = IntEnumExtensions.TryFromWireInt(graphicTypeInt, out GraphicType gt);
            graphicType = gt;
        }

        double? lineWidth = ParseOptionalDouble(fields, offset + 1);

        int? lineColor = null;
        if (HasField(fields, offset + 2) && TryParseRgbaHex(fields[offset + 2].AsSpan(), out int lc))
        {
            lineColor = lc;
        }

        int? fillColor = null;
        if (HasField(fields, offset + 3) && TryParseRgbaHex(fields[offset + 3].AsSpan(), out int fc))
        {
            fillColor = fc;
        }

        int? textColor = null;
        if (HasField(fields, offset + 4) && TryParseRgbaHex(fields[offset + 4].AsSpan(), out int tc))
        {
            textColor = tc;
        }

        DataEncoding? encoding = null;
        if (HasField(fields, offset + 5))
        {
            if (DataEncodingExtensions.TryFromWireString(fields[offset + 5].AsSpan(), out DataEncoding enc))
            {
                encoding = enc;
            }
        }

        string? annotation = null;
        if (HasField(fields, offset + 6))
        {
            if (encoding == DataEncoding.Base64)
            {
                TryFromBase64String(fields[offset + 6], out annotation);
            }
            else
            {
                annotation = fields[offset + 6];
            }
        }

        message = new GraphicMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            GraphicType: graphicType,
            LineWidth: lineWidth,
            LineColor: lineColor,
            FillColor: fillColor,
            TextColor: textColor,
            Encoding: encoding,
            Annotation: annotation);
        return true;
    }

    private static string SerializeGraphic(GraphicMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.GraphicType.HasValue
            ? m.GraphicType.Value.ToWireInt().ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.LineWidth.HasValue
            ? ((int)m.LineWidth.Value).ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.LineColor.HasValue ? FormatRgbaHex(m.LineColor.Value) : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.FillColor.HasValue ? FormatRgbaHex(m.FillColor.Value) : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.TextColor.HasValue ? FormatRgbaHex(m.TextColor.Value) : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Encoding.HasValue ? m.Encoding.Value.ToWireString() : string.Empty);
        sb.Append(FieldSeparator);
        if (m.Annotation != null)
        {
            sb.Append(m.Encoding == DataEncoding.Base64 ? ToBase64(m.Annotation) : m.Annotation);
        }
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // HEARTBEAT
    // -------------------------------------------------------------------------

    private static bool TryParseHeartbeat(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        string? recipient = HasField(fields, offset) ? fields[offset] : null;
        message = new HeartbeatMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            Recipient: recipient);
        return true;
    }

    private static string SerializeHeartbeat(HeartbeatMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.Recipient ?? string.Empty);
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // KEYEXCHANGE
    // -------------------------------------------------------------------------

    private static bool TryParseKeyexchange(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        string? recipient = HasField(fields, offset) ? fields[offset] : null;

        AlgorithmType? algorithmType = null;
        if (HasField(fields, offset + 1) && TryParseDecimalInt(fields[offset + 1].AsSpan(), out int algInt))
        {
            _ = IntEnumExtensions.TryFromWireInt(algInt, out AlgorithmType at);
            algorithmType = at;
        }

        int? phase = null;
        if (HasField(fields, offset + 2) && TryParseDecimalInt(fields[offset + 2].AsSpan(), out int phaseInt))
        {
            phase = phaseInt;
        }

        int? keyLenSecret = null;
        if (HasField(fields, offset + 3) && TryParseDecimalInt(fields[offset + 3].AsSpan(), out int klsInt))
        {
            keyLenSecret = klsInt;
        }

        int? keyLenDhKem = null;
        if (HasField(fields, offset + 4) && TryParseDecimalInt(fields[offset + 4].AsSpan(), out int kldInt))
        {
            keyLenDhKem = kldInt;
        }

        BigInteger? primeNumber = null;
        if (HasField(fields, offset + 5))
        {
            if (BigInteger.TryParse("00" + fields[offset + 5], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out BigInteger prime))
            {
                primeNumber = prime;
            }
        }

        BigInteger? naturalNumber = null;
        if (HasField(fields, offset + 6))
        {
            if (BigInteger.TryParse("00" + fields[offset + 6], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out BigInteger natural))
            {
                naturalNumber = natural;
            }
        }

        long? iv = null;
        if (HasField(fields, offset + 7) && TryParseHexLong16(fields[offset + 7].AsSpan(), out long ivVal))
        {
            iv = ivVal;
        }

        byte[]? publicKey = null;
        if (HasField(fields, offset + 8))
        {
            TryFromBase64Bytes(fields[offset + 8], out publicKey);
        }

        message = new KeyexchangeMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            Recipient: recipient,
            AlgorithmType: algorithmType,
            Phase: phase,
            KeyLengthSharedSecret: keyLenSecret,
            KeyLengthDhKem: keyLenDhKem,
            PrimeNumber: primeNumber,
            NaturalNumber: naturalNumber,
            Iv: iv,
            PublicKey: publicKey);
        return true;
    }

    private static string SerializeKeyexchange(KeyexchangeMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.Recipient ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.AlgorithmType.HasValue
            ? m.AlgorithmType.Value.ToWireInt().ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Phase.HasValue
            ? m.Phase.Value.ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.KeyLengthSharedSecret.HasValue
            ? m.KeyLengthSharedSecret.Value.ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.KeyLengthDhKem.HasValue
            ? m.KeyLengthDhKem.Value.ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.PrimeNumber.HasValue ? FormatBigIntegerHex(m.PrimeNumber.Value) : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.NaturalNumber.HasValue ? FormatBigIntegerHex(m.NaturalNumber.Value) : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Iv.HasValue
            ? m.Iv.Value.ToString("X16", CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.PublicKey != null ? ToBase64(m.PublicKey.ToArray()) : string.Empty);
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // METEO
    // -------------------------------------------------------------------------

    private static bool TryParseMeteo(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = new MeteoMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            SpeedThroughWater: ParseOptionalDouble(fields, offset),
            WaterSpeed: ParseOptionalDouble(fields, offset + 1),
            WaterDirection: ParseOptionalDouble(fields, offset + 2),
            WaterTemperature: ParseOptionalDouble(fields, offset + 3),
            WaterDepth: ParseOptionalDouble(fields, offset + 4),
            AirTemperature: ParseOptionalDouble(fields, offset + 5),
            DewPoint: ParseOptionalDouble(fields, offset + 6),
            HumidityRel: ParseOptionalDouble(fields, offset + 7),
            Pressure: ParseOptionalDouble(fields, offset + 8),
            WindSpeed: ParseOptionalDouble(fields, offset + 9),
            WindDirection: ParseOptionalDouble(fields, offset + 10),
            Visibility: ParseOptionalDouble(fields, offset + 11),
            CloudHeight: ParseOptionalDouble(fields, offset + 12),
            CloudCover: ParseOptionalDouble(fields, offset + 13),
            Reference: HasField(fields, offset + 14) ? fields[offset + 14] : null);
        return true;
    }

    private static string SerializeMeteo(MeteoMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        AppendOptionalDouble(sb, m.SpeedThroughWater);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.WaterSpeed);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.WaterDirection);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.WaterTemperature);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.WaterDepth);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.AirTemperature);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.DewPoint);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.HumidityRel);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Pressure);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.WindSpeed);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.WindDirection);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Visibility);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.CloudHeight);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.CloudCover);
        sb.Append(FieldSeparator);
        sb.Append(m.Reference ?? string.Empty);
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // OWNUNIT
    // -------------------------------------------------------------------------

    private static bool TryParseOwnUnit(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        if (!HasField(fields, offset) || !TryParseDouble(fields[offset].AsSpan(), out double latitude))
        {
            string latField = Field(fields, offset);
            Log.ParseFailureInvalidField(logger, "Latitude", latField);
            return false;
        }

        if (!HasField(fields, offset + 1) || !TryParseDouble(fields[offset + 1].AsSpan(), out double longitude))
        {
            string lonField = Field(fields, offset + 1);
            Log.ParseFailureInvalidField(logger, "Longitude", lonField);
            return false;
        }

        message = new OwnUnitMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            Latitude: latitude,
            Longitude: longitude,
            Altitude: ParseOptionalDouble(fields, offset + 2),
            Speed: ParseOptionalDouble(fields, offset + 3),
            Course: ParseOptionalDouble(fields, offset + 4),
            Heading: ParseOptionalDouble(fields, offset + 5),
            Roll: ParseOptionalDouble(fields, offset + 6),
            Pitch: ParseOptionalDouble(fields, offset + 7),
            Name: HasField(fields, offset + 8) ? fields[offset + 8] : null,
            Sidc: HasField(fields, offset + 9) ? fields[offset + 9] : null);
        return true;
    }

    private static string SerializeOwnUnit(OwnUnitMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(FormatDouble(m.Latitude));
        sb.Append(FieldSeparator);
        sb.Append(FormatDouble(m.Longitude));
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Altitude);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Speed);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Course);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Heading);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Roll);
        sb.Append(FieldSeparator);
        AppendOptionalDouble(sb, m.Pitch);
        sb.Append(FieldSeparator);
        sb.Append(m.Name ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Sidc ?? string.Empty);
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // RESEND
    // -------------------------------------------------------------------------

    private static bool TryParseResend(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        if (!HasField(fields, offset))
        {
            Log.ParseFailureInvalidField(logger, "Recipient", string.Empty);
            return false;
        }
        string recipient = fields[offset];

        string missingTypeField = Field(fields, offset + 1);
        if (!HasField(fields, offset + 1) || !MessageTypeExtensions.TryFromWireString(missingTypeField.AsSpan(), out MessageType missingType))
        {
            Log.ParseFailureInvalidField(logger, "MissingMessageType", missingTypeField);
            return false;
        }

        string missingNumberField = Field(fields, offset + 2);
        if (!HasField(fields, offset + 2) || !TryParseFullHexByte(missingNumberField.AsSpan(), out byte missingNumber))
        {
            Log.ParseFailureInvalidField(logger, "MissingMessageNumber", missingNumberField);
            return false;
        }

        message = new ResendMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            Recipient: recipient,
            MissingMessageType: missingType,
            MissingMessageNumber: missingNumber);
        return true;
    }

    private static string SerializeResend(ResendMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.Recipient);
        sb.Append(FieldSeparator);
        sb.Append(m.MissingMessageType.ToWireString());
        sb.Append(FieldSeparator);
        sb.Append(m.MissingMessageNumber.ToString("X2", CultureInfo.InvariantCulture));
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // STATUS
    // -------------------------------------------------------------------------

    private static bool TryParseStatus(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        TechnicalState? tecState = null;
        if (HasField(fields, offset) && TryParseDecimalInt(fields[offset].AsSpan(), out int tecStateInt))
        {
            _ = IntEnumExtensions.TryFromWireInt(tecStateInt, out TechnicalState ts);
            tecState = ts;
        }

        OperationalState? opsState = null;
        if (HasField(fields, offset + 1) && TryParseDecimalInt(fields[offset + 1].AsSpan(), out int opsStateInt))
        {
            _ = IntEnumExtensions.TryFromWireInt(opsStateInt, out OperationalState os);
            opsState = os;
        }

        IReadOnlyList<string>? ammoNames = null;
        IReadOnlyList<double>? ammoLevels = null;
        if (HasField(fields, offset + 2))
        {
            _ = TryParseResourceLevels(fields[offset + 2], out ammoNames, out ammoLevels);
        }

        IReadOnlyList<string>? fuelNames = null;
        IReadOnlyList<double>? fuelLevels = null;
        if (HasField(fields, offset + 3))
        {
            // TODO check if discard is OK
            _ = TryParseResourceLevels(fields[offset + 3], out fuelNames, out fuelLevels);
        }

        IReadOnlyList<string>? battNames = null;
        IReadOnlyList<double>? battLevels = null;
        if (HasField(fields, offset + 4))
        {
            // TODO check if discard is OK
            _ = TryParseResourceLevels(fields[offset + 4], out battNames, out battLevels);
        }

        int? cmdId = null;
        if (HasField(fields, offset + 5) && TryParseDecimalInt(fields[offset + 5].AsSpan(), out int cmdIdInt))
        {
            cmdId = cmdIdInt;
        }

        CommandState? cmdState = null;
        if (HasField(fields, offset + 6) && TryParseDecimalInt(fields[offset + 6].AsSpan(), out int cmdStateInt))
        {
            _ = IntEnumExtensions.TryFromWireInt(cmdStateInt, out CommandState cs);
            cmdState = cs;
        }

        string? hostname = null;
        if (HasField(fields, offset + 7))
        {
            TryFromBase64String(fields[offset + 7], out hostname);
        }

        IReadOnlyList<string>? mediaUrls = null;
        if (HasField(fields, offset + 8))
        {
            string[] urlParts = fields[offset + 8].Split(HashSeparator);
            List<string> urls = [];
            foreach (string part in urlParts)
            {
                if (TryFromBase64String(part, out string url))
                {
                    urls.Add(url);
                }
            }
            if (urls.Count > 0)
            {
                mediaUrls = urls;
            }
        }

        string? freeText = null;
        if (HasField(fields, offset + 9))
        {
            TryFromBase64String(fields[offset + 9], out freeText);
        }

        message = new StatusMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            TecState: tecState,
            OpsState: opsState,
            AmmunitionLevelNames: ammoNames,
            AmmunitionLevels: ammoLevels,
            FuelLevelNames: fuelNames,
            FuelLevels: fuelLevels,
            BatterieLevelNames: battNames,
            BatterieLevels: battLevels,
            CmdId: cmdId,
            CmdState: cmdState,
            Hostname: hostname,
            MediaUrls: mediaUrls,
            FreeText: freeText);
        return true;
    }

    private static string SerializeStatus(StatusMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.TecState.HasValue
            ? m.TecState.Value.ToWireInt().ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.OpsState.HasValue
            ? m.OpsState.Value.ToWireInt().ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.AmmunitionLevelNames != null && m.AmmunitionLevels != null
            ? FormatResourceLevels(m.AmmunitionLevelNames, m.AmmunitionLevels)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.FuelLevelNames != null && m.FuelLevels != null
            ? FormatResourceLevels(m.FuelLevelNames, m.FuelLevels)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.BatterieLevelNames != null && m.BatterieLevels != null
            ? FormatResourceLevels(m.BatterieLevelNames, m.BatterieLevels)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.CmdId.HasValue
            ? m.CmdId.Value.ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.CmdState.HasValue
            ? m.CmdState.Value.ToWireInt().ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Hostname != null ? ToBase64(m.Hostname) : string.Empty);
        sb.Append(FieldSeparator);
        if (m.MediaUrls is { Count: > 0 })
        {
            sb.Append(string.Join(HashSeparator, m.MediaUrls.Select(ToBase64)));
        }
        sb.Append(FieldSeparator);
        sb.Append(m.FreeText != null ? ToBase64(m.FreeText) : string.Empty);
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // TEXT
    // -------------------------------------------------------------------------

    private static bool TryParseText(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        message = null;

        string? recipient = HasField(fields, offset) ? fields[offset] : null;

        TextType? type = null;
        if (HasField(fields, offset + 1) && TryParseDecimalInt(fields[offset + 1].AsSpan(), out int typeInt))
        {
            _ = IntEnumExtensions.TryFromWireInt(typeInt, out TextType tt);
            type = tt;
        }

        DataEncoding? encoding = null;
        if (HasField(fields, offset + 2))
        {
            if (DataEncodingExtensions.TryFromWireString(fields[offset + 2].AsSpan(), out DataEncoding enc))
            {
                encoding = enc;
            }
        }

        string? textContent = null;
        if (HasField(fields, offset + 3))
        {
            if (encoding == DataEncoding.Base64)
            {
                TryFromBase64String(fields[offset + 3], out textContent);
            }
            else
            {
                textContent = fields[offset + 3];
            }
        }

        string? reference = HasField(fields, offset + 4) ? fields[offset + 4] : null;

        message = new TextMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            Recipient: recipient,
            Type: type,
            Encoding: encoding,
            TextContent: textContent,
            Reference: reference);
        return true;
    }

    private static string SerializeText(TextMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.Recipient ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Type.HasValue
            ? m.Type.Value.ToWireInt().ToString(CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(m.Encoding.HasValue ? m.Encoding.Value.ToWireString() : string.Empty);
        sb.Append(FieldSeparator);
        if (m.TextContent != null)
        {
            sb.Append(m.Encoding == DataEncoding.Base64 ? ToBase64(m.TextContent) : m.TextContent);
        }
        sb.Append(FieldSeparator);
        sb.Append(m.Reference ?? string.Empty);
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // TIMESYNC
    // -------------------------------------------------------------------------

    private static bool TryParseTimesync(
        string[] fields,
        int offset,
        in CommonHeader header,
        ILogger logger,
        [NotNullWhen(true)] out ISedapExpressMessage? message)
    {
        long? timestamp = null;
        if (HasField(fields, offset) && TryParseHexLong16(fields[offset].AsSpan(), out long ts))
        {
            timestamp = ts;
        }

        message = new TimesyncMessage(
            Number: header.Number,
            Time: header.Time,
            Sender: header.Sender,
            Classification: header.Classification,
            Acknowledgement: header.Acknowledgement,
            Mac: header.Mac,
            Timestamp: timestamp);
        return true;
    }

    private static string SerializeTimesync(TimesyncMessage m)
    {
        StringBuilder sb = SerializeCommonHeader(m);
        sb.Append(m.Timestamp.HasValue
            ? m.Timestamp.Value.ToString("X16", CultureInfo.InvariantCulture)
            : string.Empty);
        return TrimTrailingSemicolons(sb.ToString());
    }

    // -------------------------------------------------------------------------
    // Shared utilities
    // -------------------------------------------------------------------------

    [System.Diagnostics.CodeAnalysis.SuppressMessage("", "CA1859", Justification = "Will be used for other message types in the future.")]
    private static StringBuilder SerializeCommonHeader(ISedapExpressMessage message)
    {
        StringBuilder sb = new();
        sb.Append(message.MessageType.ToWireString());
        sb.Append(FieldSeparator);
        sb.Append(message.Number.HasValue
            ? message.Number.Value.ToString("X2", CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(message.Time.HasValue
            ? message.Time.Value.ToString("X", CultureInfo.InvariantCulture)
            : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(message.Sender ?? string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(message.Classification.ToWireChar());
        sb.Append(FieldSeparator);
        sb.Append(message.Acknowledgement.ToWireString());
        sb.Append(FieldSeparator);
        sb.Append(message.Mac ?? string.Empty);
        sb.Append(FieldSeparator);
        return sb;
    }

    private static string TrimTrailingSemicolons(string input) =>
        input.TrimEnd(FieldSeparator);

    private static string LogSafe(ReadOnlySpan<char> unsafeInput)
    {
        const int maxLength = 200;
        return unsafeInput
            [..Math.Min(unsafeInput.Length, maxLength)]
            .ToString()
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }

    private static bool IsHexChar(char c) =>
        c is (>= '0' and <= '9') or (>= 'A' and <= 'F') or (>= 'a' and <= 'f');

    private static int HexVal(char c) =>
        c is >= '0' and <= '9' ? c - '0' : char.ToUpperInvariant(c) - 'A' + 10;

    private static double? ParseOptionalDouble(string[] fields, int index)
    {
        if (!HasField(fields, index))
        {
            return null;
        }
        return TryParseDouble(fields[index].AsSpan(), out double val) ? val : null;
    }

    private static void AppendOptionalDouble(StringBuilder sb, double? value)
    {
        if (value.HasValue)
        {
            sb.Append(FormatDouble(value.Value));
        }
    }
}
