Plan previo a Sprint 4
Bloque 1 — Integridad del guardado

Esto es lo más importante.

Hoy el frontend puede mandar el Chart completo y el backend lo persiste casi tal cual. Eso abre la puerta a inconsistencias entre datos natales y snapshot guardado.

Qué haría:

cambiar SaveChartRequest para que el frontend envíe solo:
nombre de la carta
input natal original
metadata mínima de UI si hace falta
el backend recalcula la carta al guardar, o al menos valida que el snapshot recibido coincida con el cálculo
dejar al backend como única fuente de verdad del ResultSnapshotJson

Resultado:

eliminas cartas adulteradas
dejas sana la base para cuentas pagas
cortas el riesgo más grande ahora mismo
Bloque 2 — Versionado y separación de snapshot

Tu análisis también detecta que SavedChart quedó bien para MVP, pero es frágil para evolución porque no distingue bien entre input original y resultado derivado, y no tiene SnapshotVersion.

Qué haría:

agregar SnapshotVersion
separar en la entidad:
OriginalInputJson o campos explícitos de input natal
ResultSnapshotJson
guardar timezone de forma más robusta, idealmente también con identificador IANA si ya lo tienes o al menos dejar el campo preparado
no depender ciegamente del DTO actual de CalculateChartResponse como contrato eterno

Resultado:

reduces ruptura retroactiva
preparas migraciones futuras de lectura
puedes soportar v1, v2, etc.
Bloque 3 — Optimizar persistencia/listado

Ahora el listado está trayendo entidades completas, incluyendo snapshot completo, aunque la vista de listado no lo necesita. Eso escala mal.

Qué haría:

crear una proyección de resumen para listados
que GetSavedCharts devuelva solo:
id
nombre
fecha nacimiento
lugar
fecha guardado
quizá Sol/Luna/Ascendente resumidos
dejar ResultSnapshotJson solo para detalle

Resultado:

listado más liviano
mejor performance
contrato más limpio
Bloque 4 — Normalizar interpretation

Hoy tienes mezcla de estructura “flat” y estructura anidada. Eso duplica significado y ya empezó a separar frontend y backend.

Qué haría:

elegir una sola estructura
mi recomendación: estructurada
core
personalPlanets
houses
summary
profiles futuro
el frontend se adapta a esa estructura única
si necesitas compatibilidad momentánea, mantén una transición corta, no permanente

Resultado:

mejor versionado
menos deuda
menos ambigüedad
Bloque 5 — Corregir bug del modo manual

Esto sí lo arreglaría antes de pasar a auth o monetización.

Tu análisis dice que hoy el modo técnico puede terminar enviando 0,0 y offset 0, generando cartas silenciosamente erróneas.

Qué haría:

validación frontend más dura en modo manual
validación backend obligatoria de:
lat/lng no default
offset razonable
no aceptar 0,0 salvo que explícitamente sea válido y consciente
mensajes claros al usuario

Resultado:

evitas datos basura
mejoras confiabilidad del producto
Bloque 6 — Preparar ownership

No metería auth completa todavía, pero sí dejaría preparada la base.

Tu análisis dice que UserId existe pero no aísla nada aún.

Qué haría:

mantener UserId nullable
preparar repositorios y servicios para filtrar por dueño en cuanto entre auth
no exponer todavía toda la API “abierta” como si fuera definitiva
si quieres, agregar un concepto transitorio de OwnerKey o dejar solo la estructura lista para el siguiente sprint

Resultado:

el Sprint 4 entra mucho más limpio
no reescribes persistencia después
