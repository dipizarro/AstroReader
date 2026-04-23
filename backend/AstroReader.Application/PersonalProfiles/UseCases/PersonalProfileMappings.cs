using AstroReader.Application.PersonalProfiles.DTOs;
using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Domain.Entities;

namespace AstroReader.Application.PersonalProfiles.UseCases;

internal static class PersonalProfileMappings
{
    public static PersonalProfileDetailDto ToDetailDto(PersonalProfile personalProfile)
    {
        return new PersonalProfileDetailDto
        {
            Id = personalProfile.Id,
            UserId = personalProfile.UserId,
            SavedChartId = personalProfile.SavedChartId,
            FullName = personalProfile.FullName,
            BirthDate = personalProfile.BirthDate.ToString("yyyy-MM-dd"),
            BirthTime = personalProfile.BirthTime.ToString("HH:mm"),
            BirthInstantUtc = personalProfile.BirthInstantUtc,
            BirthPlace = personalProfile.BirthPlace,
            Latitude = (double)personalProfile.Latitude,
            Longitude = (double)personalProfile.Longitude,
            TimezoneOffsetMinutes = personalProfile.TimezoneOffsetMinutes,
            SelfPerceptionFocus = personalProfile.SelfPerceptionFocus,
            CurrentChallenge = personalProfile.CurrentChallenge,
            DesiredInsight = personalProfile.DesiredInsight,
            SelfDescription = personalProfile.SelfDescription,
            CreatedAtUtc = personalProfile.CreatedAtUtc,
            UpdatedAtUtc = personalProfile.UpdatedAtUtc
        };
    }

    public static PersonalProfileListItemDto ToListItemDto(PersonalProfile personalProfile)
    {
        return new PersonalProfileListItemDto
        {
            Id = personalProfile.Id,
            SavedChartId = personalProfile.SavedChartId,
            FullName = personalProfile.FullName,
            BirthDate = personalProfile.BirthDate.ToString("yyyy-MM-dd"),
            BirthTime = personalProfile.BirthTime.ToString("HH:mm"),
            BirthPlace = personalProfile.BirthPlace,
            Latitude = (double)personalProfile.Latitude,
            Longitude = (double)personalProfile.Longitude,
            TimezoneOffsetMinutes = personalProfile.TimezoneOffsetMinutes,
            SelfPerceptionFocus = personalProfile.SelfPerceptionFocus,
            CreatedAtUtc = personalProfile.CreatedAtUtc
        };
    }

    public static SavedChartPersonalProfileSummaryDto ToSavedChartSummaryDto(PersonalProfile personalProfile)
    {
        return new SavedChartPersonalProfileSummaryDto
        {
            Id = personalProfile.Id,
            FullName = personalProfile.FullName,
            SelfPerceptionFocus = personalProfile.SelfPerceptionFocus,
            CurrentChallenge = personalProfile.CurrentChallenge,
            DesiredInsight = personalProfile.DesiredInsight
        };
    }
}
