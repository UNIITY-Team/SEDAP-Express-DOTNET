using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Emission message.
/// </summary>
/// <param name="Number"><inheritdoc/></param>
/// <param name="Time"><inheritdoc/></param>
/// <param name="Sender"><inheritdoc/></param>
/// <param name="Classification"><inheritdoc/></param>
/// <param name="Acknowledgement"><inheritdoc/></param>
/// <param name="Mac"><inheritdoc/></param>
/// <param name="EmissionId">Emission identifier.</param>
/// <param name="DeleteMode">Delete flag.</param>
/// <param name="SensorLatitude">Sensor latitude in degrees.</param>
/// <param name="SensorLongitude">Sensor longitude in degrees.</param>
/// <param name="SensorAltitude">Sensor altitude in meters.</param>
/// <param name="EmitterLatitude">Emitter latitude in degrees.</param>
/// <param name="EmitterLongitude">Emitter longitude in degrees.</param>
/// <param name="EmitterAltitude">Emitter altitude in meters.</param>
/// <param name="Bearing">Bearing (0-359.xxx degrees).</param>
/// <param name="Frequencies">Frequencies (MHz).</param>
/// <param name="Bandwidth">Bandwidth.</param>
/// <param name="Power">Power.</param>
/// <param name="FreqAgility">Frequency agility.</param>
/// <param name="PrfAgility">PRF agility.</param>
/// <param name="Function">Emission function.</param>
/// <param name="SpotNumber">Spot number.</param>
/// <param name="Sidc">SIDC (15 characters).</param>
/// <param name="Comment">Comment.</param>
public sealed record class EmissionMessage(
    byte? Number,
    long? Time,
    string? Sender,
    Classification Classification,
    Acknowledgement Acknowledgement,
    string? Mac,
    string EmissionId,
    DeleteMode DeleteMode,
    double? SensorLatitude,
    double? SensorLongitude,
    double? SensorAltitude,
    double? EmitterLatitude,
    double? EmitterLongitude,
    double? EmitterAltitude,
    double? Bearing,
    IReadOnlyList<double>? Frequencies,
    double? Bandwidth,
    double? Power,
    FreqAgility? FreqAgility,
    PrfAgility? PrfAgility,
    EmissionFunction? Function,
    int? SpotNumber,
    string? Sidc,
    string? Comment) : ISedapExpressMessage
{
    /// <inheritdoc/>
    public MessageType MessageType => MessageType.Emission;
}
