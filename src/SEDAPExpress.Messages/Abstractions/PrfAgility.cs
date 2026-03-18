namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Pulse repetition frequency agility.
/// </summary>
public enum PrfAgility
{
    /// <summary>
    /// Fixed/Periodic.
    /// </summary>
    FixedPeriodic,

    /// <summary>
    /// Staggered.
    /// </summary>
    Staggered,

    /// <summary>
    /// Jittered.
    /// </summary>
    Jittered,

    /// <summary>
    /// Sliding.
    /// </summary>
    Sliding,

    /// <summary>
    /// Wobulated.
    /// </summary>
    Wobulated,

    /// <summary>
    /// Switched.
    /// </summary>
    Switched,

    /// <summary>
    /// Adaptive.
    /// </summary>
    Adaptive,

    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown,
}
