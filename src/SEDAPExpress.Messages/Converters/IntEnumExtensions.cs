using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages;

/// <summary>
/// Wire-format helpers for sequential integer-mapped enums.
/// </summary>
public static class IntEnumExtensions
{
    /// <summary>Converts this <see cref="AlgorithmType"/> to its wire integer.</summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire integer.</returns>
    public static int ToWireInt(this AlgorithmType value) => (int)value;

    /// <summary>Parses an <see cref="AlgorithmType"/> from a wire integer.</summary>
    /// <param name="input">Input integer.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/>.</returns>
    public static bool TryFromWireInt(int input, out AlgorithmType result)
    {
        result = (AlgorithmType)input;
        return true;
    }

    /// <summary>Converts this <see cref="CommandFlagType"/> to its wire integer.</summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire integer.</returns>
    public static int ToWireInt(this CommandFlagType value) => (int)value;

    /// <summary>Parses a <see cref="CommandFlagType"/> from a wire integer.</summary>
    /// <param name="input">Input integer.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/>.</returns>
    public static bool TryFromWireInt(int input, out CommandFlagType result)
    {
        result = (CommandFlagType)input;
        return true;
    }

    /// <summary>Converts this <see cref="CommandState"/> to its wire integer.</summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire integer.</returns>
    public static int ToWireInt(this CommandState value) => (int)value;

    /// <summary>Parses a <see cref="CommandState"/> from a wire integer.</summary>
    /// <param name="input">Input integer.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/>.</returns>
    public static bool TryFromWireInt(int input, out CommandState result)
    {
        result = (CommandState)input;
        return true;
    }

    /// <summary>Converts this <see cref="EmissionFunction"/> to its wire integer.</summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire integer.</returns>
    public static int ToWireInt(this EmissionFunction value) => (int)value;

    /// <summary>Parses an <see cref="EmissionFunction"/> from a wire integer.</summary>
    /// <param name="input">Input integer.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/>.</returns>
    public static bool TryFromWireInt(int input, out EmissionFunction result)
    {
        result = (EmissionFunction)input;
        return true;
    }

    /// <summary>Converts this <see cref="FreqAgility"/> to its wire integer.</summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire integer.</returns>
    public static int ToWireInt(this FreqAgility value) => (int)value;

    /// <summary>Parses a <see cref="FreqAgility"/> from a wire integer.</summary>
    /// <param name="input">Input integer.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/>.</returns>
    public static bool TryFromWireInt(int input, out FreqAgility result)
    {
        result = (FreqAgility)input;
        return true;
    }

    /// <summary>Converts this <see cref="GraphicType"/> to its wire integer.</summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire integer.</returns>
    public static int ToWireInt(this GraphicType value) => (int)value;

    /// <summary>Parses a <see cref="GraphicType"/> from a wire integer.</summary>
    /// <param name="input">Input integer.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/>.</returns>
    public static bool TryFromWireInt(int input, out GraphicType result)
    {
        result = (GraphicType)input;
        return true;
    }

    /// <summary>Converts this <see cref="OperationalState"/> to its wire integer.</summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire integer.</returns>
    public static int ToWireInt(this OperationalState value) => (int)value;

    /// <summary>Parses an <see cref="OperationalState"/> from a wire integer.</summary>
    /// <param name="input">Input integer.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/>.</returns>
    public static bool TryFromWireInt(int input, out OperationalState result)
    {
        result = (OperationalState)input;
        return true;
    }

    /// <summary>Converts this <see cref="PrfAgility"/> to its wire integer.</summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire integer.</returns>
    public static int ToWireInt(this PrfAgility value) => (int)value;

    /// <summary>Parses a <see cref="PrfAgility"/> from a wire integer.</summary>
    /// <param name="input">Input integer.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/>.</returns>
    public static bool TryFromWireInt(int input, out PrfAgility result)
    {
        result = (PrfAgility)input;
        return true;
    }

    /// <summary>Converts this <see cref="TechnicalState"/> to its wire integer.</summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire integer.</returns>
    public static int ToWireInt(this TechnicalState value) => (int)value;

    /// <summary>Parses a <see cref="TechnicalState"/> from a wire integer.</summary>
    /// <param name="input">Input integer.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/>.</returns>
    public static bool TryFromWireInt(int input, out TechnicalState result)
    {
        result = (TechnicalState)input;
        return true;
    }

    /// <summary>Converts this <see cref="TextType"/> to its wire integer.</summary>
    /// <param name="value">Value.</param>
    /// <returns>Wire integer.</returns>
    public static int ToWireInt(this TextType value) => (int)value;

    /// <summary>Parses a <see cref="TextType"/> from a wire integer.</summary>
    /// <param name="input">Input integer.</param>
    /// <param name="result">Result.</param>
    /// <returns><see langword="true"/>.</returns>
    public static bool TryFromWireInt(int input, out TextType result)
    {
        result = (TextType)input;
        return true;
    }
}
