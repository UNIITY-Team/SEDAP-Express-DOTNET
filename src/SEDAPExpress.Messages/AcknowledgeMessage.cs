using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Acknowledge message.
/// </summary>
/// <param name="Number"><inheritdoc/></param>
/// <param name="Time"><inheritdoc/></param>
/// <param name="Sender"><inheritdoc/></param>
/// <param name="Classification"><inheritdoc/></param>
/// <param name="Mac"><inheritdoc/></param>
/// <param name="Recipient">Recipient</param>
/// <param name="AckedMessageNumber">Number of the acknowledged message.</param>
/// <param name="AckedMessageType">Type of the acknowledged message.</param>
public sealed record class AcknowledgeMessage(
    byte Number,
    long Time,
    string Sender,
    Classification Classification,
    string Mac,
    string Recipient,
    byte AckedMessageNumber,
    MessageType AckedMessageType) : ISedapExpressMessage
{
    /// <inheritdoc/>
    public MessageType MessageType => MessageType.Acknowledge;
}
