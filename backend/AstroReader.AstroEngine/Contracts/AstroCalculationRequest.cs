using System;

namespace AstroReader.AstroEngine.Contracts;

/// <summary>
/// Representa el input mínimo estrictamente matemático para calcular una carta astral.
/// Desacoplado de los DTOs HTTP o perfiles de usuario.
/// </summary>
public record AstroCalculationRequest(
    DateTime UtcDateTime, 
    double Latitude, 
    double Longitude
);
