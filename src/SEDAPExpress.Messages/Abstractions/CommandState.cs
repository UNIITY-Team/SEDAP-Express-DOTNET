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
    /// Executed successfully.
    /// </summary>
    ExecutedSuccessfully,

    /// <summary>
    /// Partially executed successfully.
    /// </summary>
    PartiallyExecutedSuccessfully,

    /// <summary>
    /// Executed not successfully.
    /// </summary>
    ExecutedNotSuccessfully,

    /// <summary>
    /// Execution not possible.
    /// </summary>
    ExecutionNotPossible,

    /// <summary>
    /// Will execute at.
    /// </summary>
    WillExecuteAt,
}
