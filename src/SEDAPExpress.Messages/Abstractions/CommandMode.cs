namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Command flag.
/// </summary>
public enum CommandMode
{
    /// <summary>
    /// Add.
    /// </summary>
    Add,

    /// <summary>
    /// Replace.
    /// </summary>
    Replace,

    /// <summary>
    /// Cancel last.
    /// </summary>
    CancelLast,

    /// <summary>
    /// Cancel all.
    /// </summary>
    CancelAll,
}
