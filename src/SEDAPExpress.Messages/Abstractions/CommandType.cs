namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Command type.
/// </summary>
public enum CommandType
{
    /// <summary>
    /// Poweroff.
    /// </summary>
    Poweroff,

    /// <summary>
    /// Reboot.
    /// </summary>
    Reboot,

    /// <summary>
    /// Shutdown.
    /// </summary>
    Shutdown,

    /// <summary>
    /// Start.
    /// </summary>
    Start,

    /// <summary>
    /// Stop.
    /// </summary>
    Stop,

    /// <summary>
    /// Pause.
    /// </summary>
    Pause,

    /// <summary>
    /// Resume.
    /// </summary>
    Resume,

    /// <summary>
    /// Reset.
    /// </summary>
    Reset,

    /// <summary>
    /// Configure.
    /// </summary>
    Configure,

    /// <summary>
    /// Update.
    /// </summary>
    Update,

    /// <summary>
    /// Generic action.
    /// </summary>
    GenericAction,
}
