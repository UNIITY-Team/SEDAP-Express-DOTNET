using System.Diagnostics.CodeAnalysis;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// (De-) Serializes SEDAP Express messages.
/// </summary>
public interface IMessageSerializer
{
    /// <summary>
    /// Deserializes a message.
    /// </summary>
    /// <param name="input">Input</param>
    /// <returns>The deserialized message.</returns>
    /// <exception cref="MessageParseException">Thrown if parsing fails.</exception>
    ISedapExpressMessage Deserialize(string input);

    /// <summary>
    /// Tries to deserialize a message.
    /// </summary>
    /// <param name="input">Input</param>
    /// <param name="message">The deserialized message</param>
    /// <returns><see langword="true"/> if deserialization is successful, else <see langword="false"/>.</returns>
    bool TryDeserialize(ReadOnlySpan<char> input, [NotNullWhen(true)] out ISedapExpressMessage? message);

    /// <summary>
    /// Serializes a message.
    /// </summary>
    /// <param name="message">Message.</param>
    /// <returns>A string representing the message</returns>
    string Serialize(ISedapExpressMessage message);
}
