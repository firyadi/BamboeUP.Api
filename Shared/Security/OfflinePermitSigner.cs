using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Shared.DataTransferObjects;

namespace Shared.Security;

public static class OfflinePermitSigner
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    public static string CanonicalPayload(OfflineAccessPermitDto permit)
    {
        var unsigned = permit with { Signature = null };
        return JsonSerializer.Serialize(unsigned, JsonOptions);
    }

    public static OfflineAccessPermitDto Sign(OfflineAccessPermitDto permit, RSA privateKey)
    {
        var payload = Encoding.UTF8.GetBytes(CanonicalPayload(permit));
        var signature = privateKey.SignData(payload, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return permit with { Signature = Convert.ToBase64String(signature) };
    }

    public static bool Verify(OfflineAccessPermitDto permit, RSA publicKey)
    {
        if (string.IsNullOrWhiteSpace(permit.Signature))
            return false;
        var payload = Encoding.UTF8.GetBytes(CanonicalPayload(permit));
        var signature = Convert.FromBase64String(permit.Signature);
        return publicKey.VerifyData(payload, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public static bool IsClockSkewSafe(OfflineAccessPermitDto permit, DateTimeOffset now, TimeSpan maxSkew)
    {
        if (now + maxSkew < permit.IssuedAt)
            return false;
        return now <= permit.ExpiresAt;
    }
}
