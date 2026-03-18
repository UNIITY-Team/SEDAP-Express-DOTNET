using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Contact message.
/// </summary>
/// <param name="Number"><inheritdoc/></param>
/// <param name="Time"><inheritdoc/></param>
/// <param name="Sender"><inheritdoc/></param>
/// <param name="Classification"><inheritdoc/></param>
/// <param name="Acknowledgement"><inheritdoc/></param>
/// <param name="Mac"><inheritdoc/></param>
/// <param name="ContactId">Contact identifier.</param>
/// <param name="DeleteMode">Delete flag.</param>
/// <param name="Latitude">Latitude in degrees.</param>
/// <param name="Longitude">Longitude in degrees.</param>
/// <param name="Altitude">Altitude in meters.</param>
/// <param name="RelativeXDistance">Relative X distance.</param>
/// <param name="RelativeYDistance">Relative Y distance.</param>
/// <param name="RelativeZDistance">Relative Z distance.</param>
/// <param name="Speed">Speed.</param>
/// <param name="Course">Course.</param>
/// <param name="Heading">Heading.</param>
/// <param name="Roll">Roll.</param>
/// <param name="Pitch">Pitch.</param>
/// <param name="Width">Width.</param>
/// <param name="Length">Length.</param>
/// <param name="Height">Height.</param>
/// <param name="Name">Name.</param>
/// <param name="Source">Contact source set.</param>
/// <param name="Sidc">SIDC (15 characters).</param>
/// <param name="Mmsi">MMSI.</param>
/// <param name="Icao">ICAO (1-6 hex chars).</param>
/// <param name="MultimediaData">Multimedia data (base64 encoded).</param>
/// <param name="Comment">Comment (base64 encoded).</param>
public sealed record class ContactMessage(
    byte? Number,
    long? Time,
    string? Sender,
    Classification Classification,
    Acknowledgement Acknowledgement,
    string? Mac,
    string ContactId,
    DeleteMode DeleteMode,
    double? Latitude,
    double? Longitude,
    double? Altitude,
    double? RelativeXDistance,
    double? RelativeYDistance,
    double? RelativeZDistance,
    double? Speed,
    double? Course,
    double? Heading,
    double? Roll,
    double? Pitch,
    double? Width,
    double? Length,
    double? Height,
    string? Name,
    IReadOnlySet<ContactSource>? Source,
    string? Sidc,
    string? Mmsi,
    string? Icao,
    IReadOnlyCollection<byte>? MultimediaData,
    string? Comment) : ISedapExpressMessage
{
    /// <inheritdoc/>
    public MessageType MessageType => MessageType.Contact;
}
