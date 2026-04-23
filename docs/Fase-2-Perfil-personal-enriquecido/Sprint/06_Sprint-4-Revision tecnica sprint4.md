Hallazgos

Bien resuelto: la creación de perfil ya entra al producto con una continuidad bastante sana. El onboarding crea el perfil, guarda un contexto explícito y navega al cálculo con profileContext, en vez de “adivinar” el perfil en caliente (PersonalProfileOnboardingForm.tsx (line 149), CalculateChartPage.tsx (line 22)).
Bien resuelto: el backend ya protege la lectura contra contaminación de perfil. Si llega personalProfileId, valida birthDate, birthTime, latitude, longitude y timezoneOffsetMinutes antes de usarlo en la interpretación (CalculateNatalChartUseCase.cs (line 238)).
Bien resuelto: el vínculo persistente perfil-carta existe de verdad y además tiene unicidad en BD (SaveChartUseCase.cs (line 68), PersonalProfileConfiguration.cs (line 71)).
Puntos débiles

Alto: el vínculo persistente todavía no se reutiliza bien en producto. Backend expone GET by-saved-chart, pero el frontend no lo consume, y el detalle de carta guardada tampoco devuelve personalProfileId ni perfil asociado. Hoy el enlace sirve para integridad, pero no para rehidratar experiencia (PersonalProfilesController.cs (line 64), SavedChartDetailDto.cs (line 5)).
Alto: el guardado perfil <-> carta no es transaccional. Primero se persiste SavedChart y recién después se actualiza PersonalProfile.SavedChartId; si falla la segunda escritura, queda una carta guardada sin enlace durable (SaveChartUseCase.cs (line 66)).
Medio: localStorage ya no manda la verdad del negocio, pero todavía manda demasiado la UX. Entrar a calcular carta dentro de la ventana de 2 horas vuelve a precargar automáticamente el último contexto pendiente aunque el usuario no venga desde ese flujo (personalProfileStorage.ts (line 3), CalculateChartPage.tsx (line 22)).
Medio: el modo manual del cálculo degrada el valor humano del dato. Si el usuario calcula en avanzado, el lugar pasa a guardarse como "Ubicación manual" en vez de un nombre real, y además el frontend no manda timezoneIana al guardar la carta (CalculateChartForm.tsx (line 160), ChartResult.tsx (line 69)).
Medio: cuando el formulario deja de adjuntar personalProfileId por cambio de datos, el comportamiento es sano, pero silencioso. A nivel producto falta un indicador claro de “perfil desvinculado para esta lectura” en el momento en que eso ocurre (CalculateChartForm.tsx (line 199)).
Medio-bajo: la persistencia del perfil existe, pero el producto todavía no ofrece una reutilización explícita de perfiles existentes. Hoy el frontend tiene create y getById, pero no un flujo real de “elegir perfil” o “retomar perfil” (personalProfileService.ts (line 5)).
Correcciones propuestas

Cerrar el circuito persistente de verdad: al abrir una carta guardada, recuperar automáticamente su perfil asociado usando by-saved-chart o incluyendo personalProfileId/perfil resumido dentro de SavedChartDetailDto.
Volver atómico el guardado: envolver SavedChart + PersonalProfile.SavedChartId en una transacción de aplicación/EF para no dejar estados intermedios.
Bajar la agresividad del contexto pendiente: usar location.state como fuente principal y dejar localStorage solo para recuperación explícita, no para precargar automáticamente cualquier entrada a /chart/calculate.
Mejorar la calidad del snapshot: permitir nombre humano del lugar también en el cálculo manual y enviar timezoneIana cuando el lugar venga geocodificado.
Hacer visible la desvinculación: si cambian datos natales y el perfil deja de aplicar, mostrar un aviso corto en el formulario antes del submit.
Siguiente sprint recomendado: “reutilización persistida”. Menos crear perfiles nuevos y más retomar perfil + carta guardada + lectura contextualizada duradera.

---

---

Prioridad 1 — reutilizar el vínculo persistente

Este es el punto grande.

Si ya existe:

GET by-saved-chart
FK
unicidad
enlace persistente

pero el frontend no lo consume y el detalle de carta no devuelve bien ese contexto, entonces todavía no estás cobrando el valor completo de lo que construiste.

Hoy el vínculo existe más para integridad que para experiencia.

Eso hay que cambiarlo.

Prioridad 2 — volver atómico el guardado perfil ↔ carta

Totalmente de acuerdo.

Guardar:

SavedChart
luego actualizar PersonalProfile.SavedChartId

sin transacción, deja una ventana de inconsistencia.

Eso sí conviene corregirlo antes de seguir sumando UX encima.

Prioridad 3 — bajar el protagonismo de localStorage

Tu hallazgo aquí es muy bueno.

localStorage ya no manda la verdad del negocio, pero todavía manda demasiado la experiencia.
Y eso puede meter comportamientos “fantasma”:

entras a calcular carta
y el sistema te mete un perfil pendiente que ya no debería influir

Yo dejaría location.state como fuente principal del flujo inmediato, y localStorage solo como recuperación explícita.

Prioridad 4 — retomar perfiles existentes

Este punto me parece muy importante a nivel producto.

Hoy AstroReader ya puede crear perfil.
El siguiente salto natural es:

elegir perfil
retomar perfil
volver a una carta asociada
seguir la experiencia contextualizada

Ahí el producto empieza a sentirse mucho más real.
