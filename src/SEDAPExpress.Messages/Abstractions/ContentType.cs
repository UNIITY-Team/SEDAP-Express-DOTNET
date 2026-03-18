namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Content type for generic messages.
/// </summary>
public enum ContentType
{
    /// <summary>
    /// SEDAP content.
    /// </summary>
    Sedap,

    /// <summary>
    /// ASCII content.
    /// </summary>
    Ascii,

    /// <summary>
    /// NMEA content.
    /// </summary>
    Nmea,

    /// <summary>
    /// XML content.
    /// </summary>
    Xml,

    /// <summary>
    /// JSON content.
    /// </summary>
    Json,

    /// <summary>
    /// Binary content.
    /// </summary>
    Binary,
}
