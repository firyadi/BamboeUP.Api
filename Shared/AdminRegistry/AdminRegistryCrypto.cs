using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Shared.AdminRegistry;

/// <summary>
/// Encrypts/decrypts admin.registry.enc using AES-256-GCM.
/// Binary layout: magic(4) + version(1) + nonce(12) + ciphertext + tag(16)
/// </summary>
public static class AdminRegistryCrypto
{
    private static readonly byte[] Magic = "BUPA"u8.ToArray();
    private const byte FormatVersion = 1;
    private const int NonceSize = 12;
    private const int TagSize = 16;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static byte[] Encrypt(AdminRegistryDocument document, string encryptionKey)
    {
        ArgumentNullException.ThrowIfNull(document);
        AdminRegistryLimits.Validate(document.Administrators);

        var key = DeriveKey(encryptionKey);
        var plaintext = JsonSerializer.SerializeToUtf8Bytes(document, JsonOptions);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(key, TagSize);
        aes.Encrypt(nonce, plaintext, ciphertext, tag);

        var output = new byte[Magic.Length + 1 + NonceSize + ciphertext.Length + TagSize];
        var offset = 0;
        Buffer.BlockCopy(Magic, 0, output, offset, Magic.Length);
        offset += Magic.Length;
        output[offset++] = FormatVersion;
        Buffer.BlockCopy(nonce, 0, output, offset, NonceSize);
        offset += NonceSize;
        Buffer.BlockCopy(ciphertext, 0, output, offset, ciphertext.Length);
        offset += ciphertext.Length;
        Buffer.BlockCopy(tag, 0, output, offset, TagSize);
        return output;
    }

    public static AdminRegistryDocument Decrypt(byte[] encryptedPayload, string encryptionKey)
    {
        ArgumentNullException.ThrowIfNull(encryptedPayload);

        if (encryptedPayload.Length < Magic.Length + 1 + NonceSize + TagSize)
            throw new InvalidOperationException("Admin registry file is too short or corrupted.");

        if (!encryptedPayload.AsSpan(0, Magic.Length).SequenceEqual(Magic))
            throw new InvalidOperationException("Admin registry file has invalid magic header.");

        var version = encryptedPayload[Magic.Length];
        if (version != FormatVersion)
            throw new InvalidOperationException($"Unsupported admin registry format version: {version}.");

        var key = DeriveKey(encryptionKey);
        var nonceOffset = Magic.Length + 1;
        var nonce = encryptedPayload.AsSpan(nonceOffset, NonceSize).ToArray();
        var cipherOffset = nonceOffset + NonceSize;
        var cipherLength = encryptedPayload.Length - cipherOffset - TagSize;
        if (cipherLength <= 0)
            throw new InvalidOperationException("Admin registry ciphertext is empty.");

        var ciphertext = encryptedPayload.AsSpan(cipherOffset, cipherLength).ToArray();
        var tag = encryptedPayload.AsSpan(cipherOffset + cipherLength, TagSize).ToArray();
        var plaintext = new byte[cipherLength];

        using var aes = new AesGcm(key, TagSize);
        aes.Decrypt(nonce, ciphertext, tag, plaintext);

        var document = JsonSerializer.Deserialize<AdminRegistryDocument>(plaintext, JsonOptions)
            ?? throw new InvalidOperationException("Admin registry payload is invalid JSON.");

        AdminRegistryLimits.Validate(document.Administrators);
        return document;
    }

    public static byte[] DeriveKey(string encryptionKey)
    {
        if (string.IsNullOrWhiteSpace(encryptionKey))
            throw new InvalidOperationException("Admin registry encryption key is not configured.");

        if (encryptionKey.Length < 32)
            throw new InvalidOperationException("Admin registry encryption key must be at least 32 characters.");

        return SHA256.HashData(Encoding.UTF8.GetBytes(encryptionKey));
    }
}
