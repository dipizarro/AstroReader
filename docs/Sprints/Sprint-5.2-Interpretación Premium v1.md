Sprint — Hardening de cobertura premium
Objetivo del sprint

Dejar AstroReader en un estado donde:

el sistema no maquille una lectura premium cuando no la tiene
el backend pueda componer parcialmente
analyzer y composer compartan una resolución común
el frontend muestre de forma honesta si la lectura es:
completa
parcial
fallback
la base quede lista para luego meter el catálogo completo
Resultado esperado al cierre

Al terminar este sprint deberías tener:

coverageStatus real en backend
composición parcial funcionando
PremiumInterpretationContext o resolver común
menos duplicación entre analyzer y composer
frontend capaz de distinguir calidad/cobertura de la lectura
fallback solo cuando realmente corresponde
Alcance recomendado
Sí incluye
coverage status
partial composition
resolver/contexto común
rediseño de flujo premium para no caer en fallback total
UI honesta
errores/telemetría mínima si aporta
No incluye todavía
catálogo completo de 72 entradas
reescritura profunda del composer
nuevas secciones premium
perfiles múltiples
IA generativa
Orden recomendado
Definir coverageStatus
Crear resolver/contexto común
Permitir composición parcial
Integrar nuevo flujo en CalculateNatalChartUseCase
Exponer cobertura al frontend
Ajustar UI para distinguir estados
Revisar el sprint
