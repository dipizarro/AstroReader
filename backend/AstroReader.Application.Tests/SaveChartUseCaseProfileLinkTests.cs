using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.Charts.Interfaces;
using AstroReader.Application.PersonalProfiles.Exceptions;
using AstroReader.Application.PersonalProfiles.Interfaces;
using AstroReader.Application.SavedCharts.DTOs;
using AstroReader.Application.SavedCharts.Exceptions;
using AstroReader.Application.SavedCharts.Interfaces;
using AstroReader.Application.SavedCharts.UseCases;
using AstroReader.AstroEngine.Contracts;
using AstroReader.Domain.Entities;
using Xunit;

namespace AstroReader.Application.Tests;

public sealed class SaveChartUseCaseProfileLinkTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldPersistProfileLink_WhenPersonalProfileIdIsProvided()
    {
        var personalProfile = CreatePersonalProfile();
        var chartCalculator = new FakeCalculateNatalChartUseCase(CreateCalculatedChartResponse());
        var profileRepository = new InMemoryPersonalProfileRepository(personalProfile);
        var savedChartRepository = new InMemorySavedChartRepository();
        var useCase = CreateUseCase(chartCalculator, profileRepository, savedChartRepository);

        var response = await useCase.ExecuteAsync(CreateRequest(personalProfile.Id));

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(personalProfile.Id, chartCalculator.LastRequest?.PersonalProfileId);
        Assert.Equal(response.Id, personalProfile.SavedChartId);
        Assert.Equal(personalProfile.Id, response.PersonalProfileId);
        Assert.NotNull(response.PersonalProfile);
        Assert.Equal(personalProfile.FullName, response.PersonalProfile!.FullName);
        Assert.Equal(personalProfile.DesiredInsight, response.PersonalProfile.DesiredInsight);
        Assert.Equal(1, profileRepository.SaveChangesCalls);
        Assert.Equal(response.Id, savedChartRepository.LastSavedChart?.Id);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReject_WhenProfileIsAlreadyLinkedToAnotherSavedChart()
    {
        var personalProfile = CreatePersonalProfile(savedChartId: Guid.NewGuid());
        var chartCalculator = new FakeCalculateNatalChartUseCase(CreateCalculatedChartResponse());
        var profileRepository = new InMemoryPersonalProfileRepository(personalProfile);
        var savedChartRepository = new InMemorySavedChartRepository();
        var useCase = CreateUseCase(chartCalculator, profileRepository, savedChartRepository);

        var exception = await Assert.ThrowsAsync<PersonalProfileIntegrityException>(
            () => useCase.ExecuteAsync(CreateRequest(personalProfile.Id)));

        Assert.Contains("ya está vinculado", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Null(savedChartRepository.LastSavedChart);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldBubbleProfileConsistencyErrors_FromChartCalculation()
    {
        var personalProfile = CreatePersonalProfile();
        var chartCalculator = new FakeCalculateNatalChartUseCase(
            exceptionToThrow: new PersonalProfileIntegrityException("El personalProfileId enviado no coincide con los datos natales usados para calcular la carta."));
        var profileRepository = new InMemoryPersonalProfileRepository(personalProfile);
        var savedChartRepository = new InMemorySavedChartRepository();
        var useCase = CreateUseCase(chartCalculator, profileRepository, savedChartRepository);

        var exception = await Assert.ThrowsAsync<PersonalProfileIntegrityException>(
            () => useCase.ExecuteAsync(CreateRequest(personalProfile.Id)));

        Assert.Contains("no coincide", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Null(savedChartRepository.LastSavedChart);
    }

    private static SaveChartUseCase CreateUseCase(
        FakeCalculateNatalChartUseCase chartCalculator,
        InMemoryPersonalProfileRepository profileRepository,
        InMemorySavedChartRepository savedChartRepository)
    {
        return new SaveChartUseCase(
            chartCalculator,
            profileRepository,
            new FakeAstroEngineTechnicalMetadataProvider(),
            savedChartRepository);
    }

    private static SaveChartRequest CreateRequest(Guid personalProfileId)
    {
        return new SaveChartRequest
        {
            ProfileName = "Carta natal · Santiago",
            PlaceName = "Santiago, Chile",
            BirthDate = "2000-01-01",
            BirthTime = "12:00",
            Latitude = -33.4472,
            Longitude = -70.6736,
            TimezoneOffsetMinutes = -240,
            TimezoneIana = "America/Santiago",
            PersonalProfileId = personalProfileId
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

    private static CalculateChartResponse CreateCalculatedChartResponse()
    {
        return new CalculateChartResponse
        {
            Metadata = new ChartMetadata
            {
                CalculatedForUtc = new DateTime(2000, 1, 1, 16, 0, 0, DateTimeKind.Utc),
                Latitude = -33.4472,
                Longitude = -70.6736,
                PlaceName = "Santiago, Chile"
            },
            Summary = new ChartSummary
            {
                Sun = "Taurus",
                Moon = "Leo",
                Ascendant = "Libra"
            },
            Planets =
            [
                new PlanetPositionDto { Name = "Sun", Sign = "Taurus", SignDegree = 15, AbsoluteDegree = 45, IsRetrograde = false },
                new PlanetPositionDto { Name = "Moon", Sign = "Leo", SignDegree = 15, AbsoluteDegree = 135, IsRetrograde = false }
            ],
            Houses =
            [
                new HousePositionDto { Number = 1, Sign = "Libra", AbsoluteDegree = 195 }
            ],
            Interpretation = new ChartInterpretation
            {
                Coverage = new InterpretationCoverage
                {
                    CoverageStatus = "fallback",
                    IsPremiumResult = false,
                    IsFallback = true,
                    CoveredEntries = [],
                    MissingEntries = [],
                    ComposedBlocks = []
                },
                Hook = "Hook",
                EnergyCore = new InterpretationContentBlock { Key = "energyCore", Title = "Tu energía central", MainText = "Texto", SubBlocks = [] },
                Core = new InterpretationContentBlock { Key = "core", Title = "Tu núcleo", MainText = "Texto", SubBlocks = [] },
                PersonalDynamics = new InterpretationContentBlock { Key = "personalDynamics", Title = "Tu forma de pensar, vincularte y actuar", MainText = "Texto", SubBlocks = [] },
                EssentialSummary = new InterpretationContentBlock { Key = "essentialSummary", Title = "Lo esencial de tu carta", MainText = "Texto", SubBlocks = [] },
                TensionsAndPotential = [],
                LifeAreas = [],
                Profiles = [],
                Closing = "Closing"
            }
        };
    }

    private sealed class FakeCalculateNatalChartUseCase : ICalculateNatalChartUseCase
    {
        private readonly CalculateChartResponse? _response;
        private readonly Exception? _exceptionToThrow;

        public FakeCalculateNatalChartUseCase(CalculateChartResponse? response = null, Exception? exceptionToThrow = null)
        {
            _response = response;
            _exceptionToThrow = exceptionToThrow;
        }

        public CalculateChartRequest? LastRequest { get; private set; }

        public Task<CalculateChartResponse> ExecuteAsync(CalculateChartRequest request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;

            if (_exceptionToThrow is not null)
            {
                throw _exceptionToThrow;
            }

            return Task.FromResult(_response ?? throw new InvalidOperationException("No fake chart response configured."));
        }
    }

    private sealed class InMemoryPersonalProfileRepository : IPersonalProfileRepository
    {
        private readonly PersonalProfile _personalProfile;

        public InMemoryPersonalProfileRepository(PersonalProfile personalProfile)
        {
            _personalProfile = personalProfile;
        }

        public int SaveChangesCalls { get; private set; }

        public Task<PersonalProfile> AddAsync(PersonalProfile personalProfile, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PersonalProfile?> GetByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PersonalProfile?>(_personalProfile.Id == id ? _personalProfile : null);
        }

        public Task<PersonalProfile?> GetTrackedByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PersonalProfile?>(_personalProfile.Id == id ? _personalProfile : null);
        }

        public Task<PersonalProfile?> GetBySavedChartIdAsync(Guid savedChartId, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<PersonalProfile?>(_personalProfile.SavedChartId == savedChartId ? _personalProfile : null);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalls++;
            return Task.CompletedTask;
        }
    }

    private sealed class InMemorySavedChartRepository : ISavedChartRepository
    {
        public SavedChart? LastSavedChart { get; private set; }

        public Task<SavedChart> AddAsync(SavedChart savedChart, CancellationToken cancellationToken = default)
        {
            LastSavedChart = savedChart;
            return Task.FromResult(savedChart);
        }

        public Task<SavedChart?> GetByIdAsync(Guid id, Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<SavedChart?>(LastSavedChart?.Id == id ? LastSavedChart : null);
        }

        public Task<IReadOnlyList<SavedChartListItemDto>> GetListItemsAsync(Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<SavedChartListItemDto>>([]);
        }
    }

    private sealed class FakeAstroEngineTechnicalMetadataProvider : IAstroEngineTechnicalMetadataProvider
    {
        public AstroEngineTechnicalMetadata GetCurrent()
        {
            return new AstroEngineTechnicalMetadata(
                CalculationEngine: "SwissEph",
                HouseSystemCode: "P",
                UsesRealEngine: true,
                UsesCustomEphemerisPath: false,
                WrapperVersion: "test");
        }
    }
}
