Objetivo:
sumar contexto humano al motor actual sin reemplazar la carta astral como núcleo.

La lógica de esta fase sería:

la carta sigue siendo la base
el perfil personal agrega contexto
la lectura futura podrá dialogar con ambos
dejamos el sistema listo para una capa IA más adelante, sin implementarla aún
Qué vamos a construir en esta fase
Resultado esperado

Al cierre de esta fase deberías tener:

modelo de usuario/perfil
formulario de onboarding enriquecido
preguntas asistidas, no solo texto libre
persistencia del perfil junto con la carta
lectura premium base + contexto del usuario
uso del nombre del usuario de forma elegante
una primera capa de “tu carta y cómo hoy te percibes”
Decisión importante de producto

No lo haría como:

login pesado obligatorio antes de ver valor

Lo haría como:

opción recomendada

onboarding premium asistido antes o justo después de la primera lectura

Eso significa:

nombre
fecha/hora/lugar
2 o 3 preguntas guiadas
texto libre corto opcional

Así no metes demasiada fricción.

Qué datos pediría
Bloque 1 — identidad básica
nombre
fecha de nacimiento
hora
lugar
Bloque 2 — autopercepción asistida
qué sientes que más te define hoy
qué te cuesta más hoy
qué te gustaría entender mejor de ti
Bloque 3 — texto libre corto
“si tuvieras que describirte en pocas líneas, ¿cómo lo harías?”

Pero con ayuda visual y límite de caracteres.

Qué no haría todavía

No haría aún:

autenticación compleja
cuentas sociales
chat IA
perfil psicológico hiper profundo
tests de personalidad largos

Primero: perfil útil, corto y bien guiado.

Arquitectura recomendada
Backend
UserProfile o AstroProfile
guardar perfil junto con la carta o asociado a usuario futuro
casos de uso para crear/leer perfil
Frontend
flujo de onboarding
formulario asistido paso a paso
UX clara, no formulario eterno
Interpretación

En esta fase no reescribiría toda la lectura.
Haría primero una capa simple:

usar el nombre
priorizar ciertos bloques según foco del usuario
agregar un bloque nuevo:
Tu carta y cómo hoy te percibes
Orden recomendado de implementación
Sprint 1

Modelo y persistencia del perfil

Sprint 2

Formulario asistido en frontend

Sprint 3

Conectar perfil con la lectura premium

Sprint 4

Pulido editorial y UX del onboarding
