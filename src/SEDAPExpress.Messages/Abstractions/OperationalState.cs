namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Operational state.
/// </summary>
public enum OperationalState
{
    /// <summary>
    /// Not operational.
    /// </summary>
    NotOperational,

    /// <summary>
    /// Degraded.
    /// </summary>
    Degraded,

    /// <summary>
    /// Operational.
    /// </summary>
    Operational,

    /// <summary>
    /// Operational semi-autonomous.
    /// </summary>
    OperationalSemiAutonomous,

    /// <summary>
    /// Operational autonomous.
    /// </summary>
    OperationalAutonomous,
}
