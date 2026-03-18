using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Graphic message.
/// </summary>
/// <param name="Number"><inheritdoc/></param>
/// <param name="Time"><inheritdoc/></param>
/// <param name="Sender"><inheritdoc/></param>
/// <param name="Classification"><inheritdoc/></param>
/// <param name="Acknowledgement"><inheritdoc/></param>
/// <param name="Mac"><inheritdoc/></param>
/// <param name="GraphicType">Graphic type.</param>
/// <param name="LineWidth">Line width.</param>
/// <param name="LineColor">Line color (RGBA).</param>
/// <param name="FillColor">Fill color (RGBA).</param>
/// <param name="TextColor">Text color (RGBA).</param>
/// <param name="Encoding">Data encoding.</param>
/// <param name="Annotation">Annotation text.</param>
public sealed record class GraphicMessage(
    byte? Number,
    long? Time,
    string? Sender,
    Classification Classification,
    Acknowledgement Acknowledgement,
    string? Mac,
    GraphicType? GraphicType,
    double? LineWidth,
    int? LineColor,
    int? FillColor,
    int? TextColor,
    DataEncoding? Encoding,
    string? Annotation) : ISedapExpressMessage
{
    /// <inheritdoc/>
    public MessageType MessageType => MessageType.Graphic;
}
