using System.Reflection;
using System.Text;

namespace AstroReader.AstroEngine.Implementations;

/// <summary>
/// Wrapper reflection-based para mantener el spike desacoplado de detalles finos del API del port.
/// Si SwissEphNet cambia alguna firma menor, este punto absorbe el impacto mejor que el resto del sistema.
/// </summary>
internal sealed class SwissEphReflectionBridge : IDisposable
{
    private const string SwissEphTypeName = "SwissEphNet.SwissEph, SwissEphNet";

    private readonly Type _swissEphType;
    private readonly object _swissEphInstance;
    private bool _disposed;

    public SwissEphReflectionBridge()
    {
        _swissEphType = Type.GetType(SwissEphTypeName, throwOnError: false)
            ?? throw new InvalidOperationException(
                "SwissEphNet no pudo cargarse. Verifica que el paquete esté restaurado en AstroReader.AstroEngine.");

        _swissEphInstance = Activator.CreateInstance(_swissEphType)
            ?? throw new InvalidOperationException("No se pudo crear una instancia de SwissEphNet.SwissEph.");
    }

    public void ConfigureEphemerisPath(string? ephemerisPath)
    {
        if (string.IsNullOrWhiteSpace(ephemerisPath))
        {
            return;
        }

        var method = FindInstanceMethod("swe_set_ephe_path", parameterCount: 1);
        method.Invoke(_swissEphInstance, [ephemerisPath]);
    }

    public int GetConstant(string name)
    {
        var field = _swissEphType.GetField(name, BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException($"SwissEphNet no expone la constante '{name}'.");

        return Convert.ToInt32(field.GetValue(null));
    }

    public double CalculateJulianDayUt(DateTime utcDateTime)
    {
        var utc = EnsureUtc(utcDateTime);
        var hour = utc.Hour
            + utc.Minute / 60d
            + utc.Second / 3600d
            + utc.Millisecond / 3_600_000d;

        var gregorianCalendarFlag = GetConstant("SE_GREG_CAL");
        var method = FindInstanceMethod("swe_julday", parameterCount: 5);

        var result = method.Invoke(_swissEphInstance,
        [
            utc.Year,
            utc.Month,
            utc.Day,
            hour,
            gregorianCalendarFlag
        ]);

        return Convert.ToDouble(result);
    }

    public (double[] Positions, int ReturnFlag, string ErrorText) CalculatePlanetLongitude(
        double julianDayUt,
        int planetId,
        int flags)
    {
        var method = FindInstanceMethod("swe_calc_ut", parameterCount: 5);
        var parameters = method.GetParameters();
        var callArguments = new object[5];

        callArguments[0] = julianDayUt;
        callArguments[1] = planetId;
        callArguments[2] = flags;
        callArguments[3] = new double[6];
        callArguments[4] = CreateErrorArgument(parameters[4].ParameterType);

        var returnValue = method.Invoke(_swissEphInstance, callArguments);

        return (
            Positions: (double[])callArguments[3],
            ReturnFlag: Convert.ToInt32(returnValue),
            ErrorText: ExtractErrorText(callArguments[4])
        );
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        (_swissEphInstance as IDisposable)?.Dispose();
        _disposed = true;
    }

    private MethodInfo FindInstanceMethod(string name, int parameterCount)
    {
        return _swissEphType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(method => method.Name == name && method.GetParameters().Length == parameterCount)
            ?? throw new InvalidOperationException($"SwissEphNet no expone el método requerido '{name}'.");
    }

    private static object CreateErrorArgument(Type parameterType)
    {
        var effectiveType = parameterType.IsByRef
            ? parameterType.GetElementType()
            : parameterType;

        if (effectiveType == typeof(StringBuilder))
        {
            return new StringBuilder(256);
        }

        if (effectiveType == typeof(string))
        {
            return string.Empty;
        }

        throw new InvalidOperationException(
            $"No se reconoce el tipo del parámetro de error de SwissEphNet: '{parameterType.FullName}'.");
    }

    private static string ExtractErrorText(object argument)
    {
        return argument switch
        {
            string text => text,
            StringBuilder buffer => buffer.ToString(),
            _ => string.Empty
        };
    }

    private static DateTime EnsureUtc(DateTime utcDateTime)
    {
        return utcDateTime.Kind switch
        {
            DateTimeKind.Utc => utcDateTime,
            DateTimeKind.Local => utcDateTime.ToUniversalTime(),
            _ => DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc)
        };
    }
}
