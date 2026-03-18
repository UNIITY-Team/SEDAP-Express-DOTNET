namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Command state.
/// </summary>
public enum CommandState
{
    /// <summary>
    /// Undefined.
    /// </summary>
    Undefined,

    /// <summary>
    /// Received.
    /// </summary>
    Received,

    /// <summary>
    /// Executing.
    /// </summary>
    Executing,

    /// <summary>
    /// Executed.
    /// </summary>
    Executed,

    /// <summary>
    /// Rejected.
    /// </summary>
    Rejected,

    /// <summary>
    /// Will execute at.
    /// </summary>
    WillExecuteAt,
}
