# AstroReader Backend

Este es el proyecto backend para **AstroReader**, construido con **.NET 8** utilizando una arquitectura limpia y orientada a dominios para permitir un crecimiento modular y mantenible.

## Estructura de Proyectos

La solución está separada en las siguientes capas (proyectos):

- **AstroReader.Api**: ASP.NET Core Web API. Es el punto de entrada de la aplicación. Configura controladores, middleware, inyección de dependencias y la documentación con Swagger. Referencia a *Application* e *Infrastructure*.
- **AstroReader.Application**: Biblioteca de clases. Contiene la lógica de negocio, casos de uso, interfaces de servicios y DTOs (Data Transfer Objects). Es el núcleo del comportamiento del sistema y actúa de orquestador. Referencia a *Domain*.
- **AstroReader.Domain**: Biblioteca de clases. Es el corazón de la aplicación y contiene las entidades, objetos de valor (Value Objects), enumeraciones, e interfaces de repositorios centrales. No tiene referencias a otros proyectos.
- **AstroReader.Infrastructure**: Biblioteca de clases. Implementa detalles técnicos externos tales como acceso a la base de datos, servicios de correo, llamados a APIs externas y repositorios definidos en *Domain*. Referencia a *Application* y *Domain*.
- **AstroReader.AstroEngine**: Biblioteca de clases independiente. Un motor especializado para cálculos u operaciones específicas de astrología. Está construido de manera acoplada cero, listo para ser consumido cuando sea necesario por *Application* o *Infrastructure*.

### Por qué usar esta estructura

Esta arquitectura (Clean Architecture / Onion Architecture) tiene varios beneficios clave:

1. **Desacoplamiento Tecnológico**: Al colocar las reglas del negocio en el *Domain* y la *Application* (sin dependencias a Web, Bases de datos, etc.), puedes actualizar o cambiar el ORM o el framework web sin tener que reescribir la lógica central.
2. **Crecimiento Modular**: Si más adelante *AstroEngine* se vuelve muy pesado, puede ser extraído a su propio microservicio de manera sencilla, ya que no depende del resto del sistema.
3. **Mantenibilidad y Testeabilidad**: Permite crear pruebas unitarias sin depender de la base de datos real o la conexión web. Se pueden introducir "mocks" o implementaciones falsas de *Infrastructure*.
4. **Claridad**: Cualquier desarrollador nuevo sabe exactamente dónde encontrar controladores (API), dónde ver cómo se hace un proceso (Application) y dónde está mapeada la tabla en la base de datos (Infrastructure).

## Instrucciones de Setup CLI

Si en algún momento necesitas reconstruir o generar una estructura similar desde cero utilizando la línea de comandos de .NET (CLI), estos son los comandos utilizados para generar esta solución:

```bash
# 1. Crear la solución
dotnet new sln -n AstroReader

# 2. Crear los proyectos asegurando usar .NET 8
dotnet new webapi -n AstroReader.Api --use-controllers -f net8.0
dotnet new classlib -n AstroReader.Application -f net8.0
dotnet new classlib -n AstroReader.Domain -f net8.0
dotnet new classlib -n AstroReader.Infrastructure -f net8.0
dotnet new classlib -n AstroReader.AstroEngine -f net8.0

# 3. Agregar los proyectos a la solución
dotnet sln add AstroReader.Api/AstroReader.Api.csproj
dotnet sln add AstroReader.Application/AstroReader.Application.csproj
dotnet sln add AstroReader.Domain/AstroReader.Domain.csproj
dotnet sln add AstroReader.Infrastructure/AstroReader.Infrastructure.csproj
dotnet sln add AstroReader.AstroEngine/AstroReader.AstroEngine.csproj

# 4. Configurar las referencias entre proyectos
# Api -> Application, Infrastructure
dotnet add AstroReader.Api/AstroReader.Api.csproj reference AstroReader.Application/AstroReader.Application.csproj AstroReader.Infrastructure/AstroReader.Infrastructure.csproj

# Application -> Domain
dotnet add AstroReader.Application/AstroReader.Application.csproj reference AstroReader.Domain/AstroReader.Domain.csproj

# Infrastructure -> Application, Domain
dotnet add AstroReader.Infrastructure/AstroReader.Infrastructure.csproj reference AstroReader.Application/AstroReader.Application.csproj AstroReader.Domain/AstroReader.Domain.csproj
```

## Estado Actual
* Se agregó un **HealthController** básico en `/api/health`.
* **Swagger** está habilitado y es accesible en desarrollo (URL principal en la web interface al correr).
* **Base de datos** y **Autenticación** aún no están configuradas de manera intencional, según el diseño inicial.
