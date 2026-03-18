using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Wire-format helpers for <see cref="Classification"/>.
/// </summary>
public static class ClassificationExtensions
{
    /// <summary>
    /// Converts this value to its single-character wire representation.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire character.</returns>
    public static char ToWireChar(this Classification value) =>
        value switch
        {
            Classification.Public => 'P',
            Classification.Unclas => 'U',
            Classification.Restricted => 'R',
            Classification.Confidential => 'C',
            Classification.Secret => 'S',
            Classification.TopSecret => 'T',
            _ => '-',
        };

    /// <summary>
    /// Tries to parse a <see cref="Classification"/> from a single wire character.
    /// </summary>
    /// <param name="input">Input character.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/> if parsing succeeded.</returns>
    public static bool TryFromWireChar(char input, out Classification result)
    {
        switch (char.ToUpperInvariant(input))
        {
            case 'P': result = Classification.Public; return true;
            case 'U': result = Classification.Unclas; return true;
            case 'R': result = Classification.Restricted; return true;
            case 'C': result = Classification.Confidential; return true;
            case 'S': result = Classification.Secret; return true;
            case 'T': result = Classification.TopSecret; return true;
            case '-': result = Classification.None; return true;
            default: result = default; return false;
        }
    }
}
