1. Objetivo real de esta fase

Reemplazar MockAstroCalculationEngine por un motor real que calcule, al menos:

Sol
Luna
Mercurio
Venus
Marte
Júpiter
Saturno
Ascendente
casas

Y hacerlo sin romper:

arquitectura
contratos
frontend
persistencia 2.

Qué NO vamos a hacer todavía

Para que esto no se descontrole, yo dejaría fuera por ahora:

Urano, Neptuno y Plutón si complican demasiado al inicio
nodos
quiron
aspectos
tránsitos
revolución solar
correcciones ultra avanzadas
precisión “nivel software profesional astrológico premium”

La meta inicial no es hacer el motor definitivo.
La meta es hacer un motor real, confiable y usable.

3. Estrategia recomendada

Yo no intentaría reinventar astronomía.

La estrategia correcta es:

Backend principal en .NET

pero

AstroEngine apoyado en una librería seria o wrapper especializado

Eso calza perfecto con lo que ya construiste.

4. Posibles caminos técnicos
   Opción A — librería .NET astronómica suficiente

Sería el camino más cómodo si encuentras una buena.

Opción B — Swiss Ephemeris vía wrapper

Probablemente el camino más serio y realista.

Opción C — microservicio externo de cálculo

Lo dejaría como plan B si .NET + wrapper se vuelve demasiado áspero.

5. Mi recomendación concreta

Yo atacaría así:

Fase 4.1

Spike técnico
Descubrir la mejor forma de integrar cálculo real en .NET.

Fase 4.2

Implementación mínima real
Sol, Luna, Mercurio, Venus, Marte, Júpiter, Saturno.

Fase 4.3

Ascendente y casas
Ya con input temporal/espacial limpio.

Fase 4.4

Validación
Comparar resultados con fuentes de referencia.

6. Qué debes validar sí o sí

Cuando entre el engine real, ya no basta con “compila”.

Debes probar:

distintas fechas
distintas horas
distintos lugares
que cambien signos y grados
que el ascendente cambie de forma sensible
que las casas tengan coherencia
