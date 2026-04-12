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
        var chart = DeserializeCalculatedChart(savedChart);

        return new SavedChartDetailDto
        {
            Id = savedChart.Id,
            UserId = savedChart.UserId,
            ProfileName = savedChart.ProfileName,
            PlaceName = savedChart.PlaceName,
            TimezoneIana = savedChart.TimezoneIana,
            BirthDate = savedChart.BirthDate.ToString("yyyy-MM-dd"),
            BirthTime = savedChart.BirthTime.ToString("HH:mm"),
            TimezoneOffsetMinutes = savedChart.TimezoneOffsetMinutes,
            BirthInstantUtc = savedChart.BirthInstantUtc,
            Latitude = (double)savedChart.Latitude,
            Longitude = (double)savedChart.Longitude,
            SunSign = savedChart.SunSign,
            MoonSign = savedChart.MoonSign,
            AscendantSign = savedChart.AscendantSign,
            CalculationEngine = savedChart.CalculationEngine,
            HouseSystemCode = savedChart.HouseSystemCode,
            SnapshotVersion = savedChart.SnapshotVersion,
            CreatedAtUtc = savedChart.CreatedAtUtc,
            UpdatedAtUtc = savedChart.UpdatedAtUtc,
            Chart = chart
        };
    }

    public static string SerializeSnapshot(CalculateChartResponse chart)
    {
        return JsonSerializer.Serialize(chart, JsonOptions);
    }

    private static CalculateChartResponse DeserializeCalculatedChart(SavedChart savedChart)
    {
        return savedChart.SnapshotVersion switch
        {
            SavedChart.CurrentSnapshotVersion => JsonSerializer.Deserialize<CalculateChartResponse>(savedChart.CalculatedChartJson, JsonOptions)
                ?? throw new InvalidOperationException($"Saved chart '{savedChart.Id}' contains an invalid snapshot."),
            _ => throw new NotSupportedException(
                $"Saved chart '{savedChart.Id}' uses unsupported snapshot version '{savedChart.SnapshotVersion}'.")
        };
    }
}
