namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Message type.
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Acknowledge.
    /// </summary>
    Acknowledge,

    /// <summary>
    /// Command.
    /// </summary>
    Command,

    /// <summary>
    /// Contact.
    /// </summary>
    Contact,

    /// <summary>
    /// Point.
    /// </summary>
    Point,

    /// <summary>
    /// Emission.
    /// </summary>
    Emission,

    /// <summary>
    /// Generic.
    /// </summary>
    Generic,

    /// <summary>
    /// Graphic.
    /// </summary>
    Graphic,

    /// <summary>
    /// Heartbeat.
    /// </summary>
    Heartbeat,

    /// <summary>
    /// Keyexchange.
    /// </summary>
    Keyexchange,

    /// <summary>
    /// Meteo.
    /// </summary>
    Meteo,

    /// <summary>
    /// Own unit.
    /// </summary>
    OwnUnit,

    /// <summary>
    /// Resend.
    /// </summary>
    Resend,

    /// <summary>
    /// Status.
    /// </summary>
    Status,

    /// <summary>
    /// Text.
    /// </summary>
    Text,

    /// <summary>
    /// Timesync.
    /// </summary>
    Timesync,
}
