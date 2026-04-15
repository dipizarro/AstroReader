namespace AstroReader.Application.Interpretations.Premium;

internal sealed class PremiumInterpretationClaimRegistry
{
    private readonly HashSet<string> _usedClaimKeys = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _usedTexts = new(StringComparer.OrdinalIgnoreCase);

    public string Use(string claimKey, string value)
    {
        var normalized = NormalizeText(value);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return string.Empty;
        }

        if (!_usedClaimKeys.Add(claimKey) || !_usedTexts.Add(normalized))
        {
            return string.Empty;
        }

        return normalized;
    }

    public string UseOrFallback(string claimKey, string value, string fallback)
    {
        var selected = Use(claimKey, value);

        return !string.IsNullOrWhiteSpace(selected)
            ? selected
            : Use($"{claimKey}.fallback", fallback);
    }

    public IReadOnlyList<string> UseMany(params (string ClaimKey, string Value)[] claims)
    {
        return claims
            .Select(claim => Use(claim.ClaimKey, claim.Value))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    private static string NormalizeText(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim();
    }
}
