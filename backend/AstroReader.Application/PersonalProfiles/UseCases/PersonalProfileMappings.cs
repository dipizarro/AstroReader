using AstroReader.Application.PersonalProfiles.DTOs;
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
}
