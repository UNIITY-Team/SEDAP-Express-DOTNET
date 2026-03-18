using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Own unit message.
/// </summary>
/// <param name="Number"><inheritdoc/></param>
/// <param name="Time"><inheritdoc/></param>
/// <param name="Sender"><inheritdoc/></param>
/// <param name="Classification"><inheritdoc/></param>
/// <param name="Acknowledgement"><inheritdoc/></param>
/// <param name="Mac"><inheritdoc/></param>
/// <param name="Latitude">Latitude in degrees.</param>
/// <param name="Longitude">Longitude in degrees.</param>
/// <param name="Altitude">Altitude in meters.</param>
/// <param name="Speed">Speed.</param>
/// <param name="Course">Course.</param>
/// <param name="Heading">Heading.</param>
/// <param name="Roll">Roll.</param>
/// <param name="Pitch">Pitch.</param>
/// <param name="Name">Name.</param>
/// <param name="Sidc">SIDC (15 characters).</param>
public sealed record class OwnUnitMessage(
    byte? Number,
    long? Time,
    string? Sender,
    Classification Classification,
    Acknowledgement Acknowledgement,
    string? Mac,
    double Latitude,
    double Longitude,
    double? Altitude,
    double? Speed,
    double? Course,
    double? Heading,
    double? Roll,
    double? Pitch,
    string? Name,
    string? Sidc) : ISedapExpressMessage
{
    /// <inheritdoc/>
    public MessageType MessageType => MessageType.OwnUnit;
}
