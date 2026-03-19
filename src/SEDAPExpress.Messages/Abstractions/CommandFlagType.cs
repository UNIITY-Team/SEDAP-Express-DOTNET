namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Command flag.
/// </summary>
/// <remarks>
/// The "Type" suffix is added to avoid a .NET analyzer warning:
/// Enums should not end with "Flag".
/// </remarks>
public enum CommandFlagType
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
