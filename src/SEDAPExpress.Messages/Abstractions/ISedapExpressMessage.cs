namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Abstraction of a SEDAP Express message.
/// </summary>
public interface ISedapExpressMessage
{
    /// <summary>
    /// Message type.
    /// </summary>
    public MessageType MessageType { get; }

    /// <summary>
    /// Message number.
    /// </summary>
    public byte? Number { get; }

    /// <summary>
    /// Message timestamp.
    /// </summary>
    public long? Time { get; }

    /// <summary>
    /// Message sender.
    /// </summary>
    public string? Sender { get; }

    /// <summary>
    /// Classification of the message.
    /// </summary>
    public Classification Classification { get; }

    /// <summary>
    /// Expresses if the sender expects acknowledgment.
    /// </summary>
    public Acknowledgement Acknowledgement { get; }

    /// <summary>
    /// Sender's MAC address.
    /// </summary>
    public string? Mac { get; }
}
