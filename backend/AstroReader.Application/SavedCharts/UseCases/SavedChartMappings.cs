using System.Text.Json;
using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Domain.Entities;

namespace AstroReader.Application.SavedCharts.UseCases;

internal static class SavedChartMappings
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static SavedChartDetailDto ToDetailDto(SavedChart savedChart)
    {
        var chart = JsonSerializer.Deserialize<CalculateChartResponse>(savedChart.ResultSnapshotJson, JsonOptions)
            ?? throw new InvalidOperationException($"Saved chart '{savedChart.Id}' contains an invalid snapshot.");

        return new SavedChartDetailDto
        {
            Id = savedChart.Id,
            UserId = savedChart.UserId,
            ProfileName = savedChart.ProfileName,
            PlaceName = savedChart.PlaceName,
            BirthDate = savedChart.BirthDate.ToString("yyyy-MM-dd"),
            BirthTime = savedChart.BirthTime.ToString("HH:mm"),
            TimezoneOffsetMinutes = savedChart.TimezoneOffsetMinutes,
            BirthInstantUtc = savedChart.BirthInstantUtc,
            Latitude = (double)savedChart.Latitude,
            Longitude = (double)savedChart.Longitude,
            SunSign = savedChart.SunSign,
            MoonSign = savedChart.MoonSign,
            AscendantSign = savedChart.AscendantSign,
            CreatedAtUtc = savedChart.CreatedAtUtc,
            UpdatedAtUtc = savedChart.UpdatedAtUtc,
            Chart = chart
        };
    }

    public static SavedChartListItemDto ToListItemDto(SavedChart savedChart)
    {
        return new SavedChartListItemDto
        {
            Id = savedChart.Id,
            ProfileName = savedChart.ProfileName,
            PlaceName = savedChart.PlaceName,
            BirthDate = savedChart.BirthDate.ToString("yyyy-MM-dd"),
            BirthTime = savedChart.BirthTime.ToString("HH:mm"),
            TimezoneOffsetMinutes = savedChart.TimezoneOffsetMinutes,
            SunSign = savedChart.SunSign,
            MoonSign = savedChart.MoonSign,
            AscendantSign = savedChart.AscendantSign,
            CreatedAtUtc = savedChart.CreatedAtUtc
        };
    }

    public static string SerializeSnapshot(CalculateChartResponse chart)
    {
        return JsonSerializer.Serialize(chart, JsonOptions);
    }
}
