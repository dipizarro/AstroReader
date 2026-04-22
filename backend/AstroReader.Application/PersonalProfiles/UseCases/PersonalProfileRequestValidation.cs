using AstroReader.Application.PersonalProfiles.DTOs;
using AstroReader.Application.PersonalProfiles.Exceptions;

namespace AstroReader.Application.PersonalProfiles.UseCases;

internal static class PersonalProfileRequestValidation
{
    public static (DateOnly BirthDate, TimeOnly BirthTime) ParseInputFormat(PersonalProfileWriteRequest request)
    {
        if (!DateOnly.TryParseExact(request.BirthDate, "yyyy-MM-dd", out var birthDate))
        {
            throw new ArgumentException("Formato de fecha inválido. Se requiere YYYY-MM-DD.");
        }

        if (!TimeOnly.TryParseExact(request.BirthTime, "HH:mm", out var birthTime))
        {
            throw new ArgumentException("Formato de hora inválido. Se requiere HH:mm.");
        }

        return (birthDate, birthTime);
    }

    public static void ValidateWriteRequest(PersonalProfileWriteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar sin fullName.");
        }

        if (string.IsNullOrWhiteSpace(request.BirthPlace))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar sin birthPlace.");
        }

        if (string.IsNullOrWhiteSpace(request.SelfPerceptionFocus))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar sin selfPerceptionFocus.");
        }

        if (string.IsNullOrWhiteSpace(request.CurrentChallenge))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar sin currentChallenge.");
        }

        if (string.IsNullOrWhiteSpace(request.DesiredInsight))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar sin desiredInsight.");
        }

        if (double.IsNaN(request.Latitude) || double.IsInfinity(request.Latitude))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar con una latitud inválida.");
        }

        if (double.IsNaN(request.Longitude) || double.IsInfinity(request.Longitude))
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar con una longitud inválida.");
        }

        if (request.Latitude is < -90 or > 90)
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar con una latitud fuera de rango.");
        }

        if (request.Longitude is < -180 or > 180)
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar con una longitud fuera de rango.");
        }

        if (request.TimezoneOffsetMinutes is < -720 or > 840)
        {
            throw new PersonalProfileIntegrityException("El perfil no se puede guardar con un offset horario fuera de rango.");
        }
    }
}
