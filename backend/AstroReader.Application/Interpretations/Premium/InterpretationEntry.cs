using System.Text.Json.Serialization;

namespace AstroReader.Application.Interpretations.Premium;

public abstract class InterpretationEntry
{
    [JsonPropertyName("summary")]
    public string Summary { get; init; } = string.Empty;

    [JsonPropertyName("keywords")]
    public IReadOnlyList<string> Keywords { get; init; } = [];

    [JsonPropertyName("strengths")]
    public IReadOnlyList<string> Strengths { get; init; } = [];

    [JsonPropertyName("challenges")]
    public IReadOnlyList<string> Challenges { get; init; } = [];

    [JsonPropertyName("integrationHooks")]
    public IReadOnlyList<string> IntegrationHooks { get; init; } = [];
}

public sealed class SunInterpretationEntry : InterpretationEntry
{
    [JsonPropertyName("identityStyle")]
    public string IdentityStyle { get; init; } = string.Empty;

    [JsonPropertyName("growthPath")]
    public string GrowthPath { get; init; } = string.Empty;
}

public sealed class MoonInterpretationEntry : InterpretationEntry
{
    [JsonPropertyName("emotionalStyle")]
    public string EmotionalStyle { get; init; } = string.Empty;

    [JsonPropertyName("emotionalNeeds")]
    public string EmotionalNeeds { get; init; } = string.Empty;

    [JsonPropertyName("securityNeeds")]
    public string SecurityNeeds { get; init; } = string.Empty;
}

public sealed class AscendantInterpretationEntry : InterpretationEntry
{
    [JsonPropertyName("outerStyle")]
    public string OuterStyle { get; init; } = string.Empty;

    [JsonPropertyName("socialStyle")]
    public string SocialStyle { get; init; } = string.Empty;

    [JsonPropertyName("firstImpression")]
    public string FirstImpression { get; init; } = string.Empty;
}

public sealed class MercuryInterpretationEntry : InterpretationEntry
{
    [JsonPropertyName("thinkingStyle")]
    public string ThinkingStyle { get; init; } = string.Empty;

    [JsonPropertyName("communicationStyle")]
    public string CommunicationStyle { get; init; } = string.Empty;

    [JsonPropertyName("learningStyle")]
    public string LearningStyle { get; init; } = string.Empty;
}

public sealed class VenusInterpretationEntry : InterpretationEntry
{
    [JsonPropertyName("relationalStyle")]
    public string RelationalStyle { get; init; } = string.Empty;

    [JsonPropertyName("attractionStyle")]
    public string AttractionStyle { get; init; } = string.Empty;

    [JsonPropertyName("affectiveNeeds")]
    public string AffectiveNeeds { get; init; } = string.Empty;
}

public sealed class MarsInterpretationEntry : InterpretationEntry
{
    [JsonPropertyName("actionStyle")]
    public string ActionStyle { get; init; } = string.Empty;

    [JsonPropertyName("desireStyle")]
    public string DesireStyle { get; init; } = string.Empty;

    [JsonPropertyName("conflictStyle")]
    public string ConflictStyle { get; init; } = string.Empty;
}
