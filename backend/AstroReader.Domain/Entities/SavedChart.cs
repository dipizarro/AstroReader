using System;

namespace AstroReader.Domain.Entities;

public class SavedChart
{
    public const int CurrentSnapshotVersion = 1;

    private SavedChart()
    {
    }

    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }

    // Original input provided by the user at save time.
    public string ProfileName { get; private set; } = string.Empty;
    public string? PlaceName { get; private set; }
    public string? TimezoneIana { get; private set; }

    public DateOnly BirthDate { get; private set; }
    public TimeOnly BirthTime { get; private set; }
    public short TimezoneOffsetMinutes { get; private set; }
    public DateTime BirthInstantUtc { get; private set; }

    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    // Explicit derived fields kept for fast listing and future querying.
    public string SunSign { get; private set; } = string.Empty;
    public string MoonSign { get; private set; } = string.Empty;
    public string AscendantSign { get; private set; } = string.Empty;

    // Versioned serialized chart payload produced by the backend.
    public int SnapshotVersion { get; private set; }
    public string CalculatedChartJson { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public SavedChart(
        string profileName,
        string? placeName,
        string? timezoneIana,
        DateOnly birthDate,
        TimeOnly birthTime,
        short timezoneOffsetMinutes,
        DateTime birthInstantUtc,
        decimal latitude,
        decimal longitude,
        string sunSign,
        string moonSign,
        string ascendantSign,
        int snapshotVersion,
        string calculatedChartJson,
        Guid? userId = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;

        ProfileName = RequireValue(profileName, nameof(profileName));
        PlaceName = NormalizeOptional(placeName);
        TimezoneIana = NormalizeOptional(timezoneIana);

        BirthDate = birthDate;
        BirthTime = birthTime;
        TimezoneOffsetMinutes = timezoneOffsetMinutes;
        BirthInstantUtc = DateTime.SpecifyKind(birthInstantUtc, DateTimeKind.Utc);

        Latitude = latitude;
        Longitude = longitude;

        SunSign = RequireValue(sunSign, nameof(sunSign));
        MoonSign = RequireValue(moonSign, nameof(moonSign));
        AscendantSign = RequireValue(ascendantSign, nameof(ascendantSign));
        SnapshotVersion = snapshotVersion > 0
            ? snapshotVersion
            : throw new ArgumentOutOfRangeException(nameof(snapshotVersion), "snapshotVersion must be greater than zero.");
        CalculatedChartJson = RequireValue(calculatedChartJson, nameof(calculatedChartJson));

        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public void AssignUser(Guid userId)
    {
        UserId = userId;
        Touch();
    }

    public void Rename(string profileName)
    {
        ProfileName = RequireValue(profileName, nameof(profileName));
        Touch();
    }

    public void UpdateTimezoneIana(string? timezoneIana)
    {
        TimezoneIana = NormalizeOptional(timezoneIana);
        Touch();
    }

    public void UpdateSnapshot(
        string sunSign,
        string moonSign,
        string ascendantSign,
        int snapshotVersion,
        string calculatedChartJson)
    {
        SunSign = RequireValue(sunSign, nameof(sunSign));
        MoonSign = RequireValue(moonSign, nameof(moonSign));
        AscendantSign = RequireValue(ascendantSign, nameof(ascendantSign));
        SnapshotVersion = snapshotVersion > 0
            ? snapshotVersion
            : throw new ArgumentOutOfRangeException(nameof(snapshotVersion), "snapshotVersion must be greater than zero.");
        CalculatedChartJson = RequireValue(calculatedChartJson, nameof(calculatedChartJson));

        Touch();
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string RequireValue(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} is required.", paramName);
        }

        return value.Trim();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
