Objetivo del sprint

Dejar AstroReader mostrando una lectura mucho más potente y con identidad clara.

Al cierre del sprint deberías tener:

nuevo interpretations.premium.json
loader tipado del JSON
nueva capa de análisis semántico
nueva capa de composición editorial
nuevo bloque interpretation en el response
frontend mostrando la nueva estructura premium
una primera versión que ya se sienta “producto”
Alcance recomendado del sprint
Sí incluye
JSON premium base
Sol, Luna, Ascendente, Mercurio, Venus, Marte
composición de:
Tu energía central
Tu núcleo
Tu forma de pensar, vincularte y actuar
Lo esencial de tu carta
adaptación del frontend a esos bloques
No incluye todavía
casas premium complejas
aspectos
perfiles múltiples
IA generativa
panel editor
premium/free gating
Orden recomendado
Definir modelos C# del catálogo premium
Crear interpretations.premium.json
Crear provider/loader del JSON
Crear InterpretationAnalyzer
Crear InterpretationComposer
Rediseñar DTO interpretation
Integrar en CalculateNatalChartUseCase
Adaptar frontend
Revisión final
