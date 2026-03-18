using Bundeswehr.Uniity.SEDAPExpress.Messages;
using Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Tests;

public sealed class IntEnumExtensionsTests
{
    [Theory]
    [InlineData(AlgorithmType.DiffieHellmanMerkle, 0)]
    [InlineData(AlgorithmType.Rsa, 1)]
    [InlineData(AlgorithmType.Ecdh, 2)]
    [InlineData(AlgorithmType.Kyber512, 3)]
    [InlineData(AlgorithmType.Kyber768, 4)]
    [InlineData(AlgorithmType.Kyber1024, 5)]
    [InlineData(AlgorithmType.FrodoKem640, 6)]
    [InlineData(AlgorithmType.FrodoKem1344, 7)]
    public void AlgorithmTypeToWireIntReturnsExpected(AlgorithmType input, int expected) =>
        Assert.Equal(expected, input.ToWireInt());

    [Theory]
    [InlineData(0, AlgorithmType.DiffieHellmanMerkle)]
    [InlineData(7, AlgorithmType.FrodoKem1344)]
    public void AlgorithmTypeTryFromWireIntReturnsExpected(int input, AlgorithmType expected)
    {
        bool result = IntEnumExtensions.TryFromWireInt(input, out AlgorithmType actual);
        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(CommandMode.Add, 0)]
    [InlineData(CommandMode.Replace, 1)]
    [InlineData(CommandMode.CancelLast, 2)]
    [InlineData(CommandMode.CancelAll, 3)]
    public void CommandModeToWireIntReturnsExpected(CommandMode input, int expected) =>
        Assert.Equal(expected, input.ToWireInt());

    [Theory]
    [InlineData(0, CommandMode.Add)]
    [InlineData(3, CommandMode.CancelAll)]
    public void CommandModeTryFromWireIntReturnsExpected(int input, CommandMode expected)
    {
        bool result = IntEnumExtensions.TryFromWireInt(input, out CommandMode actual);
        Assert.True(result);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(GraphicType.Point, 0)]
    [InlineData(GraphicType.Line, 1)]
    [InlineData(GraphicType.Polyline, 2)]
    [InlineData(GraphicType.Polygon, 3)]
    [InlineData(GraphicType.Circle, 4)]
    [InlineData(GraphicType.Arc, 5)]
    [InlineData(GraphicType.Rectangle, 6)]
    [InlineData(GraphicType.Text, 7)]
    [InlineData(GraphicType.Arrow, 8)]
    [InlineData(GraphicType.Ellipsoid, 9)]
    public void GraphicTypeToWireIntReturnsExpected(GraphicType input, int expected) =>
        Assert.Equal(expected, input.ToWireInt());

    [Theory]
    [InlineData(TechnicalState.OffAbsent, 0)]
    [InlineData(TechnicalState.Standby, 1)]
    [InlineData(TechnicalState.Operational, 2)]
    [InlineData(TechnicalState.Degraded, 3)]
    [InlineData(TechnicalState.Fault, 4)]
    public void TechnicalStateToWireIntReturnsExpected(TechnicalState input, int expected) =>
        Assert.Equal(expected, input.ToWireInt());

    [Theory]
    [InlineData(OperationalState.NotOperational, 0)]
    [InlineData(OperationalState.OperationalLimited, 1)]
    [InlineData(OperationalState.OperationalManual, 2)]
    [InlineData(OperationalState.OperationalSemiAutonomous, 3)]
    [InlineData(OperationalState.OperationalAutonomous, 4)]
    public void OperationalStateToWireIntReturnsExpected(OperationalState input, int expected) =>
        Assert.Equal(expected, input.ToWireInt());

    [Theory]
    [InlineData(TextType.Undefined, 0)]
    [InlineData(TextType.Information, 1)]
    [InlineData(TextType.Warning, 2)]
    [InlineData(TextType.Alert, 3)]
    [InlineData(TextType.Chat, 4)]
    public void TextTypeToWireIntReturnsExpected(TextType input, int expected) =>
        Assert.Equal(expected, input.ToWireInt());

    [Theory]
    [InlineData(PrfAgility.FixedPeriodic, 0)]
    [InlineData(PrfAgility.Staggered, 1)]
    [InlineData(PrfAgility.Jittered, 2)]
    [InlineData(PrfAgility.Sliding, 3)]
    [InlineData(PrfAgility.Wobulated, 4)]
    [InlineData(PrfAgility.Switched, 5)]
    [InlineData(PrfAgility.Adaptive, 6)]
    [InlineData(PrfAgility.Unknown, 7)]
    public void PrfAgilityToWireIntReturnsExpected(PrfAgility input, int expected) =>
        Assert.Equal(expected, input.ToWireInt());

    [Theory]
    [InlineData(FreqAgility.StableFixed, 0)]
    [InlineData(FreqAgility.FrequencyHopping, 1)]
    [InlineData(FreqAgility.FrequencyAgile, 2)]
    [InlineData(FreqAgility.Chirp, 3)]
    [InlineData(FreqAgility.SpreadSpectrum, 4)]
    [InlineData(FreqAgility.Unknown, 5)]
    public void FreqAgilityToWireIntReturnsExpected(FreqAgility input, int expected) =>
        Assert.Equal(expected, input.ToWireInt());
}
