Hallazgos

Alta: hoy el vínculo perfil -> lectura no es confiable a nivel de identidad ni de datos natales. El frontend adjunta personalProfileId usando solo coincidencia de birthDate + birthTime del último perfil en localStorage (CalculateChartForm.tsx (line 165), personalProfileStorage.ts (line 3)). Luego el backend acepta ese id sin verificar que el perfil coincida con la carta que se está calculando (CalculateNatalChartUseCase.cs (line 134), CalculateNatalChartUseCase.cs (line 230)). Resultado: un perfil puede contaminar una lectura que no le corresponde.
Alta: el sistema ya está modelado para enlazar perfil y carta guardada, pero el flujo real no lo usa. La FK y el índice único están bien planteados (PersonalProfileConfiguration.cs (line 64)), y el alta de perfil ya soporta SavedChartId (CreatePersonalProfileUseCase.cs (line 29)), pero guardar carta no recibe ni propaga PersonalProfileId (SaveChartRequest.cs (line 5), SaveChartUseCase.cs (line 27)). En producto, eso deja la integración apoyada en memoria local del navegador, no en una relación duradera.
Media: el onboarding ya es razonablemente fluido, pero termina en un callejón sin salida. Cuando el perfil se crea, la pantalla solo ofrece Crear otro perfil y no empuja a la siguiente acción valiosa, que sería calcular o releer la carta con contexto (PersonalProfileOnboardingForm.tsx (line 767)). La UX del formulario es buena; la continuidad del producto todavía no.
Media: la integración editorial sí agrega valor, pero todavía depende de heurísticas frágiles. El bloque nuevo está bien orientado y no invalida al usuario (PremiumInterpretationProfileNarrative.cs (line 85)), pero la priorización y el contraste se resuelven por substring matching y vocabulario fijo (PremiumInterpretationProfileNarrative.cs (line 18), PremiumInterpretationProfileNarrative.cs (line 176)). Funciona como v1 editorial, no como capa robusta todavía.
Media-baja: el fallback conserva el gesto de contexto, pero se simplifica bastante y pierde parte del valor del perfil. En fallback solo aparece una versión resumida del bloque de contraste y no la priorización (PremiumInterpretationFallbackFactory.cs (line 39)). Eso hace que la experiencia sea desigual según cobertura editorial.
Media-baja: la cobertura de tests está corta para esta fase. Encontré test del bloque editorial nuevo (PremiumInterpretationProfileNarrativeTests.cs (line 11)), pero no vi cobertura dedicada para crear/leer/actualizar perfil, enlazarlo con carta guardada ni validar mismatches entre personalProfileId y datos natales.
Evaluación
El perfil sí agrega valor real y, en lo editorial, no reemplaza la carta. La mejor señal es que el bloque nuevo parte desde Sol/Luna/Ascendente y recién después conversa con la autopercepción (PremiumInterpretationProfileNarrative.cs (line 99)). También la UI lo presenta como “contexto” y no como núcleo de lectura (ChartInterpretationSection.tsx (line 165)).

La persistencia base quedó sana: entidad clara, validaciones razonables, timestamps, coordenadas con precisión y unicidad por SavedChartId cuando exista el vínculo. El problema no es el modelo; es que todavía no cerramos el circuito de persistencia real entre onboarding, carta guardada y lectura futura.

Siguiente sprint
Haría un sprint de continuidad y enlace duradero del perfil, antes de más pulido cosmético.

Validar en backend que personalProfileId coincida con birthDate, birthTime, latitude, longitude y timezoneOffsetMinutes de la lectura.
Pasar personalProfileId al flujo de guardado de carta y enlazar perfil <-> SavedChart de forma persistente.
Después de crear perfil, llevar al usuario directo a calcular su carta con datos precargados o a ver la lectura contextualizada.
Dejar de depender del “último perfil en localStorage” como fuente principal de contexto.
Agregar tests de use case para create/get/update, enlace con SavedChart, y casos de mismatch de perfil.
Afinar la capa editorial con fixtures reales de respuestas de usuarios, para ver dónde el contraste/priorización suena útil y dónde se siente mecánico.
Mi lectura corta es esta: la fase va bien y ya se nota en producto, pero todavía está en “valor visible con pegamento temporal”. El próximo sprint debería convertir eso en una experiencia durable y consistente.
