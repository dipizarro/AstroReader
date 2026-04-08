using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Application.SavedCharts.Interfaces;
using AstroReader.Domain.Entities;

namespace AstroReader.Application.SavedCharts.UseCases;

public class SaveChartUseCase : ISaveChartUseCase
{
    private readonly ISavedChartRepository _savedChartRepository;

    public SaveChartUseCase(ISavedChartRepository savedChartRepository)
    {
        _savedChartRepository = savedChartRepository;
    }

    public async Task<SavedChartDetailDto> ExecuteAsync(
        SaveChartRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!DateOnly.TryParseExact(request.BirthDate, "yyyy-MM-dd", out var birthDate))
        {
            throw new ArgumentException("Formato de fecha inválido. Se requiere YYYY-MM-DD.");
        }

        if (!TimeOnly.TryParseExact(request.BirthTime, "HH:mm", out var birthTime))
        {
            throw new ArgumentException("Formato de hora inválido. Se requiere HH:mm.");
        }

        var birthInstantUtc = BuildBirthInstantUtc(birthDate, birthTime, request.TimezoneOffsetMinutes);
        var snapshotJson = SavedChartMappings.SerializeSnapshot(request.Chart);

        var savedChart = new SavedChart(
            request.ProfileName,
            request.PlaceName,
            birthDate,
            birthTime,
            (short)request.TimezoneOffsetMinutes,
            birthInstantUtc,
            (decimal)request.Latitude,
            (decimal)request.Longitude,
            request.Chart.Summary.Sun,
            request.Chart.Summary.Moon,
            request.Chart.Summary.Ascendant,
            snapshotJson,
            request.UserId);

        var persistedChart = await _savedChartRepository.AddAsync(savedChart, cancellationToken);
        return SavedChartMappings.ToDetailDto(persistedChart);
    }

    private static DateTime BuildBirthInstantUtc(DateOnly birthDate, TimeOnly birthTime, int timezoneOffsetMinutes)
    {
        var dateTimeOffset = new DateTimeOffset(
            birthDate.Year,
            birthDate.Month,
            birthDate.Day,
            birthTime.Hour,
            birthTime.Minute,
            0,
            TimeSpan.FromMinutes(timezoneOffsetMinutes));

        return dateTimeOffset.UtcDateTime;
    }
}
