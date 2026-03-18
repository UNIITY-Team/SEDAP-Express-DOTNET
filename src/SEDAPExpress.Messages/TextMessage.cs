using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Text message.
/// </summary>
/// <param name="Number"><inheritdoc/></param>
/// <param name="Time"><inheritdoc/></param>
/// <param name="Sender"><inheritdoc/></param>
/// <param name="Classification"><inheritdoc/></param>
/// <param name="Acknowledgement"><inheritdoc/></param>
/// <param name="Mac"><inheritdoc/></param>
/// <param name="Recipient">Recipient.</param>
/// <param name="Type">Text type.</param>
/// <param name="Encoding">Data encoding.</param>
/// <param name="TextContent">Text content.</param>
/// <param name="Reference">Reference.</param>
public sealed record class TextMessage(
    byte? Number,
    long? Time,
    string? Sender,
    Classification Classification,
    Acknowledgement Acknowledgement,
    string? Mac,
    string? Recipient,
    TextType? Type,
    DataEncoding? Encoding,
    string? TextContent,
    string? Reference) : ISedapExpressMessage
{
    /// <inheritdoc/>
    public MessageType MessageType => MessageType.Text;
}
