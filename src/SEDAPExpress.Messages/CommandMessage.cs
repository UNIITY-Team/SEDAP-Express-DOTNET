using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Command message.
/// </summary>
/// <param name="Number"><inheritdoc/></param>
/// <param name="Time"><inheritdoc/></param>
/// <param name="Sender"><inheritdoc/></param>
/// <param name="Classification"><inheritdoc/></param>
/// <param name="Acknowledgement"><inheritdoc/></param>
/// <param name="Mac"><inheritdoc/></param>
/// <param name="Recipient">Recipient.</param>
/// <param name="CmdId">Command ID (optional hex short).</param>
/// <param name="CmdFlag">Command flag.</param>
/// <param name="CmdType">Command type.</param>
/// <param name="CmdTypeDependentParameters">Command-type-dependent parameters.</param>
public sealed record class CommandMessage(
    byte? Number,
    long? Time,
    string? Sender,
    Classification Classification,
    Acknowledgement Acknowledgement,
    string? Mac,
    string? Recipient,
    short? CmdId,
    CommandFlagType CmdFlag,
    CommandType CmdType,
    IReadOnlyList<string>? CmdTypeDependentParameters) : ISedapExpressMessage
{
    /// <inheritdoc/>
    public MessageType MessageType => MessageType.Command;
}
