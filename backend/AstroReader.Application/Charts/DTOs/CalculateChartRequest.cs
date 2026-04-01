using System.ComponentModel.DataAnnotations;

namespace AstroReader.Application.Charts.DTOs;

public record CalculateChartRequest
{
    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "El formato de birthDate debe ser YYYY-MM-DD.")]
    public string BirthDate { get; init; } = string.Empty;

    [Required(ErrorMessage = "La hora de nacimiento es obligatoria.")]
    [RegularExpression(@"^(?:[01]\d|2[0-3]):[0-5]\d$", ErrorMessage = "El formato de birthTime debe ser HH:mm.")]
    public string BirthTime { get; init; } = string.Empty;

    [Required(ErrorMessage = "El lugar de nacimiento es obligatorio.")]
    [MinLength(2, ErrorMessage = "El lugar de nacimiento debe tener al menos 2 caracteres.")]
    public string BirthPlace { get; init; } = string.Empty;
}
