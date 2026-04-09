using AstroReader.AstroEngine.Configuration;
using Microsoft.Extensions.Options;

namespace AstroReader.AstroEngine.Internal;

internal sealed class SwissEphClientFactory : ISwissEphClientFactory
{
    private readonly SwissEphOptions _options;

    public SwissEphClientFactory(IOptions<SwissEphOptions> options)
    {
        _options = options.Value;
    }

    public ISwissEphClient CreateClient()
    {
        // SwissEphNet no es thread-safe por instancia.
        // Creamos una instancia nueva por operación y la descartamos al final.
        return new SwissEphClient(_options.EphemerisPath);
    }
}
