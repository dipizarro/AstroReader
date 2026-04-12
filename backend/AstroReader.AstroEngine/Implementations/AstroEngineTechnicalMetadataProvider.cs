using AstroReader.AstroEngine.Configuration;
using AstroReader.AstroEngine.Contracts;
using Microsoft.Extensions.Options;

namespace AstroReader.AstroEngine.Implementations;

internal sealed class AstroEngineTechnicalMetadataProvider : IAstroEngineTechnicalMetadataProvider
{
    private readonly SwissEphOptions _options;

    public AstroEngineTechnicalMetadataProvider(IOptions<SwissEphOptions> options)
    {
        _options = options.Value;
    }

    public AstroEngineTechnicalMetadata GetCurrent()
    {
        var usesSwissEph = _options.ShouldUseSwissEph();
        var wrapperVersion = usesSwissEph
            ? Type.GetType("SwissEphNet.SwissEph, SwissEphNet", throwOnError: false)?.Assembly.GetName().Version?.ToString()
            : null;

        return new AstroEngineTechnicalMetadata(
            CalculationEngine: _options.GetConfiguredEngineName(),
            HouseSystemCode: usesSwissEph ? _options.HouseSystem : null,
            UsesRealEngine: usesSwissEph,
            UsesCustomEphemerisPath: !string.IsNullOrWhiteSpace(_options.EphemerisPath),
            WrapperVersion: wrapperVersion);
    }
}
