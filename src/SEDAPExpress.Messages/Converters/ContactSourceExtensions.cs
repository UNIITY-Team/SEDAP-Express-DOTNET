using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Wire-format helpers for <see cref="ContactSource"/>.
/// </summary>
public static class ContactSourceExtensions
{
    /// <summary>
    /// Converts this value to its wire character.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire character.</returns>
    public static char ToWireChar(this ContactSource value) =>
        value switch
        {
            ContactSource.None => ' ',
            ContactSource.Radar => 'R',
            ContactSource.Ais => 'A',
            ContactSource.Iff => 'I',
            ContactSource.Sonar => 'S',
            ContactSource.Ew => 'E',
            ContactSource.Optical => 'O',
            ContactSource.Synthetic => 'Y',
            ContactSource.Manual => 'M',
            _ => ' ',
        };

    /// <summary>
    /// Parses a <see cref="ContactSource"/> from a wire character, defaulting to
    /// <see cref="ContactSource.None"/> for unknown input.
    /// </summary>
    /// <param name="input">Input character.</param>
    /// <returns>The parsed value.</returns>
    public static ContactSource FromWireChar(char input) =>
        input switch
        {
            ' ' => ContactSource.None,
            'R' or 'r' => ContactSource.Radar,
            'A' or 'a' => ContactSource.Ais,
            'I' or 'i' => ContactSource.Iff,
            'S' or 's' => ContactSource.Sonar,
            'E' or 'e' => ContactSource.Ew,
            'O' or 'o' => ContactSource.Optical,
            'Y' or 'y' => ContactSource.Synthetic,
            'M' or 'm' => ContactSource.Manual,
            _ => ContactSource.None,
        };
}
