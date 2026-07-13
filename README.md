# Peliculas API

API REST construida con ASP.NET Core para gestionar películas, géneros, usuarios, listas, valoraciones y reseñas.

## Contenido

- `Peliculas/Program.cs`: configuración principal de servicios y pipeline.
- `Peliculas/Config/AppDbContext.cs`: contexto de Entity Framework Core.
- `Peliculas/Services/`: lógica de negocio.
- `Peliculas/Repositories/`: acceso a datos.
- `Peliculas/Controllers/`: endpoints de la API.

## Requisitos

- .NET SDK 8.0+ instalado
- SQL Server accesible desde el equipo local o remoto

## Instalación

1. Abrir la terminal en la raíz del repositorio:
   ```bash
   cd c:\Users\escob\programacion\Peliculas-TP
   ```
2. Restaurar paquetes:
   ```bash
   dotnet restore Peliculas/Peliculas.csproj
   ```

## Ejecutar en desarrollo

Con la configuración de `launchSettings.json`, se puede iniciar en modo Development:

```bash
dotnet run --project Peliculas/Peliculas.csproj
```

Esto levantará la API y habilitará Swagger UI en el entorno de desarrollo.

## Generar build

Para compilar la aplicación para producción:

```bash
dotnet build Peliculas/Peliculas.csproj --configuration Release
```

## Configuración

### Connection string

La cadena de conexión está en `Peliculas/appsettings.json` bajo `ConnectionStrings:devConnection`.

Ejemplo actual:

```json
"ConnectionStrings": {
  "devConnection": "Data Source=DESKTOP-2LE5L51;Initial Catalog=PeliculasAPI;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;"
}
```

Cambia `Data Source`, `Initial Catalog` y la configuración de seguridad según tu servidor SQL.

### Secret JWT

El token JWT se obtiene desde `Peliculas/appsettings.json` en `Secrets:jwt`.

Ejemplo actual:

```json
"Secrets": {
  "jwt": "ESTA_CLAVE_DEBE_TENER_AL_MENOS_32_CARACTERES_OK"
}
```

Debes reemplazar este valor por un secreto seguro en producción.

### Variables de entorno opcionales

- `ASPNETCORE_ENVIRONMENT`: define el entorno (por ejemplo, `Development` o `Production`).

Si prefieres no usar `appsettings.json` para el JWT o la cadena de conexión, puedes definir estas variables también en tu entorno o en un archivo de usuario.

## Notas sobre `Program.cs`

- Registra servicios y repositorios con inyección de dependencias.
- Configura autenticación con JWT y cookies.
- Habilita validación de modelo personalizada con respuestas estructuradas.
- Activa CORS amplio para permitir cualquier origen, método y cabecera.
- Activa Swagger solo en desarrollo.

## Uso básico

1. Iniciar la API.
2. Abrir Swagger UI en `https://localhost:7003/swagger` o `http://localhost:5007/swagger`.
3. Probar los endpoints disponibles.

## Ejemplo de comandos

```bash
cd c:\Users\escob\programacion\Peliculas-TP
dotnet restore Peliculas/Peliculas.csproj
dotnet run --project Peliculas/Peliculas.csproj
```

---
