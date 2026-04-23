using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.Charts.UseCases;
using AstroReader.Application.Interpretations.Premium;
using AstroReader.Application.PersonalProfiles.Exceptions;
using AstroReader.Application.PersonalProfiles.Interfaces;
using AstroReader.AstroEngine.Contracts;
using AstroReader.Domain.Entities;
using AstroReader.Domain.Enums;
using Xunit;

namespace AstroReader.Application.Tests;

public sealed class CalculateNatalChartUseCaseProfileIntegrityTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldUseProfileContext_WhenProfileMatchesNatalRequest()
    {
        var personalProfile = CreatePersonalProfile();
        var useCase = CreateUseCase(personalProfile);
        var request = CreateRequest(personalProfile.Id);

        var response = await useCase.ExecuteAsync(request);

        Assert.Contains("Lucia", response.Interpretation.Hook, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(
            response.Interpretation.Profiles,
            profile => string.Equals(profile.Key, "self-perception-contrast", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRejectProfileContext_WhenProfileDoesNotMatchNatalRequest()
    {
        var personalProfile = CreatePersonalProfile();
        var useCase = CreateUseCase(personalProfile);
        var request = CreateRequest(personalProfile.Id) with
        {
            Latitude = -33.4479,
            TimezoneOffsetMinutes = -180
        };

        var exception = await Assert.ThrowsAsync<PersonalProfileIntegrityException>(() => useCase.ExecuteAsync(request));

        Assert.Contains("personalProfileId", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("latitude", exception.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("timezoneOffsetMinutes", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldKeepContrastAndPriorityProfiles_WhenInterpretationFallsBackWithReaderProfile()
    {
        var personalProfile = CreatePersonalProfile();
        var useCase = CreateUseCase(personalProfile, CreateEmptyCatalog());
        var request = CreateRequest(personalProfile.Id);

        var response = await useCase.ExecuteAsync(request);

        Assert.Equal("fallback", response.Interpretation.Coverage.CoverageStatus);
        Assert.False(response.Interpretation.Coverage.IsPremiumResult);
        Assert.True(response.Interpretation.Coverage.IsFallback);
        Assert.Equal(2, response.Interpretation.Profiles.Count);

        var contrastProfile = Assert.Single(
            response.Interpretation.Profiles,
            profile => string.Equals(profile.Key, "self-perception-contrast", StringComparison.OrdinalIgnoreCase));
        Assert.Contains("lectura base", contrastProfile.Summary, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Sol en Taurus, Luna en Leo y Ascendente en Libra", contrastProfile.Summary);
        Assert.Contains("claridad emocional", contrastProfile.Summary, StringComparison.OrdinalIgnoreCase);

        var priorityProfile = Assert.Single(
            response.Interpretation.Profiles,
            profile => string.Equals(profile.Key, "reading-priority", StringComparison.OrdinalIgnoreCase));
        Assert.Contains("todavía no desarrolla toda la capa premium", priorityProfile.Summary, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Lo esencial de tu carta", priorityProfile.Summary);
    }

    private static CalculateNatalChartUseCase CreateUseCase(
        PersonalProfile personalProfile,
        PremiumInterpretationCatalog? catalog = null)
    {
        var provider = new InMemoryPremiumInterpretationCatalogProvider(catalog ?? CreateCompleteCatalog());
        var resolver = new PremiumInterpretationContextResolver(provider);
        var analyzer = new PremiumInterpretationAnalyzer();
        var composer = new PremiumInterpretationComposer();

        return new CalculateNatalChartUseCase(
            new FixedAstroCalculationEngine(),
            new SinglePersonalProfileRepository(personalProfile),
            resolver,
            analyzer,
            composer);
    }

    private static CalculateChartRequest CreateRequest(Guid personalProfileId)
    {
        return new CalculateChartRequest
        {
            BirthDate = "2000-01-01",
            BirthTime = "12:00",
            Latitude = -33.4472,
            Longitude = -70.6736,
            TimezoneOffsetMinutes = -240,
            PlaceName = "Santiago, Chile",
            PersonalProfileId = personalProfileId
        };
    }

    private static PremiumInterpretationCatalog CreateEmptyCatalog()
    {
        return new PremiumInterpretationCatalog
        {
            Version = "test",
            Planets = new PremiumInterpretationPlanetCatalog
            {
                Sun = new ZodiacInterpretationSet<SunInterpretationEntry>(),
                Moon = new ZodiacInterpretationSet<MoonInterpretationEntry>(),
                Ascendant = new ZodiacInterpretationSet<AscendantInterpretationEntry>(),
                Mercury = new ZodiacInterpretationSet<MercuryInterpretationEntry>(),
                Venus = new ZodiacInterpretationSet<VenusInterpretationEntry>(),
                Mars = new ZodiacInterpretationSet<MarsInterpretationEntry>()
            }
        };
    }

    private static PersonalProfile CreatePersonalProfile()
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
            selfDescription: "Soy alguien sensible, intensa y muy intuitiva.");
    }

    private static PremiumInterpretationCatalog CreateCompleteCatalog()
    {
        return new PremiumInterpretationCatalog
        {
            Version = "test",
            Planets = new PremiumInterpretationPlanetCatalog
            {
                Sun = new ZodiacInterpretationSet<SunInterpretationEntry>
                {
                    Taurus = CreateSunEntry()
                },
                Moon = new ZodiacInterpretationSet<MoonInterpretationEntry>
                {
                    Leo = CreateMoonEntry()
                },
                Ascendant = new ZodiacInterpretationSet<AscendantInterpretationEntry>
                {
                    Libra = CreateAscendantEntry()
                },
                Mercury = new ZodiacInterpretationSet<MercuryInterpretationEntry>
                {
                    Aries = CreateMercuryEntry()
                },
                Venus = new ZodiacInterpretationSet<VenusInterpretationEntry>
                {
                    Taurus = CreateVenusEntry()
                },
                Mars = new ZodiacInterpretationSet<MarsInterpretationEntry>
                {
                    Scorpio = CreateMarsEntry()
                }
            }
        };
    }

    private static SunInterpretationEntry CreateSunEntry()
    {
        return new SunInterpretationEntry
        {
            Summary = "Base solar.",
            Keywords = ["estabilidad", "constancia"],
            Strengths = ["fortaleza solar"],
            Challenges = ["reto solar"],
            IntegrationHooks = ["hook solar"],
            IdentityStyle = "Identidad estable.",
            GrowthPath = "Crecimiento flexible."
        };
    }

    private static MoonInterpretationEntry CreateMoonEntry()
    {
        return new MoonInterpretationEntry
        {
            Summary = "Base lunar.",
            Keywords = ["calidez", "expresión"],
            Strengths = ["fortaleza lunar"],
            Challenges = ["reto lunar"],
            IntegrationHooks = ["hook lunar"],
            EmotionalStyle = "Emoción visible.",
            EmotionalNeeds = "Necesidad emocional clara.",
            SecurityNeeds = "Seguridad afectiva."
        };
    }

    private static AscendantInterpretationEntry CreateAscendantEntry()
    {
        return new AscendantInterpretationEntry
        {
            Summary = "Base ascendente.",
            Keywords = ["armonía", "vínculo"],
            Strengths = ["fortaleza ascendente"],
            Challenges = ["reto ascendente"],
            IntegrationHooks = ["hook ascendente"],
            OuterStyle = "Presencia amable.",
            SocialStyle = "Estilo social diplomático.",
            FirstImpression = "Primera impresión cuidada."
        };
    }

    private static MercuryInterpretationEntry CreateMercuryEntry()
    {
        return new MercuryInterpretationEntry
        {
            Summary = "Base mercurial.",
            Keywords = ["claridad", "agilidad"],
            Strengths = ["fortaleza mercurio"],
            Challenges = ["reto mercurio"],
            IntegrationHooks = ["hook mercurio"],
            ThinkingStyle = "Pensamiento directo.",
            CommunicationStyle = "Comunicación franca.",
            LearningStyle = "Aprendizaje activo."
        };
    }

    private static VenusInterpretationEntry CreateVenusEntry()
    {
        return new VenusInterpretationEntry
        {
            Summary = "Base venusina.",
            Keywords = ["placer", "lealtad"],
            Strengths = ["fortaleza venus"],
            Challenges = ["reto venus"],
            IntegrationHooks = ["hook venus"],
            RelationalStyle = "Vínculo estable.",
            AttractionStyle = "Atracción serena.",
            AffectiveNeeds = "Necesidad afectiva clara."
        };
    }

    private static MarsInterpretationEntry CreateMarsEntry()
    {
        return new MarsInterpretationEntry
        {
            Summary = "Base marciana.",
            Keywords = ["intensidad", "determinación"],
            Strengths = ["fortaleza mars"],
            Challenges = ["reto mars"],
            IntegrationHooks = ["hook mars"],
            ActionStyle = "Acción enfocada.",
            DesireStyle = "Deseo intenso.",
            ConflictStyle = "Conflicto estratégico."
        };
    }

    private static AsteroidalData CreatePlanetData(ZodiacSign sign)
    {
        const double signDegree = 15d;

        return new AsteroidalData(
            AbsoluteDegree: ((int)sign * 30d) + signDegree,
            ZodiacSignIndex: (int)sign,
            SignDegree: signDegree,
            IsRetrograde: false);
    }

    private static double GetAbsoluteDegree(ZodiacSign sign)
    {
        return ((int)sign * 30d) + 15d;
    }

    private sealed class FixedAstroCalculationEngine : IAstroCalculationEngine
    {
        public AstroCalculationResult Calculate(AstroCalculationRequest request)
        {
            return new AstroCalculationResult
            {
                AscendantDegree = GetAbsoluteDegree(ZodiacSign.Libra),
                PlanetaryPositions = new Dictionary<int, AsteroidalData>
                {
                    [0] = CreatePlanetData(ZodiacSign.Taurus),
                    [1] = CreatePlanetData(ZodiacSign.Leo),
                    [2] = CreatePlanetData(ZodiacSign.Aries),
                    [3] = CreatePlanetData(ZodiacSign.Taurus),
                    [4] = CreatePlanetData(ZodiacSign.Scorpio)
                },
                Houses = Enumerable.Range(1, 12)
                    .ToDictionary(
                        houseNumber => houseNumber,
                        houseNumber => GetAbsoluteDegree((ZodiacSign)(((int)ZodiacSign.Libra + houseNumber - 1) % 12)))
            };
        }
    }

    private sealed class InMemoryPremiumInterpretationCatalogProvider : IPremiumInterpretationCatalogProvider
    {
        private readonly PremiumInterpretationCatalog _catalog;

        public InMemoryPremiumInterpretationCatalogProvider(PremiumInterpretationCatalog catalog)
        {
            _catalog = catalog;
        }

        public PremiumInterpretationCatalog GetCatalog() => _catalog;

        public InterpretationEntry GetEntry(PremiumInterpretationPosition position, ZodiacSign sign) =>
            _catalog.GetEntry(position, sign);

        public TEntry GetEntry<TEntry>(PremiumInterpretationPosition position, ZodiacSign sign)
            where TEntry : InterpretationEntry =>
            _catalog.GetEntry<TEntry>(position, sign);
    }

    private sealed class SinglePersonalProfileRepository : IPersonalProfileRepository
    {
        private readonly PersonalProfile _personalProfile;

        public SinglePersonalProfileRepository(PersonalProfile personalProfile)
        {
            _personalProfile = personalProfile;
        }

        public Task<PersonalProfile> AddAsync(PersonalProfile personalProfile, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<PersonalProfile>> GetListAsync(Guid? ownerUserId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<PersonalProfile>>([_personalProfile]);
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
            return Task.FromResult<PersonalProfile?>(null);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
