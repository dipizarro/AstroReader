using System.Text.RegularExpressions;
using AstroReader.Application.Charts.DTOs;
using AstroReader.Application.Interpretations.Premium;
using AstroReader.Domain.Enums;
using Xunit;

namespace AstroReader.Application.Tests;

public sealed partial class PremiumInterpretationNarrativeRegressionTests
{
    private const double MaximumBlockOverlap = 0.52d;
    private const double MaximumClosingOverlap = 0.34d;
    private const int LongPhraseWordLength = 8;
    private const int ClosingPhraseWordLength = 6;

    [Fact]
    public void CompleteReading_ShouldNotRepeatExactSentencesAcrossMainSections()
    {
        var interpretation = CreateCompleteInterpretation();
        var sections = GetNarrativeSections(interpretation);

        var duplicatedSentences = sections
            .SelectMany(section => SplitSentences(section.Text)
                .Select(sentence => new NarrativeSentence(section.Name, NormalizeSentence(sentence))))
            .Where(sentence => sentence.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length >= 5)
            .GroupBy(sentence => sentence.Text, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Select(sentence => sentence.SectionName).Distinct().Count() > 1)
            .Select(group => group.Key)
            .ToList();

        Assert.Empty(duplicatedSentences);
    }

    [Fact]
    public void CompleteReading_ShouldKeepLongPhraseReuseBelowRegressionThreshold()
    {
        var interpretation = CreateCompleteInterpretation();
        var sections = GetNarrativeSections(interpretation);

        foreach (var first in sections)
        {
            foreach (var second in sections.Where(section => string.CompareOrdinal(section.Name, first.Name) > 0))
            {
                var repeatedPhrases = GetWordShingles(first.Text, LongPhraseWordLength)
                    .Intersect(GetWordShingles(second.Text, LongPhraseWordLength), StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var overlap = CalculateMeaningfulWordOverlap(first.Text, second.Text);

                Assert.True(
                    repeatedPhrases.Count == 0,
                    $"Sections '{first.Name}' and '{second.Name}' repeat long phrases: {string.Join(" | ", repeatedPhrases.Take(3))}");
                Assert.True(
                    overlap <= MaximumBlockOverlap,
                    $"Sections '{first.Name}' and '{second.Name}' overlap too much ({overlap:P0}).");
            }
        }
    }

    [Fact]
    public void Closing_ShouldStayIndependentFromPreviousSectionsAndCatalogHooks()
    {
        var interpretation = CreateCompleteInterpretation();
        var previousText = string.Join(
            " ",
            GetNarrativeSections(interpretation)
                .Where(section => section.Name != "closing")
                .Select(section => section.Text));

        var closing = interpretation.Closing;
        var repeatedClosingPhrases = GetWordShingles(closing, ClosingPhraseWordLength)
            .Intersect(GetWordShingles(previousText, ClosingPhraseWordLength), StringComparer.OrdinalIgnoreCase)
            .ToList();
        var closingOverlap = CalculateMeaningfulWordOverlap(closing, previousText);

        Assert.DoesNotContain("Elegir calma material", closing, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Volver al cuerpo antes de decidir", closing, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Nombrar el deseo antes de actuar", closing, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(repeatedClosingPhrases);
        Assert.True(
            closingOverlap <= MaximumClosingOverlap,
            $"Closing overlaps too much with previous sections ({closingOverlap:P0}).");
    }

    private static ChartInterpretation CreateCompleteInterpretation()
    {
        var provider = new InMemoryPremiumInterpretationCatalogProvider(CreateCompleteCatalog());
        var resolver = new PremiumInterpretationContextResolver(provider);
        var analyzer = new PremiumInterpretationAnalyzer();
        var composer = new PremiumInterpretationComposer();
        var useCase = new PremiumInterpretationPreviewUseCase(resolver, analyzer, composer);

        return useCase.Execute(new PremiumInterpretationPreviewRequest
        {
            Sun = "Taurus",
            Moon = "Leo",
            Ascendant = "Libra",
            Mercury = "Aries",
            Venus = "Taurus",
            Mars = "Scorpio"
        }).Interpretation;
    }

    private static IReadOnlyList<NarrativeSection> GetNarrativeSections(ChartInterpretation interpretation)
    {
        return
        [
            new NarrativeSection("hook", interpretation.Hook),
            ToNarrativeSection("energyCore", interpretation.EnergyCore),
            ToNarrativeSection("core", interpretation.Core),
            ToNarrativeSection("personalDynamics", interpretation.PersonalDynamics),
            ToNarrativeSection("essentialSummary", interpretation.EssentialSummary),
            new NarrativeSection("closing", interpretation.Closing)
        ];
    }

    private static NarrativeSection ToNarrativeSection(string name, InterpretationContentBlock block)
    {
        var text = string.Join(
            " ",
            new[] { block.MainText }
                .Concat(block.SubBlocks.Select(subBlock => subBlock.Text))
                .Where(x => !string.IsNullOrWhiteSpace(x)));

        return new NarrativeSection(name, text);
    }

    private static IReadOnlyList<string> SplitSentences(string text)
    {
        return SentenceSplitter()
            .Split(text)
            .Select(NormalizeSentence)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    private static IReadOnlySet<string> GetWordShingles(string text, int wordLength)
    {
        var words = GetMeaningfulWords(text);

        if (words.Count < wordLength)
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        return Enumerable.Range(0, words.Count - wordLength + 1)
            .Select(index => string.Join(" ", words.Skip(index).Take(wordLength)))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static double CalculateMeaningfulWordOverlap(string first, string second)
    {
        var firstWords = GetMeaningfulWords(first).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var secondWords = GetMeaningfulWords(second).ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (firstWords.Count == 0 || secondWords.Count == 0)
        {
            return 0d;
        }

        var sharedCount = firstWords.Intersect(secondWords, StringComparer.OrdinalIgnoreCase).Count();
        var smallerSectionCount = Math.Min(firstWords.Count, secondWords.Count);

        return (double)sharedCount / smallerSectionCount;
    }

    private static IReadOnlyList<string> GetMeaningfulWords(string text)
    {
        return WordMatcher()
            .Matches(text.ToLowerInvariant())
            .Select(match => match.Value)
            .Where(word => word.Length > 3)
            .Where(word => !StopWords.Contains(word))
            .ToList();
    }

    private static string NormalizeSentence(string sentence)
    {
        return WhitespaceNormalizer()
            .Replace(sentence.ToLowerInvariant().Trim(), " ")
            .Trim(' ', '.', ',', ';', ':', '!', '?');
    }

    private static PremiumInterpretationCatalog CreateCompleteCatalog()
    {
        return new PremiumInterpretationCatalog
        {
            Version = "narrative-regression-test",
            Planets = new PremiumInterpretationPlanetCatalog
            {
                Sun = new ZodiacInterpretationSet<SunInterpretationEntry>
                {
                    Taurus = new SunInterpretationEntry
                    {
                        Summary = "Tu identidad busca construir valor con paciencia, presencia y sentido de continuidad.",
                        Keywords = ["estabilidad", "cuerpo", "constancia"],
                        Strengths = ["sostener procesos reales"],
                        Challenges = ["confundir seguridad con inmovilidad"],
                        IntegrationHooks = ["Elegir calma material antes de responder desde la urgencia."],
                        IdentityStyle = "Te afirmas cuando puedes avanzar con ritmo propio y convertir intención en algo tangible.",
                        GrowthPath = "Elegir calma material sin cerrarte al cambio que ya está pidiendo espacio."
                    }
                },
                Moon = new ZodiacInterpretationSet<MoonInterpretationEntry>
                {
                    Leo = new MoonInterpretationEntry
                    {
                        Summary = "Tu mundo emocional necesita calidez, expresión honesta y reconocimiento afectivo.",
                        Keywords = ["calidez", "expresión", "orgullo"],
                        Strengths = ["dar ánimo y presencia emocional"],
                        Challenges = ["sobreactuar cuando falta validación"],
                        IntegrationHooks = ["Dar lugar a la emoción sin convertir cada herida en escenario."],
                        EmotionalStyle = "Sientes con intensidad visible y necesitas que lo importante tenga corazón.",
                        EmotionalNeeds = "Buscas vínculos donde tu entusiasmo sea recibido sin tener que exagerarlo.",
                        SecurityNeeds = "Te estabiliza sentir aprecio sincero, juego y espacio para mostrar vulnerabilidad."
                    }
                },
                Ascendant = new ZodiacInterpretationSet<AscendantInterpretationEntry>
                {
                    Libra = new AscendantInterpretationEntry
                    {
                        Summary = "Tu presencia entra buscando proporción, belleza relacional y lectura fina del ambiente.",
                        Keywords = ["armonía", "criterio", "encuentro"],
                        Strengths = ["crear puentes sin imponer"],
                        Challenges = ["postergar decisiones por cuidar la forma"],
                        IntegrationHooks = ["Permitir que la amabilidad también tenga borde."],
                        OuterStyle = "Sueles presentarte con tacto, elegancia social y atención a cómo se siente el intercambio.",
                        SocialStyle = "Tu manera de acercarte prioriza acuerdos, tono justo y cuidado por la reciprocidad.",
                        FirstImpression = "Puedes dar una primera impresión amable, medida y naturalmente diplomática."
                    }
                },
                Mercury = new ZodiacInterpretationSet<MercuryInterpretationEntry>
                {
                    Aries = new MercuryInterpretationEntry
                    {
                        Summary = "Tu mente necesita entrar rápido en la idea y probarla sin demasiada espera.",
                        Keywords = ["rapidez", "franqueza", "iniciativa"],
                        Strengths = ["decir lo esencial sin rodeos"],
                        Challenges = ["responder antes de escuchar todo"],
                        IntegrationHooks = ["Hacer una pausa breve antes de convertir la primera idea en conclusión."],
                        ThinkingStyle = "Piensas mejor cuando hay movimiento, desafío y permiso para iniciar.",
                        CommunicationStyle = "Tu palabra tiende a ser frontal, directa y más valiente que decorativa.",
                        LearningStyle = "Aprendes probando, corrigiendo en marcha y entrando al problema de frente."
                    }
                },
                Venus = new ZodiacInterpretationSet<VenusInterpretationEntry>
                {
                    Taurus = new VenusInterpretationEntry
                    {
                        Summary = "Tu forma de vincularte valora consistencia, sensualidad tranquila y cuidado concreto.",
                        Keywords = ["placer", "lealtad", "cuidado"],
                        Strengths = ["ofrecer afecto estable"],
                        Challenges = ["apegarte a lo cómodo aunque ya no nutra"],
                        IntegrationHooks = ["Distinguir permanencia de costumbre."],
                        RelationalStyle = "En el vínculo buscas presencia confiable, gestos simples y una lealtad que se demuestre.",
                        AttractionStyle = "Te atrae lo natural, lo honesto y lo que transmite calma sin tener que impresionar.",
                        AffectiveNeeds = "Necesitas afecto constante, contacto con lo sensorial y seguridad emocional encarnada."
                    }
                },
                Mars = new ZodiacInterpretationSet<MarsInterpretationEntry>
                {
                    Scorpio = new MarsInterpretationEntry
                    {
                        Summary = "Tu impulso actúa con profundidad, foco y capacidad de atravesar lo incómodo.",
                        Keywords = ["intensidad", "estrategia", "deseo"],
                        Strengths = ["ir al fondo de lo difícil"],
                        Challenges = ["guardar tensión hasta que se vuelva demasiado fuerte"],
                        IntegrationHooks = ["Nombrar el deseo antes de actuar desde control o sospecha."],
                        ActionStyle = "Actúas mejor cuando hay propósito real y una razón interna que justifique la intensidad.",
                        DesireStyle = "Tu deseo no suele ser liviano: busca profundidad, verdad y transformación.",
                        ConflictStyle = "En conflicto puedes observar mucho antes de moverte, pero cuando decides vas al núcleo."
                    }
                }
            }
        };
    }

    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "ahora",
        "algo",
        "como",
        "cuando",
        "desde",
        "donde",
        "esta",
        "este",
        "esto",
        "hacia",
        "para",
        "pero",
        "puede",
        "puedes",
        "porque",
        "sobre",
        "tambien",
        "tiene",
        "tienes",
        "todo",
        "toda",
        "entre"
    };

    private sealed record NarrativeSection(string Name, string Text);

    private sealed record NarrativeSentence(string SectionName, string Text);

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

    [GeneratedRegex(@"(?<=[\.\!\?])\s+")]
    private static partial Regex SentenceSplitter();

    [GeneratedRegex(@"\p{L}+")]
    private static partial Regex WordMatcher();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceNormalizer();
}
