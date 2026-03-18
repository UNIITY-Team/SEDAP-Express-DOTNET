using System.Numerics;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Key exchange message.
/// </summary>
/// <param name="Number"><inheritdoc/></param>
/// <param name="Time"><inheritdoc/></param>
/// <param name="Sender"><inheritdoc/></param>
/// <param name="Classification"><inheritdoc/></param>
/// <param name="Acknowledgement"><inheritdoc/></param>
/// <param name="Mac"><inheritdoc/></param>
/// <param name="Recipient">Recipient.</param>
/// <param name="AlgorithmType">Algorithm type.</param>
/// <param name="Phase">Phase (0-3).</param>
/// <param name="KeyLengthSharedSecret">Key length of the shared secret (128 or 256).</param>
/// <param name="KeyLengthDhKem">Key length for DH/KEM (1024, 2048, or 4096).</param>
/// <param name="PrimeNumber">Prime number (hex).</param>
/// <param name="NaturalNumber">Natural number (hex).</param>
/// <param name="Iv">Initialization vector (16-char uppercase hex).</param>
/// <param name="PublicKey">Public key (base64 encoded).</param>
public sealed record class KeyexchangeMessage(
    byte? Number,
    long? Time,
    string? Sender,
    Classification Classification,
    Acknowledgement Acknowledgement,
    string? Mac,
    string? Recipient,
    AlgorithmType? AlgorithmType,
    int? Phase,
    int? KeyLengthSharedSecret,
    int? KeyLengthDhKem,
    BigInteger? PrimeNumber,
    BigInteger? NaturalNumber,
    long? Iv,
    IReadOnlyCollection<byte>? PublicKey) : ISedapExpressMessage
{
    /// <inheritdoc/>
    public MessageType MessageType => MessageType.Keyexchange;
}
