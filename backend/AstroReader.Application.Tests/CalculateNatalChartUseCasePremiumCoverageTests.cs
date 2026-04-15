using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.Charts.UseCases;
using AstroReader.Application.Interpretations.Premium;
using AstroReader.AstroEngine.Contracts;
using AstroReader.Domain.Enums;
using Xunit;

namespace AstroReader.Application.Tests;

public sealed class CalculateNatalChartUseCasePremiumCoverageTests
{
    [Fact]
    public void Execute_ShouldReturnCompleteCoverage_WhenCatalogCoversAllRequiredEntries()
    {
        var useCase = CreateUseCase(CreateCompleteCatalog());

        var response = useCase.Execute(CreateRequest());

        Assert.Equal("complete", response.Interpretation.Coverage.CoverageStatus);
        Assert.True(response.Interpretation.Coverage.IsPremiumResult);
        Assert.False(response.Interpretation.Coverage.IsFallback);
        Assert.Equal(6, response.Interpretation.Coverage.CoveredEntries.Count);
        Assert.Empty(response.Interpretation.Coverage.MissingEntries);
        Assert.Equal(4, response.Interpretation.Coverage.ComposedBlocks.Count);
        Assert.Contains("energyCore", response.Interpretation.Coverage.ComposedBlocks);
        Assert.Contains("core", response.Interpretation.Coverage.ComposedBlocks);
        Assert.Contains("personalDynamics", response.Interpretation.Coverage.ComposedBlocks);
        Assert.Contains("essentialSummary", response.Interpretation.Coverage.ComposedBlocks);
    }

    [Fact]
    public void Execute_ShouldReturnPartialCoverage_WhenCoreEntriesAreAvailableButPersonalDynamicsAreMissing()
    {
        var useCase = CreateUseCase(CreateCorePartialCatalog());

        var response = useCase.Execute(CreateRequest());

        Assert.Equal("partial", response.Interpretation.Coverage.CoverageStatus);
        Assert.True(response.Interpretation.Coverage.IsPremiumResult);
        Assert.False(response.Interpretation.Coverage.IsFallback);
        Assert.Equal(3, response.Interpretation.Coverage.CoveredEntries.Count);
        Assert.Equal(3, response.Interpretation.Coverage.MissingEntries.Count);
        Assert.Contains("sun.taurus", response.Interpretation.Coverage.CoveredEntries);
        Assert.Contains("moon.leo", response.Interpretation.Coverage.CoveredEntries);
        Assert.Contains("ascendant.libra", response.Interpretation.Coverage.CoveredEntries);
        Assert.Contains("mercury.aries", response.Interpretation.Coverage.MissingEntries);
        Assert.Contains("venus.taurus", response.Interpretation.Coverage.MissingEntries);
        Assert.Contains("mars.scorpio", response.Interpretation.Coverage.MissingEntries);
        Assert.Contains("energyCore", response.Interpretation.Coverage.ComposedBlocks);
        Assert.Contains("core", response.Interpretation.Coverage.ComposedBlocks);
        Assert.Contains("essentialSummary", response.Interpretation.Coverage.ComposedBlocks);
        Assert.DoesNotContain("personalDynamics", response.Interpretation.Coverage.ComposedBlocks);
    }

    [Fact]
    public void Execute_ShouldReturnFallbackCoverage_WhenCatalogHasNoUsefulEntries()
    {
        var useCase = CreateUseCase(CreateEmptyCatalog());

        var response = useCase.Execute(CreateRequest());

        Assert.Equal("fallback", response.Interpretation.Coverage.CoverageStatus);
        Assert.False(response.Interpretation.Coverage.IsPremiumResult);
        Assert.True(response.Interpretation.Coverage.IsFallback);
        Assert.Empty(response.Interpretation.Coverage.CoveredEntries);
        Assert.Equal(6, response.Interpretation.Coverage.MissingEntries.Count);
        Assert.Empty(response.Interpretation.Coverage.ComposedBlocks);
        Assert.Contains("sun.taurus", response.Interpretation.Coverage.MissingEntries);
        Assert.Contains("moon.leo", response.Interpretation.Coverage.MissingEntries);
        Assert.Contains("ascendant.libra", response.Interpretation.Coverage.MissingEntries);
    }

    private static CalculateNatalChartUseCase CreateUseCase(PremiumInterpretationCatalog catalog)
    {
        var provider = new InMemoryPremiumInterpretationCatalogProvider(catalog);
        var resolver = new PremiumInterpretationContextResolver(provider);
        var analyzer = new PremiumInterpretationAnalyzer();
        var composer = new PremiumInterpretationComposer();

        return new CalculateNatalChartUseCase(
            new FixedAstroCalculationEngine(),
            resolver,
            analyzer,
            composer);
    }

    private static CalculateChartRequest CreateRequest()
    {
        return new CalculateChartRequest
        {
            BirthDate = "2000-01-01",
            BirthTime = "12:00",
            Latitude = 0,
            Longitude = 0,
            TimezoneOffsetMinutes = 0,
            PlaceName = "Coverage Test"
        };
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

    private static PremiumInterpretationCatalog CreateCorePartialCatalog()
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
                Mercury = new ZodiacInterpretationSet<MercuryInterpretationEntry>(),
                Venus = new ZodiacInterpretationSet<VenusInterpretationEntry>(),
                Mars = new ZodiacInterpretationSet<MarsInterpretationEntry>()
            }
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
}
