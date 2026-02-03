# Sistema de Gestión de Incidencias TI - Integración PostgreSQL ↔ MongoDB

## 📋 Tabla de Contenidos
1. [Introducción](#introducción)
2. [Descripción del Proyecto](#descripción-del-proyecto)
3. [Requisitos Técnicos](#requisitos-técnicos)
4. [Arquitectura del Sistema](#arquitectura-del-sistema)
5. [Modelo de Datos](#modelo-de-datos)
6. [Reglas de Transformación](#reglas-de-transformación)
7. [Sincronización de Datos](#sincronización-de-datos)
8. [Instalación y Configuración](#instalación-y-configuración)
9. [Uso del Sistema](#uso-del-sistema)
10. [Conclusiones](#conclusiones)

---

## 📌 Introducción

Este proyecto implementa una **arquitectura distribuida híbrida** que integra dos motores de bases de datos complementarios: **PostgreSQL** (relacional) y **MongoDB** (documental). El sistema demuestra cómo gestionar información en ambos motores simultáneamente, manteniendo consistencia mediante mecanismos de sincronización bidireccional.

**Caso de Uso:** Gestión integral de incidencias de Tecnologías de la Información (TI), permitiendo reportar, rastrear y resolver problemas de infraestructura de manera distribuida y resiliente.

---

## 🎯 Descripción del Proyecto

El sistema implementa un **módulo de gestión de incidencias TI** que permite:

- **Crear** nuevas incidencias con título, descripción, prioridad y estado
- **Consultar** incidencias con filtros por prioridad y estado
- **Actualizar** incidencias durante su resolución
- **Eliminar** incidencias cuando se cierran o se registran erróneamente
- **Sincronizar** datos entre PostgreSQL y MongoDB con resolución automática de conflictos
- **Registrar auditoría** completa mediante logs estructurados
- **Visualizar estadísticas** de incidencias activas y resueltas

### Características Diferenciadoras:

✅ **Arquitectura Dual:**
- PostgreSQL para datos estructurados y normalizados
- MongoDB para historiales, logs y auditoría completa

✅ **Sincronización Inteligente:**
- Basada en `UltimaActualizacion` (timestamp)
- Resolución automática de conflictos (última modificación gana)
- Logs detallados de cada operación

✅ **Frontend Moderno:**
- Interfaz React con componentes reutilizables
- Diseño responsivo con estilos centralizados
- Alertas visuales y feedback en tiempo real

✅ **API REST Completa:**
- Documentación Swagger integrada
- CORS habilitado para desarrollo
- Manejo robusto de errores

---

## 🔧 Requisitos Técnicos

### Backend
- **.NET 8.0** (ASP.NET Core Web API)
- **Entity Framework Core 8.0** + Npgsql 8.0 (PostgreSQL)
- **MongoDB.Driver 3.6.0**
- **Swagger/Swashbuckle 6.6.2**

### Base de Datos
- **PostgreSQL 14+**
- **MongoDB 5.0+** (Community o Atlas)

### Frontend
- **React 18+**
- **Vite 5.0+**
- **Axios** (HTTP client)

### Herramientas de Desarrollo
- Visual Studio Code
- PostgreSQL pgAdmin
- MongoDB Compass
- Postman o Thunder Client (para testing API)

---

## 🏗️ Arquitectura del Sistema

```
┌─────────────────────────────────────────────────────────────┐
│                      FRONTEND (React)                        │
│  ┌──────────────┬──────────────┬────────────┬─────────────┐ │
│  │   Header     │    Form      │  FilterBar │   Cards     │ │
│  └──────────────┴──────────────┴────────────┴─────────────┘ │
│  ┌────────────────────────────────────────────────────────┐ │
│  │              Estilos Centralizados (CSS)               │ │
│  └────────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                            ↕ HTTP REST
┌─────────────────────────────────────────────────────────────┐
│               BACKEND (ASP.NET Core Web API)                │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Controladores REST                                   │  │
│  │  • IncidenciasController (SQL)                        │  │
│  │  • IncidenciasMongoController (Logs)                  │  │
│  │  • IncidenciasMongoDirectController (Direct)          │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Servicios de Negocio                                │  │
│  │  • SyncService (Mongo→SQL vía Logs)                  │  │
│  │  • MongoToSqlSyncService (Mongo→SQL Directo)         │  │
│  │  • LogService (Auditoría)                             │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  DTOs & Models                                       │  │
│  │  • IncidenciaDto, CreateIncidenciaDto, etc.          │  │
│  │  • IncidenciaSql, IncidenciaMongo, IncidenciaLog     │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
        ↓                                       ↓
┌───────────────────┐              ┌───────────────────┐
│   PostgreSQL      │              │    MongoDB        │
│                   │              │                   │
│ Tabla: Incidencias│              │ Collections:      │
│ • Id (PK, int)    │              │ • IncidenciaLogs  │
│ • GuidId (Guid)   │              │ • IncidenciasDirect
│ • Titulo          │              │ • Auditoría       │
│ • Descripcion     │              │                   │
│ • Estado          │              │ Documentos JSON   │
│ • Prioridad       │              │ con estructura    │
│ • FechaCreacion   │              │ flexible          │
│ • UltimaActualizacion           │                   │
└───────────────────┘              └───────────────────┘
```

---

## 📊 Modelo de Datos

### PostgreSQL - Tabla `Incidencias`

```sql
CREATE TABLE "Incidencias" (
  "Id" SERIAL PRIMARY KEY,
  "GuidId" UUID NOT NULL UNIQUE,
  "Titulo" VARCHAR(200) NOT NULL,
  "Descripcion" TEXT NOT NULL,
  "Estado" VARCHAR(50) NOT NULL,
  "Prioridad" VARCHAR(50) NOT NULL,
  "FechaCreacion" TIMESTAMP NOT NULL,
  "UltimaActualizacion" TIMESTAMP NOT NULL
);
```

### MongoDB - Colección `IncidenciaLogs`

```json
{
  "_id": ObjectId("507f1f77bcf86cd799439011"),
  "incidenciaId": 1,
  "acción": "Creación",
  "usuario": "admin",
  "fecha": ISODate("2026-02-03T10:30:00Z"),
  "datos": {
    "titulo": "Servidor SQL caído",
    "descripcion": "El servidor principal de BD no responde",
    "estado": "Abierta",
    "prioridad": "Crítica",
    "fechaCreacion": ISODate("2026-02-03T10:30:00Z"),
    "ultimaActualizacion": ISODate("2026-02-03T10:30:00Z")
  }
}
```

### MongoDB - Colección `IncidenciasDirect`

```json
{
  "_id": ObjectId("507f1f77bcf86cd799439012"),
  "guidId": "550e8400-e29b-41d4-a716-446655440000",
  "titulo": "Red caída",
  "descripcion": "La red corporativa no funciona",
  "estado": "En Proceso",
  "prioridad": "Alta",
  "fechaCreacion": ISODate("2026-02-03T11:00:00Z"),
  "ultimaActualizacion": ISODate("2026-02-03T11:15:00Z")
}
```

---

## 🔄 Reglas de Transformación

### PostgreSQL → MongoDB (IncidenciaLog)

| Campo SQL | Campo Log | Transformación |
|-----------|-----------|-----------------|
| Id | incidenciaId | Directa |
| Titulo | datos.titulo | Anidado en datos |
| Descripcion | datos.descripcion | Anidado en datos |
| Estado | datos.estado | Anidado en datos |
| Prioridad | datos.prioridad | Anidado en datos |
| FechaCreacion | datos.fechaCreacion | Directa ISO |
| UltimaActualizacion | datos.ultimaActualizacion | Directa ISO |
| (nuevo) | acción | Generado: "Creación", "Actualización", "Eliminación" |
| (nuevo) | usuario | Capturado del request |
| (nuevo) | fecha | DateTime.UtcNow |

### MongoDB → PostgreSQL (IncidenciaMongo → IncidenciaSql)

| Campo MongoDB | Campo SQL | Transformación |
|---------------|-----------|-----------------|
| guidId | GuidId | Directa |
| titulo | Titulo | Directa |
| descripcion | Descripcion | Directa |
| estado | Estado | Directa |
| prioridad | Prioridad | Directa |
| fechaCreacion | FechaCreacion | Directa |
| ultimaActualizacion | UltimaActualizacion | Directa (para conflictos) |

### Regla de Conflictos

**Si existe duplicado por GuidId:**
```
SI MongoDB.ultimaActualizacion > SQL.ultimaActualizacion
  ENTONCES aplicar valores de MongoDB en SQL
  SINO mantener valores de SQL
```

---

## 🔀 Sincronización de Datos

### Flujo 1: Logs → SQL (`SyncService`)

```
POST /api/incidencias (CREATE en SQL)
  ↓
Crear IncidenciaLog en MongoDB con acción="Creación"
  ↓
POST /api/incidencias/sync
  ↓
Leer todos los logs de IncidenciaLogs
  ↓
Para cada log (ordenado por fecha):
  ├─ Si acción="Creación" → INSERT en SQL
  ├─ Si acción="Actualización" → UPDATE en SQL
  └─ Si acción="Eliminación" → DELETE en SQL
  ↓
SaveChanges en EF Core
```

### Flujo 2: Mongo Directo → SQL (`MongoToSqlSyncService`)

```
PUT /api/mongo/direct/incidencias/{guid} (UPDATE en MongoDB)
  ↓
POST /api/mongo/direct/incidencias/sync
  ↓
Leer todos los docs de IncidenciasDirect
  ↓
Para cada MongoDB doc:
  ├─ Buscar en SQL por GuidId
  ├─ SI no existe → INSERT en SQL
  └─ SI existe:
      └─ SI MongoDB.ultimaActualizacion > SQL.ultimaActualizacion
        └─ UPDATE en SQL
  ↓
SaveChanges en EF Core
```

---

## 🚀 Instalación y Configuración

### 1. Requisitos Previos

```bash
# Verificar .NET 8
dotnet --version

# Verificar PostgreSQL (desde PowerShell o cmd)
psql --version

# Verificar MongoDB
mongod --version
```

### 2. Configurar PostgreSQL

```bash
# Crear BD
createdb incidencias_ti -U postgres

# Ejecutar script SQL (incluido)
psql -U postgres -d incidencias_ti -f backend/SQL_INIT.sql
```

### 3. Configurar MongoDB

```bash
# Importar colecciones (desde mongosh)
mongoimport --uri "mongodb://localhost:27017/IncidenciasLogs" \
  --collection IncidenciaLogs \
  --file backend/MONGO_SEED.json \
  --jsonArray

mongoimport --uri "mongodb://localhost:27017/IncidenciasLogs" \
  --collection IncidenciasDirect \
  --file backend/MONGO_SEED_DIRECT.json \
  --jsonArray
```

### 4. Configurar Backend

```bash
cd backend/IncidenciasTI.API

# Variables de entorno
$env:PG_PASSWORD = "tu_password_postgres"

# Restaurar dependencias
dotnet restore

# Aplicar migraciones
dotnet ef database update

# Ejecutar
dotnet run
```

API estará en: `http://localhost:5268`

Swagger: `http://localhost:5268/swagger`

### 5. Configurar Frontend

```bash
cd frontend/incidencias-ti-ui

# Instalar dependencias
npm install

# Ejecutar desarrollo
npm run dev
```

Frontend estará en: `http://localhost:5173`

---

## 📖 Uso del Sistema

### Endpoints Principales

#### **SQL (PostgreSQL)**
- `GET /api/incidencias` - Listar todas
- `GET /api/incidencias/{id}` - Obtener por ID
- `POST /api/incidencias` - Crear
- `PUT /api/incidencias/{id}` - Actualizar
- `DELETE /api/incidencias/{id}` - Eliminar
- `POST /api/incidencias/sync` - Sincronizar desde logs

#### **MongoDB Logs**
- `GET /api/mongo/incidencias` - Listar desde logs
- `GET /api/mongo/incidencias/{id}` - Obtener desde logs
- `POST /api/mongo/incidencias` - Crear log
- `PUT /api/mongo/incidencias/{id}` - Actualizar y loguear
- `DELETE /api/mongo/incidencias/{id}` - Eliminar y loguear
- `POST /api/mongo/incidencias/sync` - Sincronizar logs→SQL

#### **MongoDB Directo**
- `GET /api/mongo/direct/incidencias` - Listar desde IncidenciasDirect
- `GET /api/mongo/direct/incidencias/{guid}` - Obtener por GUID
- `POST /api/mongo/direct/incidencias` - Crear directo en MongoDB
- `PUT /api/mongo/direct/incidencias/{guid}` - Actualizar directo
- `DELETE /api/mongo/direct/incidencias/{guid}` - Eliminar directo
- `POST /api/mongo/direct/incidencias/sync` - Sincronizar MongoDB→SQL

### Ejemplo de Flujo Completo

```bash
# 1. Crear incidencia en SQL
POST /api/incidencias
{
  "titulo": "Servidor caído",
  "descripcion": "El servidor principal no responde",
  "prioridad": "Crítica",
  "usuario": "Admin"
}

# 2. Verificar log en MongoDB (mediante Compass)
# Colección: IncidenciaLogs
# Debería existir con acción="Creación"

# 3. Sincronizar desde logs a SQL
POST /api/incidencias/sync

# 4. Verificar que el dato está en SQL
GET /api/incidencias

# 5. Crear desde MongoDB directo
POST /api/mongo/direct/incidencias
{
  "titulo": "Red congestionada",
  "descripcion": "Ancho de banda al 95%",
  "prioridad": "Alta",
  "usuario": "User"
}

# 6. Sincronizar desde MongoDB a SQL
POST /api/mongo/direct/incidencias/sync

# 7. Verificar en SQL
GET /api/incidencias
```

---

## ✅ Requisitos Cumplidos

### Funcionales Mínimos
- ✅ **CRUD en PostgreSQL y MongoDB** - 100%
- ✅ **Transformación de Datos** - Bidireccional
- ✅ **Sincronización** - Manual con conflicto resolution
- ✅ **Aplicación REST + Frontend** - Completa

### Entregables Obligatorios
- ✅ **Documento formal** - Este README
- ✅ **Scripts SQL** - `SQL_INIT.sql`
- ✅ **Colecciones MongoDB** - `MONGO_SEED.json`
- ✅ **Modelo de datos** - Documentado arriba
- ✅ **Arquitectura** - Diagrama incluido
- ✅ **Reglas de transformación** - Tabla explicada

---

## 🎓 Conclusiones

### Logros Alcanzados

1. **Arquitectura Escalable:** Separación clara entre datos relacionales y documentales
2. **Sincronización Inteligente:** Manejo automático de conflictos basado en timestamps
3. **Auditoría Completa:** Cada operación queda registrada en MongoDB
4. **Interfaz Profesional:** Frontend moderno y responsivo
5. **API REST Documentada:** Swagger integrado para fácil testing

### Decisiones Metodológicas Clave

| Decisión | Razón |
|----------|-------|
| PostgreSQL para datos activos | Rendimiento en consultas normalizadas |
| MongoDB para logs/auditoría | Flexibilidad de esquema, escalabilidad horizontal |
| UltimaActualizacion en ambos | Resolución determinística de conflictos |
| GUID compartido | Sincronización confiable entre motores |
| React + Vite | Desarrollo rápido, bundling optimizado |

### Posibles Mejoras Futuras

- 🔐 Autenticación y autorización (JWT)
- 📊 Dashboard de analíticas avanzadas
- 🔔 Notificaciones en tiempo real (WebSockets)
- 📱 Aplicación móvil nativa
- 🧪 Suite completa de tests (Unit, Integration)
- 🐳 Containerización con Docker/Compose
- 📈 Monitoring y alertas (Prometheus, Grafana)
- 🔄 Sincronización automática con cambios en tiempo real

### Recomendaciones

1. **Producción:** Implementar autenticación antes de deployar
2. **Escalabilidad:** Evaluar caché (Redis) para consultas frecuentes
3. **Reliability:** Agregar retry logic y circuit breakers en sincronización
4. **Monitoring:** Implementar logging centralizado y métricas
5. **Testing:** Desarrollar tests automatizados (mínimo 70% cobertura)

---

## 📞 Contacto y Soporte

Para reportar issues o sugerencias, contactar al equipo de desarrollo.

**Última actualización:** Febrero 3, 2026  
**Versión:** 1.0.0  
**Status:** ✅ Producción
