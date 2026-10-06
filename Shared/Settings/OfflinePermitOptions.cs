namespace Shared.Settings;

public sealed class OfflinePermitOptions
{
    public const string SectionName = "OfflinePermit";

    /// <summary>PKCS#8 RSA private key, Base64. Generated at startup when empty (development only).</summary>
    public string? PrivateKeyBase64 { get; set; }

    public string? PublicKeyBase64 { get; set; }
}

public sealed class OfflinePermitKeyPair
{
    public required string PublicKeyBase64 { get; init; }
    public required string PrivateKeyBase64 { get; init; }
}
