using System.ComponentModel.DataAnnotations;

namespace AstroReader.Application.PersonalProfiles.DTOs;

public record CreatePersonalProfileRequest
{
    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [MaxLength(120, ErrorMessage = "El fullName no puede exceder los 120 caracteres.")]
    public string FullName { get; init; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "El formato de birthDate debe ser YYYY-MM-DD.")]
    public string BirthDate { get; init; } = string.Empty;

    [Required(ErrorMessage = "La hora de nacimiento es obligatoria.")]
    [RegularExpression(@"^(?:[01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "El formato de birthTime debe ser HH:mm.")]
    public string BirthTime { get; init; } = string.Empty;

    [Required(ErrorMessage = "El lugar de nacimiento es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El birthPlace no puede exceder los 200 caracteres.")]
    public string BirthPlace { get; init; } = string.Empty;

    [Range(-90.0, 90.0, ErrorMessage = "La latitud debe estar entre -90 y 90 grados.")]
    public double Latitude { get; init; }

    [Range(-180.0, 180.0, ErrorMessage = "La longitud debe estar entre -180 y 180 grados.")]
    public double Longitude { get; init; }

    [Range(-720, 840, ErrorMessage = "El offset de zona horaria debe estar entre -12h y +14h.")]
    public int TimezoneOffsetMinutes { get; init; }

    [Required(ErrorMessage = "La autopercepción principal es obligatoria.")]
    [MaxLength(280, ErrorMessage = "El selfPerceptionFocus no puede exceder los 280 caracteres.")]
    public string SelfPerceptionFocus { get; init; } = string.Empty;

    [Required(ErrorMessage = "El desafío actual es obligatorio.")]
    [MaxLength(280, ErrorMessage = "El currentChallenge no puede exceder los 280 caracteres.")]
    public string CurrentChallenge { get; init; } = string.Empty;

    [Required(ErrorMessage = "El insight deseado es obligatorio.")]
    [MaxLength(280, ErrorMessage = "El desiredInsight no puede exceder los 280 caracteres.")]
    public string DesiredInsight { get; init; } = string.Empty;

    [MaxLength(600, ErrorMessage = "La selfDescription no puede exceder los 600 caracteres.")]
    public string? SelfDescription { get; init; }

    public Guid? UserId { get; init; }
    public Guid? SavedChartId { get; init; }
}
