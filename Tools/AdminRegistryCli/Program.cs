using Shared.AdminRegistry;

if (args.Length < 2)
{
    Console.WriteLine("Usage: AdminRegistryCli <outputPath> <encryptionKey> [userId:userName:grantedById ...]");
    Console.WriteLine("Example: AdminRegistryCli ..\\..\\BamboeUp.Api\\App_Data\\admin.registry.enc dev_key_32chars 1:admin:1 10004:Firyadi:100");
    return 1;
}

var outputPath = args[0];
var encryptionKey = args[1];
var entries = new List<AdminRegistryEntry>();

for (var i = 2; i < args.Length; i++)
{
    var parts = args[i].Split(':', 3);
    if (parts.Length != 3
        || !long.TryParse(parts[0], out var userId)
        || !long.TryParse(parts[2], out var grantedById))
    {
        Console.WriteLine($"Invalid entry format: {args[i]}");
        return 1;
    }

    entries.Add(new AdminRegistryEntry
    {
        UserId = userId,
        UserName = parts[1],
        GrantedById = grantedById,
        GrantedAt = DateTime.UtcNow
    });
}

AdminRegistryLimits.Validate(entries);
var encrypted = AdminRegistryCrypto.Encrypt(new AdminRegistryDocument { Administrators = entries }, encryptionKey);
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
File.WriteAllBytes(outputPath, encrypted);
Console.WriteLine($"Wrote {entries.Count} administrator(s) to {Path.GetFullPath(outputPath)}");
return 0;
