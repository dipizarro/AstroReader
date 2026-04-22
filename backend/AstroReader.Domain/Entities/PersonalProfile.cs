using System;

namespace AstroReader.Domain.Entities;

public class PersonalProfile
{
    private PersonalProfile()
    {
    }

    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? SavedChartId { get; private set; }

    public string FullName { get; private set; } = string.Empty;
    public DateOnly BirthDate { get; private set; }
    public TimeOnly BirthTime { get; private set; }
    public DateTime BirthInstantUtc { get; private set; }
    public string BirthPlace { get; private set; } = string.Empty;
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public short TimezoneOffsetMinutes { get; private set; }

    public string SelfPerceptionFocus { get; private set; } = string.Empty;
    public string CurrentChallenge { get; private set; } = string.Empty;
    public string DesiredInsight { get; private set; } = string.Empty;
    public string? SelfDescription { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public PersonalProfile(
        string fullName,
        DateOnly birthDate,
        TimeOnly birthTime,
        string birthPlace,
        decimal latitude,
        decimal longitude,
        short timezoneOffsetMinutes,
        string selfPerceptionFocus,
        string currentChallenge,
        string desiredInsight,
        string? selfDescription = null,
        Guid? userId = null,
        Guid? savedChartId = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        SavedChartId = savedChartId;

        SetBirthIdentity(fullName, birthDate, birthTime, birthPlace, latitude, longitude, timezoneOffsetMinutes);
        SetSelfContext(selfPerceptionFocus, currentChallenge, desiredInsight, selfDescription);

        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = CreatedAtUtc;
    }

    public void AssignUser(Guid userId)
    {
        UserId = userId;
        Touch();
    }

    public void LinkToSavedChart(Guid savedChartId)
    {
        SavedChartId = savedChartId;
        Touch();
    }

    public void Update(
        string fullName,
        DateOnly birthDate,
        TimeOnly birthTime,
        string birthPlace,
        decimal latitude,
        decimal longitude,
        short timezoneOffsetMinutes,
        string selfPerceptionFocus,
        string currentChallenge,
        string desiredInsight,
        string? selfDescription)
    {
        SetBirthIdentity(fullName, birthDate, birthTime, birthPlace, latitude, longitude, timezoneOffsetMinutes);
        SetSelfContext(selfPerceptionFocus, currentChallenge, desiredInsight, selfDescription);
        Touch();
    }

    private void SetBirthIdentity(
        string fullName,
        DateOnly birthDate,
        TimeOnly birthTime,
        string birthPlace,
        decimal latitude,
        decimal longitude,
        short timezoneOffsetMinutes)
    {
        FullName = RequireValue(fullName, nameof(fullName));
        BirthDate = birthDate;
        BirthTime = birthTime;
        BirthPlace = RequireValue(birthPlace, nameof(birthPlace));
        Latitude = ValidateLatitude(latitude);
        Longitude = ValidateLongitude(longitude);
        TimezoneOffsetMinutes = ValidateTimezoneOffset(timezoneOffsetMinutes);
        BirthInstantUtc = BuildBirthInstantUtc(birthDate, birthTime, TimezoneOffsetMinutes);
    }

    private void SetSelfContext(
        string selfPerceptionFocus,
        string currentChallenge,
        string desiredInsight,
        string? selfDescription)
    {
        SelfPerceptionFocus = RequireValue(selfPerceptionFocus, nameof(selfPerceptionFocus));
        CurrentChallenge = RequireValue(currentChallenge, nameof(currentChallenge));
        DesiredInsight = RequireValue(desiredInsight, nameof(desiredInsight));
        SelfDescription = NormalizeOptional(selfDescription);
    }

    private void Touch()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static DateTime BuildBirthInstantUtc(DateOnly birthDate, TimeOnly birthTime, short timezoneOffsetMinutes)
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

    private static decimal ValidateLatitude(decimal latitude)
    {
        if (latitude is < -90 or > 90)
        {
            throw new ArgumentOutOfRangeException(nameof(latitude), "latitude must be between -90 and 90.");
        }

        return latitude;
    }

    private static decimal ValidateLongitude(decimal longitude)
    {
        if (longitude is < -180 or > 180)
        {
            throw new ArgumentOutOfRangeException(nameof(longitude), "longitude must be between -180 and 180.");
        }

        return longitude;
    }

    private static short ValidateTimezoneOffset(short timezoneOffsetMinutes)
    {
        if (timezoneOffsetMinutes is < -720 or > 840)
        {
            throw new ArgumentOutOfRangeException(
                nameof(timezoneOffsetMinutes),
                "timezoneOffsetMinutes must be between -720 and 840.");
        }

        return timezoneOffsetMinutes;
    }
}
