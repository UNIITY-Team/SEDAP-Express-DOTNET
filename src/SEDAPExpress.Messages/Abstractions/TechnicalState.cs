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
    /// Standby.
    /// </summary>
    Standby,

    /// <summary>
    /// Operational.
    /// </summary>
    Operational,

    /// <summary>
    /// Degraded.
    /// </summary>
    Degraded,

    /// <summary>
    /// Fault.
    /// </summary>
    Fault,
}
