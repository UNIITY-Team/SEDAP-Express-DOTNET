namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Exception to be thrown by parsing logic.
/// </summary>
public sealed class MessageParseException : Exception
{
    /// <inheritdoc/>
    public MessageParseException() : base() { }

    /// <inheritdoc/>
    public MessageParseException(string message) : base(message) { }

    /// <inheritdoc/>
    public MessageParseException(string message, Exception inner) : base(message, inner) { }
}
