using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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

        if (!TryParseMessageTypeName(fields[0].AsSpan(), out MessageType messageType))
        {
            Log.ParseFailureUnknownMessageType(_logger, fields[0], sanitized.Value);
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
            _ => false,
        };
    }

    /// <inheritdoc/>
    public string Serialize(ISedapExpressMessage message)
    {
        return message switch
        {
            AcknowledgeMessage m => SerializeAcknowledge(m),
            _ => throw new ArgumentException($"Unknown message type: {message.GetType().Name}", nameof(message)),
        };
    }

    private static bool TryParseMessageTypeName(ReadOnlySpan<char> input, out MessageType messageType)
    {
        if (MemoryExtensions.Equals(input, "ACKNOWLEDGE", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Acknowledge;
            return true;
        }
        if (MemoryExtensions.Equals(input, "COMMAND", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Command;
            return true;
        }
        if (MemoryExtensions.Equals(input, "CONTACT", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Contact;
            return true;
        }
        if (MemoryExtensions.Equals(input, "POINT", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Point;
            return true;
        }
        if (MemoryExtensions.Equals(input, "EMISSION", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Emission;
            return true;
        }
        if (MemoryExtensions.Equals(input, "GENERIC", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Generic;
            return true;
        }
        if (MemoryExtensions.Equals(input, "GRAPHIC", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Graphic;
            return true;
        }
        if (MemoryExtensions.Equals(input, "HEARTBEAT", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Heartbeat;
            return true;
        }
        if (MemoryExtensions.Equals(input, "KEYEXCHANGE", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Keyexchange;
            return true;
        }
        if (MemoryExtensions.Equals(input, "METEO", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Meteo;
            return true;
        }
        if (MemoryExtensions.Equals(input, "OWNUNIT", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.OwnUnit;
            return true;
        }
        if (MemoryExtensions.Equals(input, "RESEND", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Resend;
            return true;
        }
        if (MemoryExtensions.Equals(input, "STATUS", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Status;
            return true;
        }
        if (MemoryExtensions.Equals(input, "TEXT", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Text;
            return true;
        }
        if (MemoryExtensions.Equals(input, "TIMESYNC", StringComparison.OrdinalIgnoreCase))
        {
            messageType = MessageType.Timesync;
            return true;
        }

        messageType = default;
        return false;
    }

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
        Classification classification = ParseClassification(fields[offset + 3].AsSpan());
        Acknowledgement acknowledgement = ParseAcknowledgement(fields[offset + 4].AsSpan());
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

    private static Classification ParseClassification(ReadOnlySpan<char> input)
    {
        if (input.Length != 1)
        {
            return Classification.None;
        }
        return input[0] switch
        {
            'P' or 'p' => Classification.Public,
            'U' or 'u' => Classification.Unclas,
            'R' or 'r' => Classification.Restricted,
            'C' or 'c' => Classification.Confidential,
            'S' or 's' => Classification.Secret,
            'T' or 't' => Classification.TopSecret,
            _ => Classification.None,
        };
    }

    private static Acknowledgement ParseAcknowledgement(ReadOnlySpan<char> input) =>
        MemoryExtensions.Equals(input, "TRUE", StringComparison.OrdinalIgnoreCase)
            ? Acknowledgement.True
            : Acknowledgement.False;

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

        if (!TryParseMessageTypeName(fields[offset + 1].AsSpan(), out MessageType ackedMessageType))
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
        sb.Append(m.AckedMessageType.ToString().ToUpperInvariant());
        sb.Append(FieldSeparator);
        sb.Append(m.AckedMessageNumber.ToString("X2", CultureInfo.InvariantCulture));
        return TrimTrailingSemicolons(sb.ToString());
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("", "CA1859", Justification = "Will be used for other message types in the future.")]
    private static StringBuilder SerializeCommonHeader(ISedapExpressMessage message)
    {
        StringBuilder sb = new();
        sb.Append(message.MessageType.ToString().ToUpperInvariant());
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
        sb.Append(SerializeClassification(message.Classification));
        sb.Append(FieldSeparator);
        sb.Append(message.Acknowledgement == Acknowledgement.True ? "TRUE" : string.Empty);
        sb.Append(FieldSeparator);
        sb.Append(message.Mac ?? string.Empty);
        sb.Append(FieldSeparator);
        return sb;
    }

    private static char SerializeClassification(Classification classification) =>
        classification switch
        {
            Classification.Public => 'P',
            Classification.Unclas => 'U',
            Classification.Restricted => 'R',
            Classification.Confidential => 'C',
            Classification.Secret => 'S',
            Classification.TopSecret => 'T',
            Classification.None => '-',
            _ => '-',
        };

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
}
