using System.Reflection;
using System.Text;
using AstroReader.AstroEngine.Exceptions;

namespace AstroReader.AstroEngine.Internal;

internal sealed class SwissEphClient : ISwissEphClient
{
    private const string SwissEphTypeName = "SwissEphNet.SwissEph, SwissEphNet";
    private const int HouseCount = 12;

    private readonly Type _swissEphType;
    private readonly object _swissEphInstance;
    private readonly string _houseSystem;
    private bool _disposed;

    public SwissEphClient(string? ephemerisPath, string? houseSystem)
    {
        _swissEphType = Type.GetType(SwissEphTypeName, throwOnError: false)
            ?? throw new AstroCalculationException(
                AstroCalculationErrorCode.Wrapper,
                publicMessage: "El motor astral real no se encuentra disponible en este momento.",
                diagnosticMessage: "SwissEphNet no pudo cargarse. Verifica que el paquete esté restaurado en AstroReader.AstroEngine.");

        _swissEphInstance = Activator.CreateInstance(_swissEphType)
            ?? throw new AstroCalculationException(
                AstroCalculationErrorCode.Wrapper,
                publicMessage: "El motor astral real no pudo inicializarse correctamente.",
                diagnosticMessage: "No se pudo crear una instancia de SwissEphNet.SwissEph.");

        EphemerisPath = string.IsNullOrWhiteSpace(ephemerisPath) ? null : ephemerisPath.Trim();
        _houseSystem = NormalizeHouseSystem(houseSystem);

        if (EphemerisPath is not null)
        {
            if (!Directory.Exists(EphemerisPath))
            {
                throw new AstroCalculationException(
                    AstroCalculationErrorCode.Ephemerides,
                    publicMessage: "Los archivos de efemerides del motor astral no estan disponibles.",
                    diagnosticMessage: $"El directorio de efemérides configurado no existe: '{EphemerisPath}'.");
            }

            var method = FindInstanceMethod("swe_set_ephe_path", parameterCount: 1);
            method.Invoke(_swissEphInstance, [EphemerisPath]);
        }
    }

    public string? EphemerisPath { get; }

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

    public int GetSwissEphemerisPlanetFlags()
    {
        return GetConstant("SEFLG_SWIEPH") | GetConstant("SEFLG_SPEED");
    }

    public SwissPlanetCalculation CalculatePlanetLongitude(double julianDayUt, int planetId, int flags)
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

        return new SwissPlanetCalculation(
            Positions: (double[])callArguments[3],
            ReturnFlag: Convert.ToInt32(returnValue),
            ErrorText: ExtractErrorText(callArguments[4]));
    }

    public SwissHouseCalculation CalculateHouses(double julianDayUt, double latitude, double longitude)
    {
        var method = FindInstanceMethod("swe_houses", parameterCount: 6);
        var parameters = method.GetParameters();
        var cusps = new double[13];
        var ascmc = new double[10];
        var callArguments = new object[6];

        callArguments[0] = julianDayUt;
        callArguments[1] = latitude;
        callArguments[2] = longitude;
        callArguments[3] = CreateHouseSystemArgument(parameters[3].ParameterType, _houseSystem[0]);
        callArguments[4] = cusps;
        callArguments[5] = ascmc;

        var returnValue = method.Invoke(_swissEphInstance, callArguments);
        var returnCode = Convert.ToInt32(returnValue);

        if (returnCode < 0)
        {
            throw new AstroCalculationException(
                AstroCalculationErrorCode.Calculation,
                publicMessage: "No fue posible calcular las casas astrales para la carta solicitada.",
                diagnosticMessage:
                $"Swiss Ephemeris devolvió un error al calcular casas. lat={latitude}, lon={longitude}, jdUt={julianDayUt}, hsys={_houseSystem}.");
        }

        var houseCusps = new Dictionary<int, double>(HouseCount);
        for (var houseNumber = 1; houseNumber <= HouseCount; houseNumber++)
        {
            houseCusps[houseNumber] = NormalizeDegrees(cusps[houseNumber]);
        }

        return new SwissHouseCalculation(
            AscendantDegree: NormalizeDegrees(ascmc[0]),
            HouseCusps: houseCusps);
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

    private int GetConstant(string name)
    {
        var field = _swissEphType.GetField(name, BindingFlags.Public | BindingFlags.Static)
            ?? throw new AstroCalculationException(
                AstroCalculationErrorCode.Wrapper,
                publicMessage: "El wrapper del motor astral no expone un contrato compatible.",
                diagnosticMessage: $"SwissEphNet no expone la constante '{name}'.");

        return Convert.ToInt32(field.GetValue(null));
    }

    private MethodInfo FindInstanceMethod(string name, int parameterCount)
    {
        return _swissEphType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(method => method.Name == name && method.GetParameters().Length == parameterCount)
            ?? throw new AstroCalculationException(
                AstroCalculationErrorCode.Wrapper,
                publicMessage: "El wrapper del motor astral no expone un contrato compatible.",
                diagnosticMessage: $"SwissEphNet no expone el método requerido '{name}' con {parameterCount} parámetros.");
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

        throw new AstroCalculationException(
            AstroCalculationErrorCode.Wrapper,
            publicMessage: "El wrapper del motor astral devolvió un formato inesperado.",
            diagnosticMessage: $"No se reconoce el tipo del parámetro de error de SwissEphNet: '{parameterType.FullName}'.");
    }

    private static object CreateHouseSystemArgument(Type parameterType, char houseSystem)
    {
        var effectiveType = parameterType.IsByRef
            ? parameterType.GetElementType()
            : parameterType;

        if (effectiveType == typeof(char))
        {
            return houseSystem;
        }

        if (effectiveType == typeof(int))
        {
            return (int)houseSystem;
        }

        throw new AstroCalculationException(
            AstroCalculationErrorCode.Wrapper,
            publicMessage: "El wrapper del motor astral devolvió un formato inesperado.",
            diagnosticMessage: $"No se reconoce el tipo del parámetro hsys de SwissEphNet: '{parameterType.FullName}'.");
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

    private static double NormalizeDegrees(double degrees)
    {
        return (degrees % 360 + 360) % 360;
    }

    private static string NormalizeHouseSystem(string? houseSystem)
    {
        if (string.IsNullOrWhiteSpace(houseSystem))
        {
            return Configuration.SwissEphOptions.DefaultHouseSystem;
        }

        return houseSystem.Trim()[0].ToString().ToUpperInvariant();
    }
}
