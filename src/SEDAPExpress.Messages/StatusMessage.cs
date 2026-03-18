using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Status message.
/// </summary>
/// <param name="Number"><inheritdoc/></param>
/// <param name="Time"><inheritdoc/></param>
/// <param name="Sender"><inheritdoc/></param>
/// <param name="Classification"><inheritdoc/></param>
/// <param name="Acknowledgement"><inheritdoc/></param>
/// <param name="Mac"><inheritdoc/></param>
/// <param name="TecState">Technical state.</param>
/// <param name="OpsState">Operational state.</param>
/// <param name="AmmunitionLevelNames">Ammunition level names.</param>
/// <param name="AmmunitionLevels">Ammunition levels.</param>
/// <param name="FuelLevelNames">Fuel level names.</param>
/// <param name="FuelLevels">Fuel levels.</param>
/// <param name="BatterieLevelNames">Battery level names.</param>
/// <param name="BatterieLevels">Battery levels.</param>
/// <param name="CmdId">Command ID.</param>
/// <param name="CmdState">Command state.</param>
/// <param name="Hostname">Hostname (base64 encoded).</param>
/// <param name="MediaUrls">Media URLs (each base64 encoded, joined with '#').</param>
/// <param name="FreeText">Free text (base64 encoded).</param>
public sealed record class StatusMessage(
    byte? Number,
    long? Time,
    string? Sender,
    Classification Classification,
    Acknowledgement Acknowledgement,
    string? Mac,
    TechnicalState? TecState,
    OperationalState? OpsState,
    IReadOnlyList<string>? AmmunitionLevelNames,
    IReadOnlyList<double>? AmmunitionLevels,
    IReadOnlyList<string>? FuelLevelNames,
    IReadOnlyList<double>? FuelLevels,
    IReadOnlyList<string>? BatterieLevelNames,
    IReadOnlyList<double>? BatterieLevels,
    int? CmdId,
    CommandState? CmdState,
    string? Hostname,
    IReadOnlyList<string>? MediaUrls,
    string? FreeText) : ISedapExpressMessage
{
    /// <inheritdoc/>
    public MessageType MessageType => MessageType.Status;
}
