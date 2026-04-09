using AstroReader.AstroEngine.Contracts;
using System.Collections.Generic;

namespace AstroReader.AstroEngine.Implementations;

/// <summary>
/// Implementación de prueba del motor astronómico.
/// En el futuro, crearemos un "SwissEphemerisEngine" que implementará The misma interfaz.
/// </summary>
public class MockAstroCalculationEngine : IAstroCalculationEngine
{
    public AstroCalculationResult Calculate(AstroCalculationRequest request)
    {
        // Valores en duro temporales, sólo para que compile y la App pueda usarlos.
        // Aquí entraría la invocación C/C++ externa.
        return new AstroCalculationResult
        {
            AscendantDegree = 185.0, // Libra (180+)
            PlanetaryPositions = new Dictionary<int, AsteroidalData>
            {
                { 0, new AsteroidalData(49.4, 1, 19.4, false) }, // Sun ~ 49.4° (Taurus)
                { 1, new AsteroidalData(127.8, 4, 7.8, false) }, // Moon ~ 127.8° (Leo)
            },
            Houses = new Dictionary<int, double>
            {
                { 1, 180.0 }, // Cúspide de Casa 1
                { 2, 210.0 }  // Cúspide de Casa 2
            }
        };
    }
}
