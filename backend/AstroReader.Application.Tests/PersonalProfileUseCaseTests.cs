using AstroReader.Application.PersonalProfiles.DTOs;
using AstroReader.Application.PersonalProfiles.Exceptions;
using AstroReader.Application.PersonalProfiles.Interfaces;
using AstroReader.Application.PersonalProfiles.UseCases;
using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Application.SavedCharts.Interfaces;
using AstroReader.Domain.Entities;
using Xunit;

namespace AstroReader.Application.Tests;

public sealed class PersonalProfileUseCaseTests
{
    [Fact]
    public async Task Create_ShouldPersistProfile_WhenRequestIsValid()
    {
        var personalProfileRepository = new InMemoryPersonalProfileRepository();
        var savedChartRepository = new InMemorySavedChartRepository();
        var useCase = new CreatePersonalProfileUseCase(personalProfileRepository, savedChartRepository);

        var result = await useCase.ExecuteAsync(CreateRequest());

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Lucia Mar", result.FullName);
        Assert.Equal("2000-01-01", result.BirthDate);
        Assert.Equal("12:00", result.BirthTime);
        Assert.Equal("Santiago, Chile", result.BirthPlace);
        Assert.Single(personalProfileRepository.StoredProfiles);
    }

    [Fact]
    public async Task GetById_ShouldReturnPersistedProfile()
    {
        var profile = CreatePersonalProfile();
        var personalProfileRepository = new InMemoryPersonalProfileRepository(profile);
        var useCase = new GetPersonalProfileByIdUseCase(personalProfileRepository);

        var result = await useCase.ExecuteAsync(profile.Id);

        Assert.Equal(profile.Id, result.Id);
        Assert.Equal(profile.SelfPerceptionFocus, result.SelfPerceptionFocus);
        Assert.Equal(profile.DesiredInsight, result.DesiredInsight);
    }

    [Fact]
    public async Task GetList_ShouldReturnProfilesOrderedByNewestFirst()
    {
        var olderProfile = CreatePersonalProfile();
        var newerProfile = CreatePersonalProfile();

        var personalProfileRepository = new InMemoryPersonalProfileRepository(olderProfile, newerProfile);
        var useCase = new GetPersonalProfilesUseCase(personalProfileRepository);

        var result = await useCase.ExecuteAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(newerProfile.Id, result[0].Id);
        Assert.Equal(olderProfile.Id, result[1].Id);
        Assert.Equal(newerProfile.BirthPlace, result[0].BirthPlace);
        Assert.Equal(newerProfile.SelfPerceptionFocus, result[0].SelfPerceptionFocus);
    }

    [Fact]
    public async Task Update_ShouldModifyExistingProfileAndPersistChanges()
    {
        var profile = CreatePersonalProfile();
        var personalProfileRepository = new InMemoryPersonalProfileRepository(profile);
        var useCase = new UpdatePersonalProfileUseCase(personalProfileRepository);

        var result = await useCase.ExecuteAsync(profile.Id, CreateUpdateRequest());

        Assert.Equal("Lucia del Mar", result.FullName);
        Assert.Equal("2000-02-02", result.BirthDate);
        Assert.Equal("14:30", result.BirthTime);
        Assert.Equal("Valparaiso, Chile", result.BirthPlace);
        Assert.Equal("Estoy aprendiendo a confiar más en mi intuición.", result.SelfDescription);
        Assert.Equal(1, personalProfileRepository.SaveChangesCalls);
    }

