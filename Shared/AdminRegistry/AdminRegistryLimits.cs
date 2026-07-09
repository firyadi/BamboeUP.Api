namespace Shared.AdminRegistry;

public static class AdminRegistryLimits
{
    public const int MinimumAdministrators = 2;
    public const int MaximumAdministrators = 3;

    public static void Validate(IReadOnlyList<AdminRegistryEntry> administrators)
    {
        if (administrators == null)
            throw new ArgumentNullException(nameof(administrators));

        if (administrators.Count < MinimumAdministrators)
            throw new InvalidOperationException(
                $"Administrator registry requires at least {MinimumAdministrators} administrators.");

        if (administrators.Count > MaximumAdministrators)
            throw new InvalidOperationException(
                $"Administrator registry allows at most {MaximumAdministrators} administrators.");

        if (administrators.Select(a => a.UserId).Distinct().Count() != administrators.Count)
            throw new InvalidOperationException("Duplicate UserId entries are not allowed.");

        if (administrators.Any(a => string.IsNullOrWhiteSpace(a.UserName)))
            throw new InvalidOperationException("UserName is required for every administrator entry.");
    }
}
