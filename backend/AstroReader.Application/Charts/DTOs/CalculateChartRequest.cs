using System.ComponentModel.DataAnnotations;

namespace AstroReader.Application.Charts.DTOs;

public record CalculateChartRequest
{
    // --- Campos Reales de Cálculo Astral ---

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "El formato de birthDate debe ser YYYY-MM-DD.")]
    public string BirthDate { get; init; } = string.Empty;

    [Required(ErrorMessage = "La hora de nacimiento es obligatoria.")]
    [RegularExpression(@"^(?:[01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "El formato de birthTime debe ser HH:mm.")]
    public string BirthTime { get; init; } = string.Empty;

    [Required(ErrorMessage = "La latitud es obligatoria para el motor matemático.")]
    [Range(-90.0, 90.0, ErrorMessage = "La latitud debe estar entre -90 y 90 grados.")]
    public double Latitude { get; init; }

    [Required(ErrorMessage = "La longitud es obligatoria para el motor matemático.")]
    [Range(-180.0, 180.0, ErrorMessage = "La longitud debe estar entre -180 y 180 grados.")]
    public double Longitude { get; init; }

    [Required(ErrorMessage = "El desfase de zona horaria (en minutos respecto a UTC) es obligatorio.")]
    [Range(-720, 840, ErrorMessage = "El offset de zona horaria debe ser realista (entre -12h y +14h).")]
    public int TimezoneOffsetMinutes { get; init; }

    // --- Campos Descriptivos para UI ---

    [MaxLength(200, ErrorMessage = "El nombre del lugar no puede exceder los 200 caracteres.")]
    public string? PlaceName { get; init; }
}
