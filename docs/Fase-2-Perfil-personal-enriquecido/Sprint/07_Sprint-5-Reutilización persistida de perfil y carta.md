Sprint — Reutilización persistida de perfil y carta
Objetivo

Hacer que perfil y carta no solo se creen correctamente, sino que puedan:

reencontrarse
reutilizarse
rehidratar la experiencia
y sostener una lectura contextualizada duradera
Qué debe resolver

1. Rehidratar perfil desde carta guardada

Cuando abras una carta guardada:

debe poder recuperarse el perfil asociado
o llegar un resumen del perfil ya incluido en el DTO 2. Volver atómico el guardado

Guardar carta + enlazar perfil en una sola unidad de trabajo.

3. Reducir agresividad de contexto pendiente

location.state primero
localStorage solo como fallback explícito

4. Mejorar el snapshot de datos humanos
   nombre real del lugar también en modo manual si existe
   timezoneIana cuando venga geocodificado
5. Hacer visible la desvinculación

Si los datos cambian y el perfil deja de aplicar:

mostrar aviso claro y corto
no dejar que el usuario lo descubra implícitamente 6. Empezar a reutilizar perfiles
listar perfiles
seleccionar perfil
retomar contexto
