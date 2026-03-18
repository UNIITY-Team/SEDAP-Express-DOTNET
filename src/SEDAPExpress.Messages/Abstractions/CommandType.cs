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
    /// Restart.
    /// </summary>
    Restart,

    /// <summary>
    /// Standby.
    /// </summary>
    Standby,

    /// <summary>
    /// Sync time.
    /// </summary>
    SyncTime,

    /// <summary>
    /// Send status.
    /// </summary>
    SendStatus,

    /// <summary>
    /// Move.
    /// </summary>
    Move,

    /// <summary>
    /// Rotate.
    /// </summary>
    Rotate,

    /// <summary>
    /// Loiter.
    /// </summary>
    Loiter,

    /// <summary>
    /// Scan area.
    /// </summary>
    ScanArea,

    /// <summary>
    /// Take photo.
    /// </summary>
    TakePhoto,

    /// <summary>
    /// Make video.
    /// </summary>
    MakeVideo,

    /// <summary>
    /// Live video.
    /// </summary>
    LiveVideo,

    /// <summary>
    /// Engagement.
    /// </summary>
    Engagement,

    /// <summary>
    /// Sanitize.
    /// </summary>
    Sanitize,

    /// <summary>
    /// Generic action.
    /// </summary>
    GenericAction,
}
