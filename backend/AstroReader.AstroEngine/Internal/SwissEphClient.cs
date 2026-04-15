using System.Reflection;
using System.Text;
using AstroReader.AstroEngine.Exceptions;

namespace AstroReader.AstroEngine.Internal;

internal sealed class SwissEphClient : ISwissEphClient
{
    private const string SwissEphTypeName = "SwissEphNet.SwissEph, SwissEphNet";
    private const string SetEphePathMethodName = "swe_set_ephe_path";
    private const string JulianDayMethodName = "swe_julday";
    private const string CalcUtMethodName = "swe_calc_ut";
    private const string HousesMethodName = "swe_houses";
    private const string GregorianCalendarConstantName = "SE_GREG_CAL";
    private const string SwissEphemerisFlagConstantName = "SEFLG_SWIEPH";
    private const string SpeedFlagConstantName = "SEFLG_SPEED";
    private const int HouseCount = 12;

    private readonly Type _swissEphType;
    private readonly SwissEphContract _contract;
    private readonly object _swissEphInstance;
    private readonly string _houseSystem;
    private bool _disposed;

    public SwissEphClient(string? ephemerisPath, string? houseSystem)
    {
        _swissEphType = LoadSwissEphType();
        _contract = ResolveContract(_swissEphType);

        _swissEphInstance = Activator.CreateInstance(_swissEphType)
            ?? throw new AstroCalculationException(
                AstroCalculationErrorCode.Wrapper,
                publicMessage: "El motor astral real no pudo inicializarse correctamente.",
                diagnosticMessage: "No se pudo crear una instancia de SwissEphNet.SwissEph.");

        EphemerisPath = string.IsNullOrWhiteSpace(ephemerisPath) ? null : ephemerisPath.Trim();
        _houseSystem = NormalizeHouseSystem(houseSystem);

        if (EphemerisPath is not null)
        {
            ValidateEphemerisDirectory(EphemerisPath);
            _contract.SetEphemerisPath.Invoke(_swissEphInstance, [EphemerisPath]);
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

        var result = _contract.CalculateJulianDay.Invoke(_swissEphInstance,
        [
            utc.Year,
            utc.Month,
            utc.Day,
            hour,
            _contract.GregorianCalendarFlag
        ]);

        return Convert.ToDouble(result);
    }

    public int GetSwissEphemerisPlanetFlags()
    {
        return _contract.SwissEphemerisFlag | _contract.SpeedFlag;
    }

    public SwissPlanetCalculation CalculatePlanetLongitude(double julianDayUt, int planetId, int flags)
    {
        var parameters = _contract.CalculatePlanetLongitude.GetParameters();
        var callArguments = new object[5];

        callArguments[0] = julianDayUt;
        callArguments[1] = planetId;
        callArguments[2] = flags;
        callArguments[3] = new double[6];
        callArguments[4] = CreateErrorArgument(parameters[4].ParameterType);

        var returnValue = _contract.CalculatePlanetLongitude.Invoke(_swissEphInstance, callArguments);

        return new SwissPlanetCalculation(
            Positions: (double[])callArguments[3],
            ReturnFlag: Convert.ToInt32(returnValue),
            ErrorText: ExtractErrorText(callArguments[4]));
    }

    public SwissHouseCalculation CalculateHouses(double julianDayUt, double latitude, double longitude)
    {
        var parameters = _contract.CalculateHouses.GetParameters();
        var cusps = new double[13];
        var ascmc = new double[10];
        var callArguments = new object[6];

        callArguments[0] = julianDayUt;
        callArguments[1] = latitude;
        callArguments[2] = longitude;
        callArguments[3] = CreateHouseSystemArgument(parameters[3].ParameterType, _houseSystem[0]);
        callArguments[4] = cusps;
        callArguments[5] = ascmc;

        var returnValue = _contract.CalculateHouses.Invoke(_swissEphInstance, callArguments);
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

    private static Type LoadSwissEphType()
    {
        return Type.GetType(SwissEphTypeName, throwOnError: false)
            ?? throw new AstroCalculationException(
                AstroCalculationErrorCode.Wrapper,
                publicMessage: "El motor astral real no se encuentra disponible en este momento.",
                diagnosticMessage: "SwissEphNet no pudo cargarse. Verifica que el paquete esté restaurado en AstroReader.AstroEngine.");
    }

    private static void ValidateEphemerisDirectory(string ephemerisPath)
    {
        if (!Directory.Exists(ephemerisPath))
        {
            throw new AstroCalculationException(
                AstroCalculationErrorCode.Ephemerides,
                publicMessage: "Los archivos de efemerides del motor astral no estan disponibles.",
                diagnosticMessage: $"El directorio de efemérides configurado no existe: '{ephemerisPath}'.");
        }

        try
        {
            if (!Directory.EnumerateFileSystemEntries(ephemerisPath).Any())
            {
                throw new AstroCalculationException(
                    AstroCalculationErrorCode.Ephemerides,
                    publicMessage: "Los archivos de efemerides del motor astral no estan disponibles.",
                    diagnosticMessage: $"El directorio de efemérides configurado está vacío: '{ephemerisPath}'.");
            }
        }
        catch (AstroCalculationException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new AstroCalculationException(
                AstroCalculationErrorCode.Ephemerides,
                publicMessage: "Los archivos de efemerides del motor astral no estan disponibles.",
                diagnosticMessage: $"No fue posible leer el directorio de efemérides configurado: '{ephemerisPath}'.",
                innerException: exception);
        }
    }

    private static SwissEphContract ResolveContract(Type swissEphType)
    {
        // Contrato externo mínimo esperado del wrapper SwissEphNet para este spike.
        // Si alguna firma cambia, preferimos fallar acá con un error explícito.
        var setEphemerisPath = RequireInstanceMethod(swissEphType, SetEphePathMethodName, 1);
        RequireParameterType(setEphemerisPath, 0, typeof(string));

        var calculateJulianDay = RequireInstanceMethod(swissEphType, JulianDayMethodName, 5);
        RequireParameterType(calculateJulianDay, 0, typeof(int));
        RequireParameterType(calculateJulianDay, 1, typeof(int));
        RequireParameterType(calculateJulianDay, 2, typeof(int));
        RequireParameterType(calculateJulianDay, 3, typeof(double));
        RequireParameterType(calculateJulianDay, 4, typeof(int));

        var calculatePlanetLongitude = RequireInstanceMethod(swissEphType, CalcUtMethodName, 5);
        RequireParameterType(calculatePlanetLongitude, 0, typeof(double));
        RequireParameterType(calculatePlanetLongitude, 1, typeof(int));
        RequireParameterType(calculatePlanetLongitude, 2, typeof(int));
        RequireParameterType(calculatePlanetLongitude, 3, typeof(double[]));
        RequireParameterMatchesOneOf(calculatePlanetLongitude, 4, typeof(string), typeof(StringBuilder));

        var calculateHouses = RequireInstanceMethod(swissEphType, HousesMethodName, 6);
        RequireParameterType(calculateHouses, 0, typeof(double));
        RequireParameterType(calculateHouses, 1, typeof(double));
        RequireParameterType(calculateHouses, 2, typeof(double));
        RequireParameterMatchesOneOf(calculateHouses, 3, typeof(char), typeof(int));
        RequireParameterType(calculateHouses, 4, typeof(double[]));
        RequireParameterType(calculateHouses, 5, typeof(double[]));

        return new SwissEphContract(
            SetEphemerisPath: setEphemerisPath,
            CalculateJulianDay: calculateJulianDay,
            CalculatePlanetLongitude: calculatePlanetLongitude,
            CalculateHouses: calculateHouses,
            GregorianCalendarFlag: GetConstant(swissEphType, GregorianCalendarConstantName),
            SwissEphemerisFlag: GetConstant(swissEphType, SwissEphemerisFlagConstantName),
            SpeedFlag: GetConstant(swissEphType, SpeedFlagConstantName));
    }

    private static int GetConstant(Type swissEphType, string name)
    {
        var field = swissEphType.GetField(name, BindingFlags.Public | BindingFlags.Static)
            ?? throw new AstroCalculationException(
                AstroCalculationErrorCode.Wrapper,
                publicMessage: "El wrapper del motor astral no expone un contrato compatible.",
                diagnosticMessage: $"SwissEphNet no expone la constante '{name}'.");

        return Convert.ToInt32(field.GetValue(null));
    }

    private static MethodInfo RequireInstanceMethod(Type swissEphType, string name, int parameterCount)
    {
        return swissEphType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(method => method.Name == name && method.GetParameters().Length == parameterCount)
            ?? throw new AstroCalculationException(
                AstroCalculationErrorCode.Wrapper,
                publicMessage: "El wrapper del motor astral no expone un contrato compatible.",
                diagnosticMessage: $"SwissEphNet no expone el método requerido '{name}' con {parameterCount} parámetros.");
    }

    private static void RequireParameterType(MethodInfo method, int index, Type expectedType)
    {
        var actualType = GetEffectiveParameterType(method, index);

        if (actualType != expectedType)
        {
            throw CreateSignatureException(method, index, actualType, expectedType.Name);
        }
    }

    private static void RequireParameterMatchesOneOf(MethodInfo method, int index, params Type[] expectedTypes)
    {
        var actualType = GetEffectiveParameterType(method, index);

        if (!expectedTypes.Contains(actualType))
        {
            var expectedTypeNames = string.Join(" or ", expectedTypes.Select(x => x.Name));
            throw CreateSignatureException(method, index, actualType, expectedTypeNames);
        }
    }

    private static Type GetEffectiveParameterType(MethodInfo method, int index)
    {
        var parameterType = method.GetParameters()[index].ParameterType;

        return parameterType.IsByRef
            ? parameterType.GetElementType() ?? parameterType
            : parameterType;
    }

    private static AstroCalculationException CreateSignatureException(
        MethodInfo method,
        int parameterIndex,
        Type actualType,
        string expectedTypeName)
    {
        return new AstroCalculationException(
            AstroCalculationErrorCode.Wrapper,
            publicMessage: "El wrapper del motor astral no expone un contrato compatible.",
            diagnosticMessage:
            $"SwissEphNet expone una firma inesperada para '{method.Name}'. El parámetro #{parameterIndex} debería ser '{expectedTypeName}' pero fue '{actualType.FullName}'.");
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

    private sealed record SwissEphContract(
        MethodInfo SetEphemerisPath,
        MethodInfo CalculateJulianDay,
        MethodInfo CalculatePlanetLongitude,
        MethodInfo CalculateHouses,
        int GregorianCalendarFlag,
        int SwissEphemerisFlag,
        int SpeedFlag
    );
}
