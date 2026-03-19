namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Delete flag.
/// </summary>
/// <remarks>
/// The "Type" suffix is added to avoid a .NET analyzer warning:
/// Enums should not end with "Flag".
/// </remarks>
public enum DeleteFlagType
{
    /// <summary>
    /// False (do not delete).
    /// </summary>
    False,

    /// <summary>
    /// True (delete).
    /// </summary>
    True,
}