    [Fact]
    public async Task Create_ShouldReject_WhenSavedChartAlreadyHasLinkedProfile()
    {
        var savedChart = CreateSavedChart();
        var existingProfile = CreatePersonalProfile(savedChartId: savedChart.Id);
        var personalProfileRepository = new InMemoryPersonalProfileRepository(existingProfile);
        var savedChartRepository = new InMemorySavedChartRepository(savedChart);
        var useCase = new CreatePersonalProfileUseCase(personalProfileRepository, savedChartRepository);

        var exception = await Assert.ThrowsAsync<PersonalProfileIntegrityException>(
            () => useCase.ExecuteAsync(CreateRequest(savedChart.Id)));

        Assert.Contains("ya tiene un perfil personal asociado", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static CreatePersonalProfileRequest CreateRequest(Guid? savedChartId = null)
    {
        return new CreatePersonalProfileRequest
        {
            FullName = "Lucia Mar",
            BirthDate = "2000-01-01",
            BirthTime = "12:00",
            BirthPlace = "Santiago, Chile",
            Latitude = -33.4472,
            Longitude = -70.6736,
            TimezoneOffsetMinutes = -240,
            SelfPerceptionFocus = "Mi sensibilidad y la forma en que percibo todo",
            CurrentChallenge = "Me cuesta sostener claridad emocional cuando todo cambia",
            DesiredInsight = "Quiero entender mejor cómo confiar en mi intuición",
            SelfDescription = "Soy alguien sensible, intensa y muy intuitiva.",
            SavedChartId = savedChartId
        };
    }

    private static UpdatePersonalProfileRequest CreateUpdateRequest()
    {
        return new UpdatePersonalProfileRequest
        {
            FullName = "Lucia del Mar",
            BirthDate = "2000-02-02",
            BirthTime = "14:30",
            BirthPlace = "Valparaiso, Chile",
            Latitude = -33.0472,
            Longitude = -71.6127,
            TimezoneOffsetMinutes = -180,
            SelfPerceptionFocus = "Hoy me define mucho mi necesidad de autenticidad",
            CurrentChallenge = "Me cuesta poner límites sin sentir culpa",
            DesiredInsight = "Quiero comprender mejor mis patrones afectivos",
            SelfDescription = "Estoy aprendiendo a confiar más en mi intuición."
        };
    }

    private static PersonalProfile CreatePersonalProfile(Guid? savedChartId = null)
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
            calculatedChartJson: "{}");
    }

    private sealed class InMemoryPersonalProfileRepository : IPersonalProfileRepository
    {
        public InMemoryPersonalProfileRepository(params PersonalProfile[] profiles)
        {
            StoredProfiles = profiles.ToList();
        }

        public List<PersonalProfile> StoredProfiles { get; }

        public int SaveChangesCalls { get; private set; }

        public Task<PersonalProfile> AddAsync(PersonalProfile personalProfile, CancellationToken cancellationToken = default)
        {
            StoredProfiles.Add(personalProfile);
            return Task.FromResult(personalProfile);
        }

        public Task<IReadOnlyList<PersonalProfile>> GetListAsync(Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<PersonalProfile> profiles = StoredProfiles
                .Where(profile => !ownerUserId.HasValue || profile.UserId == ownerUserId.Value)
                .OrderByDescending(profile => profile.CreatedAtUtc)
                .ToList();

            return Task.FromResult(profiles);
        }

        public Task<PersonalProfile?> GetByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            var profile = StoredProfiles.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(FilterByOwner(profile, ownerUserId));
        }

        public Task<PersonalProfile?> GetTrackedByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            var profile = StoredProfiles.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(FilterByOwner(profile, ownerUserId));
        }

        public Task<PersonalProfile?> GetBySavedChartIdAsync(Guid savedChartId, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            var profile = StoredProfiles.FirstOrDefault(x => x.SavedChartId == savedChartId);
            return Task.FromResult(FilterByOwner(profile, ownerUserId));
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalls++;
            return Task.CompletedTask;
        }

        private static PersonalProfile? FilterByOwner(PersonalProfile? profile, Guid? ownerUserId)
        {
            if (profile is null)
            {
                return null;
            }

            if (ownerUserId.HasValue && profile.UserId != ownerUserId.Value)
            {
                return null;
            }

            return profile;
        }
    }

    private sealed class InMemorySavedChartRepository : ISavedChartRepository
    {
        public InMemorySavedChartRepository(params SavedChart[] charts)
        {
            StoredCharts = charts.ToList();
        }

        public List<SavedChart> StoredCharts { get; }

        public Task<SavedChart> AddAsync(SavedChart savedChart, CancellationToken cancellationToken = default)
        {
            StoredCharts.Add(savedChart);
            return Task.FromResult(savedChart);
        }

        public Task<SavedChart?> GetByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            var chart = StoredCharts.FirstOrDefault(x => x.Id == id);

            if (chart is null)
            {
                return Task.FromResult<SavedChart?>(null);
            }

            if (ownerUserId.HasValue && chart.UserId != ownerUserId.Value)
            {
                return Task.FromResult<SavedChart?>(null);
            }

            return Task.FromResult<SavedChart?>(chart);
        }

        public Task<IReadOnlyList<SavedChartListItemDto>> GetListItemsAsync(Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<SavedChartListItemDto>>([]);
        }
    }
}
