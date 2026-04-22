# Revisión Técnica: AstroReader pre-Sprint 3

Esta auditoría técnica detalla las robustas bases que hemos sentado, detecta cuellos de botella emergentes (deuda técnica intencional y no intencional) y provee un roadmap realista para crecer en el próximo Sprint.

---

## 1. Bases Magistrales (Qué está bien resuelto)

*   **Arquitectura Limpia & Desacoplamiento Excepcional:**
    *   Si hoy necesitas migrar el motor matemático a Swiss Ephemeris (`AstroEngine`), no tocas ni una línea de `Application` ni del `Controller`.
    *   El modelo de dominio (`NatalChart`, `PlanetPosition`) permanece inmutable y purificado de DTOs JSON.
*   **Manejo Unificado de Errores Vía Interceptor:** El `GlobalExceptionHandler` intercepta silenciosamente y envía RFC 7807 (`ProblemDetails` 400 o 500) garantizando que el Frontend React consumirá JSONs consistentes, blindando excepciones internas de API.
*   **Contrato de Salida Rico:** Proveemos Metadatos UTC, Grados Relativos/Absolutos y `IsRetrograde` desde la raíz. El Frontend React ahora tiene los campos perfectos de **`AbsoluteDegree`** para simplemente inyectar la data en gráficos polares en canvas o D3 sin ninguna matemática compleja.

---

## 2. Deuda Técnica Temprana (Ajustes recomendados AHORA)

> [!WARNING]
> Elementos críticos que deben acomodarse antes de escalar la lógica astrológica.

1.  **Falsa Resolución Espacial (Geocoding):**
    *   **Problema:** Nuestro DTO de entrada acepta `BirthPlace` (ej: "Buenos Aires") y el servidor lo ignora y *hardcodea* Lat/Long en `-34.6` y `-58.4`.
    *   **Solución (Sprint 3):** Eliminar `BirthPlace` del DTO de cálculo y requerir obligatoriamente `Latitude` y `Longitude`. El Frontend React debe resolver la ciudad a coordenadas (vía Google Maps/Mapbox API) antes de enviar la petición.
2.  **Tiempo y Ceguera de Zonas Horarias:**
    *   **Problema:** Concatenamos un string manual a una matriz UTC pura omitiendo el TimeZone de la ciudad donde ocurrió el evento.
    *   **Solución (Sprint 3):** Exigir un `TimeZoneOffset` local además del String. Si el front envía "1990-05-15 14:30" en Madrid, el backend requiere saber a cuántas horas UTC equivale ese Offset en esa época para alimentar exacto al `AstroEngine`.

---

## 3. Hoja de Vida y Escalado (Qué dejar para más ADELANTE)

> [!TIP]
> No incurrir en sobreingeniería hoy en estos puntos para mantener agilidad en MVP.

1.  **Interpretaciones Ricas (Base de Datos / JSONs)**
    *   *Ahora:* Tu `BasicInterpretationEngine` compila Switch Expressions (`switch/case`). Es lo ideal. ¡Es un nanosegundo de costo!
    *   *Futuro:* Cuando un CMS o editores modifiquen los textos sagitarios/acuarios mensualmente, mover esto a EF Core (`Infrastructure`) contra una Tabla SQL o un generador OpenAI.
2.  **Aspectos Planetarios (Geometría Compleja)**
    *   Cálculo de Trígonos/Cuadraturas involucra una n dimensionalidad de combinaciones matemáticas limitadas por un "Orb" tolerado. Aislar primero Swiss Ephemeris de planetas base antes de tocar aspectos.
3.  **Persistencia y Perfiles de Lectura (EF Core)**
    *   Guardar la "Carta de mi Amigo Juan". Introduce Entity Framework Core en el proyecto `Infrastructure` para almacenar `ChartProfile` vinculados a un futuro `UserId`. Hoy la API es un "Calculador Stateless". Mantengámoslo así hasta estabilizar el motor real.

---

## 4. Resumen Propuesto Plan de Choque "Sprint 3"

1.  *(Backend)*: Refactorizar `CalculateChartRequest` eliminando el string del lugar y aceptando nativamente `Latitude` y `Longitude`.
2.  *(AstroEngine)*: Reemplazar `MockAstroCalculationEngine` conectando una DLL real de *Swiss Ephemeris* (.NET Standard wrapper en C/C++ clásico) para la primera matemática verdadera de Sol y Luna.
3.  *(Frontend React)*: Integrar un Autocomplete (Mapbox/Google Places) donde React extraiga el Lat/Lng del buscador local, la envíe a nuestro backend, y agarre nuestro Array absoluto de `AbsoluteDegree` para pintar una carta de 12 sectores.

## Formulación de Siguiente Paso
Revisa esta radiografía general. Entiendo que lo primero y más ágil será arreglar y obligar la "Entrada Espacial". ¿Le damos prioridad a la corrección del "Geocoding/Coordenadas" en el DTO de entrada para dejar la cañería limpia para el Motor en este hilo?
