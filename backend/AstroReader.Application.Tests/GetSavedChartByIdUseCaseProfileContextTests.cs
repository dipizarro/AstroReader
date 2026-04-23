using AstroReader.Application.PersonalProfiles.Interfaces;
using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Application.SavedCharts.Interfaces;
using AstroReader.Application.SavedCharts.UseCases;
using AstroReader.Domain.Entities;
using Xunit;

namespace AstroReader.Application.Tests;

public sealed class GetSavedChartByIdUseCaseProfileContextTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnLinkedProfileSummary_WhenSavedChartHasAssociatedProfile()
    {
        var savedChart = CreateSavedChart();
        var personalProfile = CreatePersonalProfile(savedChart.Id);
        var useCase = new GetSavedChartByIdUseCase(
            new InMemorySavedChartRepository(savedChart),
            new InMemoryPersonalProfileRepository(personalProfile));

        var result = await useCase.ExecuteAsync(savedChart.Id);

        Assert.Equal(savedChart.Id, result.Id);
        Assert.Equal(personalProfile.Id, result.PersonalProfileId);
        Assert.NotNull(result.PersonalProfile);
        Assert.Equal(personalProfile.FullName, result.PersonalProfile!.FullName);
        Assert.Equal(personalProfile.SelfPerceptionFocus, result.PersonalProfile.SelfPerceptionFocus);
        Assert.Equal(personalProfile.CurrentChallenge, result.PersonalProfile.CurrentChallenge);
        Assert.Equal(personalProfile.DesiredInsight, result.PersonalProfile.DesiredInsight);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNullProfileContext_WhenSavedChartHasNoAssociatedProfile()
    {
        var savedChart = CreateSavedChart();
        var useCase = new GetSavedChartByIdUseCase(
            new InMemorySavedChartRepository(savedChart),
            new InMemoryPersonalProfileRepository());

        var result = await useCase.ExecuteAsync(savedChart.Id);

        Assert.Null(result.PersonalProfileId);
        Assert.Null(result.PersonalProfile);
    }

    private static SavedChart CreateSavedChart()
    {
        return new SavedChart(
            profileName: "Carta natal · Santiago",
            placeName: "Santiago, Chile",
            timezoneIana: "America/Santiago",
            birthDate: new DateOnly(2000, 1, 1),
            birthTime: new TimeOnly(12, 0),
            timezoneOffsetMinutes: -240,
            birthInstantUtc: new DateTime(2000, 1, 1, 16, 0, 0, DateTimeKind.Utc),
            latitude: -33.4472m,
            longitude: -70.6736m,
            sunSign: "Taurus",
            moonSign: "Leo",
            ascendantSign: "Libra",
            calculationEngine: "SwissEph",
            houseSystemCode: "P",
            snapshotVersion: SavedChart.CurrentSnapshotVersion,
            calculatedChartJson: """
                {
                  "summary": { "sun": "Taurus", "moon": "Leo", "ascendant": "Libra" },
                  "planets": [],
                  "houses": [],
                  "interpretation": {
                    "coverage": {
                      "coverageStatus": "fallback",
                      "isPremiumResult": false,
                      "isFallback": true,
                      "coveredEntries": [],
                      "missingEntries": [],
                      "composedBlocks": []
                    },
                    "hook": "",
                    "energyCore": { "key": "energyCore", "title": "Tu energía central", "mainText": "", "subBlocks": [] },
                    "core": { "key": "core", "title": "Tu núcleo", "mainText": "", "subBlocks": [] },
                    "personalDynamics": { "key": "personalDynamics", "title": "Tu forma de pensar, vincularte y actuar", "mainText": "", "subBlocks": [] },
                    "essentialSummary": { "key": "essentialSummary", "title": "Lo esencial de tu carta", "mainText": "", "subBlocks": [] },
                    "tensionsAndPotential": [],
                    "lifeAreas": [],
                    "profiles": [],
                    "closing": ""
                  }
                }
                """);
    }

    private static PersonalProfile CreatePersonalProfile(Guid savedChartId)
    {
        return new PersonalProfile(
            fullName: "Lucia Mar",
            birthDate: new DateOnly(2000, 1, 1),
            birthTime: new TimeOnly(12, 0),
            birthPlace: "Santiago, Chile",
            latitude: -33.4472m,
            longitude: -70.6736m,
            timezoneOffsetMinutes: -240,
            selfPerceptionFocus: "Mi sensibilidad y la forma en que percibo todo",
            currentChallenge: "Me cuesta sostener claridad emocional cuando todo cambia",
            desiredInsight: "Quiero entender mejor cómo confiar en mi intuición",
            selfDescription: "Soy alguien sensible, intensa y muy intuitiva.",
            savedChartId: savedChartId);
    }

    private sealed class InMemorySavedChartRepository : ISavedChartRepository
    {
        private readonly SavedChart _savedChart;

        public InMemorySavedChartRepository(SavedChart savedChart)
        {
            _savedChart = savedChart;
        }

        public Task<SavedChart> AddAsync(SavedChart savedChart, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<SavedChart?> GetByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<SavedChart?>(_savedChart.Id == id ? _savedChart : null);
        }

        public Task<IReadOnlyList<SavedChartListItemDto>> GetListItemsAsync(Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<SavedChartListItemDto>>([]);
        }
    }

    private sealed class InMemoryPersonalProfileRepository : IPersonalProfileRepository
    {
        private readonly PersonalProfile? _personalProfile;

        public InMemoryPersonalProfileRepository(PersonalProfile? personalProfile = null)
        {
            _personalProfile = personalProfile;
        }

        public Task<PersonalProfile> AddAsync(PersonalProfile personalProfile, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PersonalProfile?> GetByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PersonalProfile?>(_personalProfile?.Id == id ? _personalProfile : null);
        }

        public Task<PersonalProfile?> GetTrackedByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PersonalProfile?>(_personalProfile?.Id == id ? _personalProfile : null);
        }

        public Task<PersonalProfile?> GetBySavedChartIdAsync(Guid savedChartId, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PersonalProfile?>(_personalProfile?.SavedChartId == savedChartId ? _personalProfile : null);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
