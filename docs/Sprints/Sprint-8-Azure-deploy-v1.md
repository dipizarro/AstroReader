Sprint — Azure Deploy v1
Objetivo

Dejar AstroReader publicado en Azure con:

frontend estable
backend estable
SQL administrado en Azure
despliegue repetible desde GitHub
Resultado esperado
URL pública del frontend
URL pública de la API
DB en Azure SQL Database
monorepo desplegado correctamente
variables por ambiente resueltas
Decisiones concretas

1. Base de datos

Usaremos Azure SQL Database.

Eso te conviene porque:

vienes de SQL Server local
evitas migrar a otro motor ahora
Microsoft documenta conexión desde .NET usando Microsoft.Data.SqlClient y también escenarios con EF Core sobre Azure SQL Database. 2. Frontend

Azure Static Web Apps
Para tu monorepo, la parte clave será:

app_location: "frontend"
output_location: "dist"

Azure Static Web Apps soporta precisamente esta configuración de build.

3. Backend

Azure App Service Linux + .NET 8
Eso te deja una ruta simple para una API ASP.NET Core. App Service soporta aplicaciones ASP.NET Core y despliegue en Linux.

Lo que debes hacer antes de tocar Azure
Backend
revisar ConnectionStrings
dejar todo sensible vía variables de entorno
revisar CORS
revisar engine astral real
revisar si el path de efemérides depende de rutas locales de Windows
Frontend
confirmar que build local genere dist
revisar VITE_API_BASE_URL
revisar routing SPA si aplica
Base de datos
confirmar que migraciones o scripts están listos para correr en Azure SQL
revisar si tu backend usa SqlConnection / Microsoft.Data.SqlClient / EF Core SQL Server

Backend preparado para Azure App Service

Cambios aplicados

- `appsettings.json` ya no trae una conexión LocalDB como valor productivo.
- `appsettings.Development.json` conserva LocalDB para desarrollo local.
- CORS dejó de usar `AllowAnyOrigin` y ahora lee `Cors:AllowedOrigins`.
- EF Core SQL Server usa `EnableRetryOnFailure` para tolerar fallos transitorios de Azure SQL.
- El `AstroReaderDbContextFactory` puede leer `ConnectionStrings__AstroReaderDb` para migraciones contra Azure SQL.
- HTTPS debe forzarse desde Azure App Service con `HTTPS Only`, evitando redirecciones ambiguas detrás del proxy Linux.

App Settings requeridos en Azure App Service

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__AstroReaderDb=<connection-string-de-Azure-SQL>`
- Alternativa si usas la sección "Connection strings" de App Service: `SQLCONNSTR_AstroReaderDb=<connection-string-de-Azure-SQL>`
- `Cors__AllowedOrigins__0=https://<frontend>.azurestaticapps.net`
- `AstroEngine__SwissEph__CalculationEngine=SwissEph`
- `AstroEngine__SwissEph__EnableSwissEphForNatalCharts=true`
- `AstroEngine__SwissEph__HouseSystem=P`
- `AstroEngine__SwissEph__EphemerisPath=` si se usa el fallback del wrapper, o una ruta Linux existente si se suben efemérides propias.

Variables requeridas en Azure Static Web Apps

- `VITE_API_BASE_URL=https://<backend-app-service>.azurewebsites.net`
- `VITE_MAPBOX_TOKEN=<token>` si se quiere geocoding real en producción.

Comunicación frontend/backend

- El backend solo acepta orígenes declarados en `Cors__AllowedOrigins`.
- El frontend en desarrollo usa `http://localhost:5000` si falta `VITE_API_BASE_URL`.
- El frontend en producción exige `VITE_API_BASE_URL`; no debe caer silenciosamente a localhost.
- La URL de `VITE_API_BASE_URL` no debe terminar en `/`.

Checklist Azure SQL

- Crear Azure SQL Database.
- Permitir acceso desde Azure Services o configurar red privada si se endurece más adelante.
- Ejecutar migraciones contra Azure SQL antes de usar producción.
- Usar connection string con `Encrypt=True`.
- Recomendado para App Service: `Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<db>;Persist Security Info=False;User ID=<user>;Password=<password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;`
- No guardar connection strings reales en el repo.

Riesgos pendientes

- Validar el smoke check de SwissEph en App Service Linux.
- Confirmar si el wrapper funciona bien sin `EphemerisPath` custom en producción.
- Si se usan efemérides propias, empaquetarlas o montarlas en una ruta Linux estable y configurar `AstroEngine__SwissEph__EphemerisPath`.
- Revisar `/api/health/astro-engine` después del deploy: debe reportar `operability.ok=true`, runtime Linux y wrapper version disponible.
- Si `AstroEngine__SwissEph__EphemerisPath` está configurado, el directorio debe existir, ser legible por App Service y contener archivos de efemérides.
