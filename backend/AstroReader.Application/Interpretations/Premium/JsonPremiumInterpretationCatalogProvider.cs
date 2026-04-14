using System.Text.Json;
using AstroReader.Domain.Enums;

namespace AstroReader.Application.Interpretations.Premium;

public sealed class JsonPremiumInterpretationCatalogProvider : IPremiumInterpretationCatalogProvider
{
    private const string CatalogRelativePath = "Interpretations/Premium/interpretations.premium.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private readonly PremiumInterpretationCatalog _catalog;

    public JsonPremiumInterpretationCatalogProvider()
    {
        var catalogPath = ResolveCatalogPath();

        if (!File.Exists(catalogPath))
        {
            throw new PremiumInterpretationCatalogException(
                $"No se encontró el catálogo premium de interpretaciones en '{catalogPath}'.");
        }

        try
        {
            var json = File.ReadAllText(catalogPath);
            _catalog = JsonSerializer.Deserialize<PremiumInterpretationCatalog>(json, JsonOptions)
                ?? throw new PremiumInterpretationCatalogException(
                    "El catálogo premium no pudo deserializarse a un objeto válido.");
        }
        catch (PremiumInterpretationCatalogException)
        {
            throw;
        }
        catch (JsonException exception)
        {
            throw new PremiumInterpretationCatalogException(
                "El catálogo premium contiene JSON inválido o incompatible con el modelo tipado.",
                exception);
        }
        catch (Exception exception)
        {
            throw new PremiumInterpretationCatalogException(
                "No fue posible cargar el catálogo premium de interpretaciones.",
                exception);
        }

        ValidateCatalog(_catalog);
    }

    public PremiumInterpretationCatalog GetCatalog()
    {
        return _catalog;
    }

    public InterpretationEntry GetEntry(PremiumInterpretationPosition position, ZodiacSign sign)
    {
        return _catalog.GetEntry(position, sign);
    }

    public TEntry GetEntry<TEntry>(PremiumInterpretationPosition position, ZodiacSign sign)
        where TEntry : InterpretationEntry
    {
        return _catalog.GetEntry<TEntry>(position, sign);
    }

    private static string ResolveCatalogPath()
    {
        return Path.Combine(AppContext.BaseDirectory, CatalogRelativePath);
    }

    private static void ValidateCatalog(PremiumInterpretationCatalog catalog)
    {
        if (string.IsNullOrWhiteSpace(catalog.Version))
        {
            throw new PremiumInterpretationCatalogException(
                "El catálogo premium debe incluir una versión en la raíz.");
        }

        var availableEntryCount =
            catalog.Planets.Sun.EnumerateAvailableEntries().Count() +
            catalog.Planets.Moon.EnumerateAvailableEntries().Count() +
            catalog.Planets.Ascendant.EnumerateAvailableEntries().Count() +
            catalog.Planets.Mercury.EnumerateAvailableEntries().Count() +
            catalog.Planets.Venus.EnumerateAvailableEntries().Count() +
            catalog.Planets.Mars.EnumerateAvailableEntries().Count();

        if (availableEntryCount == 0)
        {
            throw new PremiumInterpretationCatalogException(
                "El catálogo premium no contiene ninguna entrada utilizable.");
        }

        ValidateEntries(catalog.Planets.Sun.EnumerateAvailableEntries(), "sun");
        ValidateEntries(catalog.Planets.Moon.EnumerateAvailableEntries(), "moon");
        ValidateEntries(catalog.Planets.Ascendant.EnumerateAvailableEntries(), "ascendant");
        ValidateEntries(catalog.Planets.Mercury.EnumerateAvailableEntries(), "mercury");
        ValidateEntries(catalog.Planets.Venus.EnumerateAvailableEntries(), "venus");
        ValidateEntries(catalog.Planets.Mars.EnumerateAvailableEntries(), "mars");
    }

    private static void ValidateEntries<TEntry>(
        IEnumerable<KeyValuePair<ZodiacSign, TEntry>> entries,
        string positionName)
        where TEntry : InterpretationEntry
    {
        foreach (var entry in entries)
        {
            ValidateCommonFields(entry.Value, positionName, entry.Key);

            switch (entry.Value)
            {
                case SunInterpretationEntry sun:
                    ValidateRequiredText(sun.IdentityStyle, positionName, entry.Key, "identityStyle");
                    ValidateRequiredText(sun.GrowthPath, positionName, entry.Key, "growthPath");
                    break;
                case MoonInterpretationEntry moon:
                    ValidateRequiredText(moon.EmotionalStyle, positionName, entry.Key, "emotionalStyle");
                    ValidateRequiredText(moon.EmotionalNeeds, positionName, entry.Key, "emotionalNeeds");
                    ValidateRequiredText(moon.SecurityNeeds, positionName, entry.Key, "securityNeeds");
                    break;
                case AscendantInterpretationEntry ascendant:
                    ValidateRequiredText(ascendant.OuterStyle, positionName, entry.Key, "outerStyle");
                    ValidateRequiredText(ascendant.SocialStyle, positionName, entry.Key, "socialStyle");
                    ValidateRequiredText(ascendant.FirstImpression, positionName, entry.Key, "firstImpression");
                    break;
                case MercuryInterpretationEntry mercury:
                    ValidateRequiredText(mercury.ThinkingStyle, positionName, entry.Key, "thinkingStyle");
                    ValidateRequiredText(mercury.CommunicationStyle, positionName, entry.Key, "communicationStyle");
                    ValidateRequiredText(mercury.LearningStyle, positionName, entry.Key, "learningStyle");
                    break;
                case VenusInterpretationEntry venus:
                    ValidateRequiredText(venus.RelationalStyle, positionName, entry.Key, "relationalStyle");
                    ValidateRequiredText(venus.AttractionStyle, positionName, entry.Key, "attractionStyle");
                    ValidateRequiredText(venus.AffectiveNeeds, positionName, entry.Key, "affectiveNeeds");
                    break;
                case MarsInterpretationEntry mars:
                    ValidateRequiredText(mars.ActionStyle, positionName, entry.Key, "actionStyle");
                    ValidateRequiredText(mars.DesireStyle, positionName, entry.Key, "desireStyle");
                    ValidateRequiredText(mars.ConflictStyle, positionName, entry.Key, "conflictStyle");
                    break;
            }
        }
    }

    private static void ValidateCommonFields(
        InterpretationEntry entry,
        string positionName,
        ZodiacSign sign)
    {
        ValidateRequiredText(entry.Summary, positionName, sign, "summary");
        ValidateRequiredCollection(entry.Keywords, positionName, sign, "keywords");
        ValidateRequiredCollection(entry.Strengths, positionName, sign, "strengths");
        ValidateRequiredCollection(entry.Challenges, positionName, sign, "challenges");
        ValidateRequiredCollection(entry.IntegrationHooks, positionName, sign, "integrationHooks");
    }

    private static void ValidateRequiredText(
        string value,
        string positionName,
        ZodiacSign sign,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new PremiumInterpretationCatalogException(
                $"La entrada '{positionName}.{sign.ToString().ToLowerInvariant()}' debe incluir el campo '{fieldName}'.");
        }
    }

    private static void ValidateRequiredCollection(
        IReadOnlyCollection<string> values,
        string positionName,
        ZodiacSign sign,
        string fieldName)
    {
        if (values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            throw new PremiumInterpretationCatalogException(
                $"La entrada '{positionName}.{sign.ToString().ToLowerInvariant()}' debe incluir una colección válida para '{fieldName}'.");
        }
    }
}
