# 🚀 Guía de Instalación y Configuración - IncidenciasTI

## Índice
1. [Requisitos Previos](#requisitos-previos)
2. [Instalación del Backend](#instalación-del-backend)
3. [Instalación del Frontend](#instalación-del-frontend)
4. [Configuración de Bases de Datos](#configuración-de-bases-de-datos)
5. [Inicialización de Datos](#inicialización-de-datos)
6. [Ejecución de la Aplicación](#ejecución-de-la-aplicación)
7. [Verificación del Sistema](#verificación-del-sistema)
8. [Troubleshooting](#troubleshooting)

---

## Requisitos Previos

### Software Necesario
- **Node.js** v18+ ([descargar](https://nodejs.org/))
- **.NET SDK 8.0** ([descargar](https://dotnet.microsoft.com/download/dotnet/8.0))
- **PostgreSQL 14+** ([descargar](https://www.postgresql.org/download/))
- **MongoDB 6.0+** ([descargar](https://www.mongodb.com/try/download/community))
- **Git** (opcional)

### Verificar Instalaciones
```powershell
# En PowerShell o CMD

# Verificar Node.js
node --version
npm --version

# Verificar .NET
dotnet --version

# Verificar PostgreSQL
psql --version

# Verificar MongoDB (si está instalado localmente)
mongod --version
```

---

## Instalación del Backend

### 1. Navegar al Directorio Backend
```powershell
cd d:\proyectosgithub\incidencias-ti\backend\IncidenciasTI.API
```

### 2. Restaurar Dependencias NuGet
```powershell
dotnet restore
```

### 3. Compilar el Proyecto
```powershell
dotnet build
```

**Salida esperada:**
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 4. Verificar la Compilación
```powershell
dotnet build --configuration Release
```

---

## Instalación del Frontend

### 1. Navegar al Directorio Frontend
```powershell
cd d:\proyectosgithub\incidencias-ti\frontend\incidencias-ti-ui
```

### 2. Instalar Dependencias
```powershell
npm install
```

### 3. Verificar Instalación
```powershell
npm list vue axios
```

---

## Configuración de Bases de Datos

### PostgreSQL - Crear Base de Datos

#### Opción 1: Usando pgAdmin (GUI)

1. Abrir **pgAdmin**
2. Expandir **Servers** → **PostgreSQL**
3. Click derecho en **Databases** → **Create** → **Database**
4. Nombre: `incidencias_ti`
5. Click en **Save**

#### Opción 2: Usando Command Line (PowerShell)

```powershell
# Conectar a PostgreSQL (pedirá contraseña)
psql -U postgres

# En la consola psql
CREATE DATABASE incidencias_ti;
\l  # Verificar que la BD fue creada
\q  # Salir
```

### MongoDB - Crear Base de Datos

#### Opción 1: MongoDB Compass (GUI)

1. Abrir **MongoDB Compass**
2. Conectar a `mongodb://localhost:27017`
3. Crear nueva base de datos:
   - Database Name: `IncidenciasLogs`
   - Collection Name: `IncidenciaLogs`
4. Crear otra colección:
   - Ir a `IncidenciasLogs`
   - Click en **+** → **Create Collection**
   - Nombre: `IncidenciasDirect`

#### Opción 2: Usando MongoDB Shell

```powershell
# Abrir MongoDB Shell
mongosh

# Crear base de datos (se crea automáticamente)
use IncidenciasLogs

# Crear colecciones
db.createCollection("IncidenciaLogs")
db.createCollection("IncidenciasDirect")

# Verificar
show collections
```

---

## Inicialización de Datos

### 1. Crear Tabla en PostgreSQL

Ejecutar el script SQL `SQL_INIT.sql`:

```powershell
# Navegate to the project root
cd d:\proyectosgithub\incidencias-ti

# Ejecutar el script
psql -U postgres -d incidencias_ti -f backend/IncidenciasTI.API/Data/SQL_INIT.sql
```

**Verificar:**
```powershell
psql -U postgres -d incidencias_ti

# En la consola psql
\dt  # Listar tablas
SELECT * FROM "Incidencias";  # Ver datos
\q
```

### 2. Aplicar Migraciones de EF Core

```powershell
cd d:\proyectosgithub\incidencias-ti\backend\IncidenciasTI.API

# Aplicar migraciones
dotnet ef database update
```

**Salida esperada:**
```
Done. Successfully updated database associated with DbContext 'AppDbContext'.
```

### 3. Importar Datos a MongoDB

#### Para la colección IncidenciaLogs:

```powershell
mongoimport --uri mongodb://localhost:27017/IncidenciasLogs `
            --collection IncidenciaLogs `
            --file backend/IncidenciasTI.API/Data/MONGO_SEED.json `
            --jsonArray
```

#### Para la colección IncidenciasDirect:

```powershell
mongoimport --uri mongodb://localhost:27017/IncidenciasLogs `
            --collection IncidenciasDirect `
            --file backend/IncidenciasTI.API/Data/MONGO_SEED_DIRECT.json `
            --jsonArray
```

**Verificar en MongoDB Compass:**
1. Conectar a `mongodb://localhost:27017`
2. Ver base de datos `IncidenciasLogs`
3. Cada colección debe mostrar los documentos importados

---

## Ejecución de la Aplicación

### 1. Iniciar PostgreSQL

**Si está instalado como servicio:**
```powershell
# Ya debe estar ejecutándose automáticamente
# Verificar:
Get-Service postgres*
```

**Si no está ejecutándose:**
```powershell
# En Windows, abrir Services y buscar PostgreSQL
# O ejecutar:
"C:\Program Files\PostgreSQL\15\bin\pg_ctl.exe" start -D "C:\Program Files\PostgreSQL\15\data"
```

### 2. Iniciar MongoDB

```powershell
# En una terminal nueva
mongod

# O si está como servicio:
net start MongoDB
```

### 3. Iniciar el Backend API

```powershell
cd d:\proyectosgithub\incidencias-ti\backend\IncidenciasTI.API

# Ejecutar en modo desarrollo
dotnet run

# O compilar y ejecutar
dotnet build
dotnet bin/Debug/net8.0/IncidenciasTI.API.exe
```

**Salida esperada:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5268
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to exit.
```

### 4. Iniciar el Frontend

**En otra terminal:**

```powershell
cd d:\proyectosgithub\incidencias-ti\frontend\incidencias-ti-ui

# Ejecutar en modo desarrollo
npm run dev
```

**Salida esperada:**
```
VITE v4.x.x  ready in xxx ms

➜  Local:   http://localhost:5173/
```

---

## Verificación del Sistema

### Checklist de Verificación

#### Backend
- [ ] API escucha en `http://localhost:5268`
- [ ] Swagger docs disponibles en `http://localhost:5268/swagger`
- [ ] Base de datos PostgreSQL conectada
- [ ] MongoDB conectado (si ves logs sin error)
- [ ] Migraciones aplicadas correctamente

#### Frontend
- [ ] Aplicación escucha en `http://localhost:5173`
- [ ] No hay errores en la consola del navegador
- [ ] El `.env` contiene `VITE_API_URL=http://localhost:5268/api`
- [ ] Los datos cargados desde el backend

#### Bases de Datos
- [ ] PostgreSQL: tabla "Incidencias" con datos
- [ ] MongoDB: colecciones "IncidenciaLogs" y "IncidenciasDirect" con documentos

### Test de Conectividad

```powershell
# Verificar que el API responde
Invoke-WebRequest -Uri "http://localhost:5268/api/incidencias" -Method GET

# Debe retornar status 200 y un JSON con incidencias
```

---

## Endpoints Disponibles

### Incidencias (SQL)
```
GET    /api/incidencias                    # Obtener todas
GET    /api/incidencias/{id}               # Obtener por ID
POST   /api/incidencias                    # Crear
PUT    /api/incidencias/{id}               # Actualizar
DELETE /api/incidencias/{id}               # Eliminar
POST   /api/incidencias/sync               # Sincronizar desde MongoDB
```

### Incidencias (MongoDB via Logs)
```
GET    /api/mongo/incidencias              # Obtener todas desde logs
POST   /api/mongo/incidencias              # Crear (genera log)
PUT    /api/mongo/incidencias/{id}         # Actualizar (genera log)
DELETE /api/mongo/incidencias/{id}         # Eliminar (genera log)
POST   /api/mongo/incidencias/sync         # Sincronizar logs a SQL
```

### Incidencias (MongoDB Directo)
```
GET    /api/mongo/direct/incidencias       # Obtener todas
POST   /api/mongo/direct/incidencias       # Crear
PUT    /api/mongo/direct/incidencias/{id}  # Actualizar
DELETE /api/mongo/direct/incidencias/{id}  # Eliminar
POST   /api/mongo/direct/incidencias/sync  # Sincronizar a SQL
```

### Estadísticas
```
GET    /api/estadisticas/resumen           # Resumen general
GET    /api/estadisticas/criticas          # Incidencias críticas
GET    /api/estadisticas/recientes         # Actividades recientes
GET    /api/estadisticas/distribucion-temporal  # Gráficas por día
GET    /api/estadisticas/ranking-estados   # Ranking de estados
GET    /api/estadisticas/health            # Health check del sistema
```

---

## Uso de la Aplicación

### Acceder a la UI
1. Abrir navegador
2. Ir a `http://localhost:5173`
3. Ver incidencias listadas
4. Clickear en "Dashboard" en la barra de navegación para ver estadísticas

### Operaciones Básicas

#### Crear Incidencia
1. Completar el formulario
2. Click en "Agregar Incidencia"
3. Ver la nueva incidencia en la lista

#### Editar Incidencia
1. Hacer click en la tarjeta de incidencia
2. El modo edición se activa automáticamente
3. Modificar campos
4. Guardar cambios

#### Eliminar Incidencia
1. Hacer click en el botón eliminar (🗑️)
2. Confirmar eliminación
3. La incidencia desaparece de la lista

#### Ver Estadísticas
1. Hacer click en "Dashboard" en la navegación superior
2. Ver gráficas y métricas en tiempo real
3. Las estadísticas se actualizan automáticamente cada 30 segundos

#### Sincronizar Datos
1. En Swagger (`http://localhost:5268/swagger`)
2. Ejecutar `POST /api/incidencias/sync`
3. Hace que los datos de MongoDB se apliquen a PostgreSQL

---

## Troubleshooting

### Error: "Connection refused" en Frontend
**Causa:** API no está escuchando o puerto incorrecto

**Solución:**
```powershell
# 1. Verificar que el backend está ejecutándose
Get-NetTCPConnection -LocalPort 5268

# 2. Verificar el .env en frontend
cat d:\proyectosgithub\incidencias-ti\frontend\incidencias-ti-ui\.env

# 3. Debe contener:
# VITE_API_URL=http://localhost:5268/api

# 4. Reiniciar frontend
npm run dev
```

### Error: "Cannot connect to PostgreSQL"
**Causa:** PostgreSQL no está ejecutándose

**Solución:**
```powershell
# 1. Verificar si el servicio está activo
Get-Service postgres*

# 2. Si no está corriendo, iniciar:
Start-Service postgresql-x64-15

# 3. Verificar conexión:
psql -U postgres -d incidencias_ti
```

### Error: "MongoDB connection timeout"
**Causa:** MongoDB no está ejecutándose

**Solución:**
```powershell
# 1. Iniciar MongoDB:
mongod

# 2. O como servicio:
net start MongoDB

# 3. Verificar con MongoDB Compass
```

### Error: "Table 'Incidencias' does not exist"
**Causa:** Migraciones no aplicadas

**Solución:**
```powershell
cd d:\proyectosgithub\incidencias-ti\backend\IncidenciasTI.API

# Aplicar migraciones
dotnet ef database update

# Si hay errores, crear tabla manualmente:
psql -U postgres -d incidencias_ti -f SQL_INIT.sql
```

### Error: "ERR_NETWORK in Browser Console"
**Causa:** API no está ejecutándose o puerto incorrecto

**Solución:**
1. Verificar que `dotnet run` está ejecutándose en backend
2. Verificar puerto 5268 está libre: `netstat -ano | findstr :5268`
3. Verificar `.env` en frontend
4. Limpiar caché del navegador (Ctrl+Shift+Del)

### Datos no aparecen en MongoDB Compass
**Causa:** Los datos no fueron importados correctamente

**Solución:**
```powershell
# Verificar que los archivos existen
Test-Path "backend/IncidenciasTI.API/Data/MONGO_SEED.json"
Test-Path "backend/IncidenciasTI.API/Data/MONGO_SEED_DIRECT.json"

# Reimportar:
mongoimport --uri mongodb://localhost:27017/IncidenciasLogs `
            --collection IncidenciaLogs `
            --file backend/IncidenciasTI.API/Data/MONGO_SEED.json `
            --jsonArray --drop
```

---

## Desarrollo y Debugging

### Habilitar Logs Detallados en Backend

En `Program.cs`, cambiar nivel de logging:

```csharp
builder.Logging.SetMinimumLevel(LogLevel.Debug);
```

### Usar Swagger UI

1. Ir a `http://localhost:5268/swagger`
2. Explorar y probar todos los endpoints
3. Ver respuestas JSON formateadas

### Debug en VS Code

1. En backend: Crear `.vscode/launch.json`:
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/backend/IncidenciasTI.API/bin/Debug/net8.0/IncidenciasTI.API.dll",
      "args": [],
      "cwd": "${workspaceFolder}/backend/IncidenciasTI.API",
      "stopAtEntry": false,
      "console": "internalConsole",
      "preLaunchTask": "build"
    }
  ]
}
```

2. Presionar F5 para iniciar debug

---

## Performance Optimization

### Frontend
- Los componentes están optimizados con React hooks
- CSS variables permiten cambios de tema dinámicos
- Grid responsivo se adapta a cualquier pantalla

### Backend
- Las consultas usan `.ToListAsync()` para no bloquear
- Índices en PostgreSQL para búsquedas rápidas
- Sincronización batch para múltiples registros

---

## Próximos Pasos

- [ ] Implementar autenticación (JWT)
- [ ] Agregar validación de datos en DTOs
- [ ] Crear tests unitarios
- [ ] Documentar API con OpenAPI 3.0
- [ ] Implementar caché con Redis
- [ ] Dockerizar la aplicación

---

## Soporte

Si encuentras problemas:

1. Revisar el archivo de logs: `backend/IncidenciasTI.API/bin/Debug/net8.0/`
2. Abrir Developer Tools en navegador (F12)
3. Revisar MongoDB Compass para ver datos sincronizados
4. Verificar que todos los servicios están ejecutándose

---

**Última actualización:** Enero 2025  
**Versión:** 1.0
