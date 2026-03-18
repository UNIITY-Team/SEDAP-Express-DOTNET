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
    /// Operational limited.
    /// </summary>
    OperationalLimited,

    /// <summary>
    /// Operational manual.
    /// </summary>
    OperationalManual,

    /// <summary>
    /// Operational semi-autonomous.
    /// </summary>
    OperationalSemiAutonomous,

    /// <summary>
    /// Operational autonomous.
    /// </summary>
    OperationalAutonomous,
}
