using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Meteorological message.
/// </summary>
/// <param name="Number"><inheritdoc/></param>
/// <param name="Time"><inheritdoc/></param>
/// <param name="Sender"><inheritdoc/></param>
/// <param name="Classification"><inheritdoc/></param>
/// <param name="Acknowledgement"><inheritdoc/></param>
/// <param name="Mac"><inheritdoc/></param>
/// <param name="SpeedThroughWater">Speed through water.</param>
/// <param name="WaterSpeed">Water speed.</param>
/// <param name="WaterDirection">Water direction.</param>
/// <param name="WaterTemperature">Water temperature.</param>
/// <param name="WaterDepth">Water depth.</param>
/// <param name="AirTemperature">Air temperature.</param>
/// <param name="DewPoint">Dew point.</param>
/// <param name="HumidityRel">Relative humidity.</param>
/// <param name="Pressure">Pressure.</param>
/// <param name="WindSpeed">Wind speed.</param>
/// <param name="WindDirection">Wind direction.</param>
/// <param name="Visibility">Visibility.</param>
/// <param name="CloudHeight">Cloud height.</param>
/// <param name="CloudCover">Cloud cover.</param>
/// <param name="Reference">Reference.</param>
public sealed record class MeteoMessage(
    byte? Number,
    long? Time,
    string? Sender,
    Classification Classification,
    Acknowledgement Acknowledgement,
    string? Mac,
    double? SpeedThroughWater,
    double? WaterSpeed,
    double? WaterDirection,
    double? WaterTemperature,
    double? WaterDepth,
    double? AirTemperature,
    double? DewPoint,
    double? HumidityRel,
    double? Pressure,
    double? WindSpeed,
    double? WindDirection,
    double? Visibility,
    double? CloudHeight,
    double? CloudCover,
    string? Reference) : ISedapExpressMessage
{
    /// <inheritdoc/>
    public MessageType MessageType => MessageType.Meteo;
}
