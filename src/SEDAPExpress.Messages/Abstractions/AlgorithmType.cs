namespace Bundeswehr.Uniity.SEDAPExpress.Messages.Abstractions;

/// <summary>
/// Key exchange algorithm type.
/// </summary>
public enum AlgorithmType
{
    /// <summary>
    /// Diffie-Hellman-Merkle.
    /// </summary>
    DiffieHellmanMerkle,

    /// <summary>
    /// RSA.
    /// </summary>
    Rsa,

    /// <summary>
    /// ECDH.
    /// </summary>
    Ecdh,

    /// <summary>
    /// Kyber-512.
    /// </summary>
    Kyber512,

    /// <summary>
    /// Kyber-768.
    /// </summary>
    Kyber768,

    /// <summary>
    /// Kyber-1024.
    /// </summary>
    Kyber1024,

    /// <summary>
    /// FrodoKEM-640.
    /// </summary>
    FrodoKem640,

    /// <summary>
    /// FrodoKEM-1344.
    /// </summary>
    FrodoKem1344,
}
