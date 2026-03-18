namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Technical state.
/// </summary>
public enum TechnicalState
{
    /// <summary>
    /// Off/Absent.
    /// </summary>
    OffAbsent,

    /// <summary>
    /// Initializing.
    /// </summary>
    Initializing,

    /// <summary>
    /// Degraded.
    /// </summary>
    Degraded,

    /// <summary>
    /// Operational.
    /// </summary>
    Operational,

    /// <summary>
    /// Fault.
    /// </summary>
    Fault,
}
