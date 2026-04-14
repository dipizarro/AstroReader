using System.ComponentModel.DataAnnotations;

namespace AstroReader.Application.Interpretations.Premium;

public sealed record PremiumInterpretationPreviewRequest
{
    [Required]
    public string Sun { get; init; } = string.Empty;

    [Required]
    public string Moon { get; init; } = string.Empty;

    [Required]
    public string Ascendant { get; init; } = string.Empty;

    [Required]
    public string Mercury { get; init; } = string.Empty;

    [Required]
    public string Venus { get; init; } = string.Empty;

    [Required]
    public string Mars { get; init; } = string.Empty;
}
