using AstroReader.AstroEngine.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace AstroReader.AstroEngine.Internal;

internal sealed class SwissEphClientFactory : ISwissEphClientFactory
{
    private readonly SwissEphOptions _options;
    private readonly ILogger<SwissEphClientFactory> _logger;
    private int _createdClientsCount;

    public SwissEphClientFactory(
        IOptions<SwissEphOptions> options,
        ILogger<SwissEphClientFactory> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public ISwissEphClient CreateClient()
    {
        // SwissEphNet no es thread-safe por instancia.
        // Creamos una instancia nueva por operación y la descartamos al final.
        _createdClientsCount++;

        if (_createdClientsCount == 1)
        {
            _logger.LogInformation(
                "Creating first SwissEph client instance. HouseSystem={HouseSystem}, EphemerisPath={EphemerisPath}",
                _options.HouseSystem,
                _options.GetEphemerisPathForLogs());
        }

        return new SwissEphClient(_options.EphemerisPath, _options.HouseSystem);
    }
}
