using Microsoft.Extensions.Logging;

internal static partial class Log
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Debug,
        Message = "Failed to parse message: {Reason}. Input: {Input}")]
    public static partial void ParseFailure(
        ILogger logger,
        string reason,
        string input);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Debug,
        Message = "Failed to parse message: too few fields ({FieldCount}). Input: {Input}")]
    public static partial void ParseFailureTooFewFields(
        ILogger logger,
        int fieldCount,
        string input);

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Debug,
        Message = "Failed to parse message: unknown message type '{MessageTypeName}'. Input: {Input}")]
    public static partial void ParseFailureUnknownMessageType(
        ILogger logger,
        string messageTypeName,
        string input);

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Debug,
        Message = "Failed to parse message: invalid field '{FieldName}' with value '{FieldValue}'")]
    public static partial void ParseFailureInvalidField(
        ILogger logger,
        string fieldName,
        string fieldValue);
}
