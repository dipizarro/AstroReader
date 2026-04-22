using AstroReader.Application.Interpretations.Premium;
using AstroReader.Domain.Entities;
using AstroReader.Domain.Enums;
using AstroReader.Domain.ValueObjects;
using Xunit;

namespace AstroReader.Application.Tests;

public sealed class PremiumInterpretationProfileNarrativeTests
{
    [Fact]
    public void Compose_ShouldIncludeSelfPerceptionContrastProfile_WhenReaderProfileIsAvailable()
    {
        var composer = new PremiumInterpretationComposer();
        var analyzer = new PremiumInterpretationAnalyzer();
        var context = CreateContextWithReaderProfile();

        var analysis = analyzer.Analyze(context);
        var composition = composer.Compose(context, analysis);

        var block = Assert.Single(composition.Profiles, x => x.Key == "self-perception-contrast");

        Assert.Equal("Tu carta y cómo hoy te percibes", block.Title);
        Assert.Contains("coincidencia", block.Summary, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("contraste", block.Summary, StringComparison.OrdinalIgnoreCase);
    }

    private static PremiumInterpretationContext CreateContextWithReaderProfile()
    {
        return new PremiumInterpretationContext
        {
            Chart = new NatalChart(
                new BirthData(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc), "0"),
                new GeoLocation(0, 0),
                [],
                []),
            ReaderProfile = new PremiumReaderProfileContext
            {
                ProfileId = Guid.NewGuid(),
                FullName = "Lucia Mar",
                DisplayName = "Lucia",
                SelfPerceptionFocus = "Mi sensibilidad y la forma en que percibo todo",
                CurrentChallenge = "Me cuesta sostener claridad emocional cuando todo cambia",
                DesiredInsight = "Quiero entender mejor cómo confiar en mi intuición",
                SelfDescription = "Soy alguien sensible, intensa y muy intuitiva."
            },
            Coverage = new PremiumInterpretationCoverageAssessment(
                CoveredEntries: [],
                MissingEntries: []),
            SunSign = ZodiacSign.Cancer,
            MoonSign = ZodiacSign.Pisces,
            AscendantSign = ZodiacSign.Libra,
            MercurySign = ZodiacSign.Cancer,
            VenusSign = ZodiacSign.Libra,
            MarsSign = ZodiacSign.Scorpio,
            Sun = new SunInterpretationEntry
            {
                Summary = "Base solar",
                Keywords = ["sensibilidad", "identidad", "cuidado"],
                IdentityStyle = "Identidad sensible y protectora.",
                GrowthPath = "Aprender a confiar en tu percepción emocional."
            },
            Moon = new MoonInterpretationEntry
            {
                Summary = "Base lunar",
                Keywords = ["sensibilidad", "intuición", "emocion"],
                EmotionalStyle = "Emoción profunda e intuitiva.",
                EmotionalNeeds = "Necesitas claridad emocional.",
                SecurityNeeds = "Buscas contención y calma."
            },
            Ascendant = new AscendantInterpretationEntry
            {
                Summary = "Base ascendente",
                Keywords = ["vinculo", "armonía"],
                OuterStyle = "Presencia amable.",
                SocialStyle = "Estilo social diplomático.",
                FirstImpression = "Primera impresión delicada."
            },
            Mercury = new MercuryInterpretationEntry
            {
                Summary = "Base mercurial",
                Keywords = ["claridad", "percepcion"],
                ThinkingStyle = "Pensamiento sensible.",
                CommunicationStyle = "Comunicación empática.",
                LearningStyle = "Aprendizaje introspectivo."
            },
            Venus = new VenusInterpretationEntry
            {
                Summary = "Base venusina",
                Keywords = ["vinculo", "cercania"],
                RelationalStyle = "Te vinculas desde la delicadeza.",
                AttractionStyle = "Buscas armonía.",
                AffectiveNeeds = "Necesitas reciprocidad."
            },
            Mars = new MarsInterpretationEntry
            {
                Summary = "Base marciana",
                Keywords = ["intensidad", "transformacion"],
                ActionStyle = "Acción intensa y profunda.",
                DesireStyle = "Deseo enfocado.",
                ConflictStyle = "Conflicto vivido con intensidad."
            }
        };
    }
}
