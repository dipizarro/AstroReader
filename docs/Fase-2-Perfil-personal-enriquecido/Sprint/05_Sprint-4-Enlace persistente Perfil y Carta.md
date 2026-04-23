Sprint — Enlace persistente Perfil ↔ Carta
Objetivo

Dejar de depender de localStorage y hacer que el perfil enriquecido:

corresponda realmente a la carta
se vincule de forma persistente
y se use de forma confiable en lecturas futuras
Qué debe resolver este sprint

1. Validación fuerte en backend

Si viene personalProfileId, el backend debe validar que coincide con:

birthDate
birthTime
latitude
longitude
timezoneOffsetMinutes

Si no coincide:

rechazar
o ignorar el perfil con error claro

Yo prefiero rechazar con error claro.

2. Propagar personalProfileId al guardado de carta

Cuando guardas una carta:

debe viajar personalProfileId
debe persistirse la relación
debe quedar enlazado de verdad con SavedChart 3. Eliminar dependencia del “último perfil” en localStorage

localStorage puede quedar como ayuda UX, pero no como fuente de verdad.

La fuente de verdad debe pasar a ser:

backend
perfil persistido
relación con carta guardada 4. Mejor continuidad UX

Al terminar el onboarding:

ir directo a calcular la carta con datos precargados
o redirigir a una vista donde esa carta ya se calcule con contexto 5. Tests de integridad

Agregar cobertura para:

mismatch perfil/carta
create/get/update perfil
save chart con personalProfileId
vínculo persistente real
