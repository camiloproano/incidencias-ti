# 🏗️ Arquitectura del Sistema - IncidenciasTI

## 1. Diagrama General de Arquitectura

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENTE (NAVEGADOR)                      │
│                     React + Vite @ :5173                        │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │ Components: Header, Navigation, Dashboard, Incidencias    │  │
│  │ State Management: React Hooks (useState, useEffect)       │  │
│  │ Styling: CSS3 + CSS Variables                             │  │
│  └───────────────────────────────────────────────────────────┘  │
└────────────────────────┬──────────────────────────────────────┘
                         │ HTTP/HTTPS
                         │ Axios Client
                         │
        ┌────────────────▼────────────────┐
        │   INTERNET / NETWORK            │
        │   (localhost para desarrollo)   │
        └────────────────┬────────────────┘
                         │
        ┌────────────────▼──────────────────────────────────┐
        │    API REST - ASP.NET Core 8.0 @ :5268           │
        │  ┌──────────────────────────────────────────────┐ │
        │  │ Controllers (C#):                            │ │
        │  │  • IncidenciasController (SQL)               │ │
        │  │  • IncidenciasMongoController (via Logs)     │ │
        │  │  • IncidenciasMongoDirectController (Direct) │ │
        │  │  • EstadisticasController (Reports)          │ │
        │  │  • DebugController (troubleshooting)         │ │
        │  └──────────────────────────────────────────────┘ │
        │  ┌──────────────────────────────────────────────┐ │
        │  │ Services (C#):                               │ │
        │  │  • LogService (CRUD de logs)                 │ │
        │  │  • SyncService (Logs → SQL)                  │ │
        │  │  • MongoToSqlSyncService (Direct → SQL)      │ │
        │  └──────────────────────────────────────────────┘ │
        │  ┌──────────────────────────────────────────────┐ │
        │  │ Data Access (EF Core):                       │ │
        │  │  • AppDbContext                              │ │
        │  │  • DbSet<IncidenciaSql>                      │ │
        │  │  • Migrations                                │ │
        │  └──────────────────────────────────────────────┘ │
        └────────┬──────────────────────┬──────────────────┘
                 │                      │
         ┌───────▼────────┐      ┌──────▼─────────┐
         │   PostgreSQL   │      │   MongoDB      │
         │  Relacional    │      │   Documental  │
         │  @ :5432       │      │   @ :27017    │
         │                │      │                │
         │ Incidencias    │      │ IncidenciaLogs │
         │ (Table)        │      │ (Collection)   │
         │                │      │                │
         │                │      │ IncidenciasDirect
         │                │      │ (Collection)   │
         └────────────────┘      └────────────────┘
```

---

## 2. Flujo de Datos

### 2.1 Crear Incidencia (SQL → MongoDB)

```
┌─────────────────────────────────────────────────────────────┐
│ 1. Usuario llena el formulario en Frontend                  │
│    - Titulo: "Error de login"                              │
│    - Descripcion: "Los usuarios no pueden acceder"         │
│    - Prioridad: "Crítica"                                  │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│ 2. Frontend realiza POST /api/incidencias                  │
│    axios.post(VITE_API_URL + '/incidencias', newInc)      │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│ 3. IncidenciasController.Create()                          │
│    - Convierte JSON a CreateIncidenciaDto                 │
│    - Mapea a IncidenciaSql                                │
│    - Asigna GuidId (Guid.NewGuid())                       │
│    - Asigna FechaCreacion (DateTime.UtcNow)              │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│ 4. Guardar en PostgreSQL                                   │
│    - AppDbContext.Incidencias.Add(incidencia)            │
│    - await _context.SaveChangesAsync()                    │
│    - Retorna 201 Created con el ID autogenerado          │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│ 5. Crear log de auditoría en MongoDB                       │
│    - LogService.CrearLogAsync()                            │
│    - Collection: "IncidenciaLogs"                         │
│    - IncidenciaLog {                                       │
│        Id: new ObjectId(),                                │
│        IncidenciaId: 1,                                   │
│        Acción: "Creación",                                │
│        Usuario: "Sistema",                                │
│        Fecha: DateTime.UtcNow,                            │
│        Datos: { titulo, descripcion, ... }              │
│      }                                                     │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│ 6. Retornar respuesta al Frontend                          │
│    HTTP 201 Created                                        │
│    {                                                       │
│      id: 1,                                               │
│      guidId: "550e8400-...",                             │
│      titulo: "Error de login",                           │
│      ...                                                  │
│    }                                                       │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│ 7. Frontend actualiza estado local                         │
│    - Agrega la incidencia a la lista                      │
│    - Limpia el formulario                                 │
│    - Muestra AlertComponent "Incidencia creada"           │
└─────────────────────────────────────────────────────────────┘
```

---

### 2.2 Sincronizar MongoDB → PostgreSQL

```
┌──────────────────────────────────────────────────────┐
│ 1. Usuario clickea "Sync" en Swagger                 │
│    POST /api/incidencias/sync                       │
└────────────────────┬─────────────────────────────────┘
                     │
┌────────────────────▼─────────────────────────────────┐
│ 2. IncidenciasController.Sync()                     │
│    await _syncService.SincronizarAsync()            │
└────────────────────┬─────────────────────────────────┘
                     │
┌────────────────────▼─────────────────────────────────┐
│ 3. SyncService.SincronizarAsync()                   │
│    - Obtiene todos los logs: await logService.      │
│      ObtenerLogsAsync()                             │
│    - Ordena por fecha: logs.OrderBy(l => l.Fecha)   │
└────────────────────┬─────────────────────────────────┘
                     │
        ┌────────────▼────────────┐
        │ Para cada log:          │
        │                         │
        ├─ Acción: "Creación"    │
        │  → Crear en SQL         │
        │                         │
        ├─ Acción: "Actualización"
        │  → Actualizar en SQL    │
        │    usando GuidId        │
        │    comparar fecha       │
        │                         │
        ├─ Acción: "Eliminación" │
        │  → Eliminar de SQL      │
        │    usando GuidId        │
        │                         │
        └─ Acción: "Sincronización-*"
           → Ignorar (loop infinito)
        
┌────────────────────▼─────────────────────────────────┐
│ 4. Guardar cambios en SQL                           │
│    await _context.SaveChangesAsync()                │
└────────────────────┬─────────────────────────────────┘
                     │
┌────────────────────▼─────────────────────────────────┐
│ 5. Crear log de sincronización                      │
│    - LogService.CrearLogAsync()                     │
│    - Acción: "Sincronización-*"                     │
│    - Evita procesamiento circular                   │
└────────────────────┬─────────────────────────────────┘
                     │
┌────────────────────▼─────────────────────────────────┐
│ 6. Retornar resultado                               │
│    { mensaje: "Sincronización completada" }         │
└──────────────────────────────────────────────────────┘
```

---

### 2.3 Sincronizar MongoDB Directo → PostgreSQL

```
┌─────────────────────────────────────────────────────────┐
│ 1. Usuario clickea "Sync Direct"                        │
│    POST /api/mongo/direct/incidencias/sync             │
└──────────────────┬────────────────────────────────────┘
                   │
┌──────────────────▼────────────────────────────────────┐
│ 2. IncidenciasMongoDirectController.Sync()           │
│    await _mongoToSqlSyncService.SincronizarAsync()   │
└──────────────────┬────────────────────────────────────┘
                   │
┌──────────────────▼────────────────────────────────────┐
│ 3. MongoToSqlSyncService.SincronizarAsync()          │
│    - Obtiene docs de IncidenciasDirect               │
│    - var docsMongoDirectos = await               │
│      collection.Find(new BsonDocument()).       │
│      ToListAsync()                              │
└──────────────────┬────────────────────────────────────┘
                   │
        ┌──────────▼──────────┐
        │ Para cada documento:│
        │                     │
        │ 1. Obtener SQL por  │
        │    GuidId           │
        │                     │
        │ 2. Comparar         │
        │    UltimaActualizacion
        │                     │
        │ 3. Si Mongo > SQL   │
        │    Actualizar SQL   │
        │                     │
        └──────────┬──────────┘
                   │
┌──────────────────▼────────────────────────────────────┐
│ 4. Guardar cambios en SQL                            │
│    await _context.SaveChangesAsync()                 │
└──────────────────┬────────────────────────────────────┘
                   │
┌──────────────────▼────────────────────────────────────┐
│ 5. Retornar resultado                                │
│    { documentosProcesados: 3 }                       │
└─────────────────────────────────────────────────────┘
```

---

## 3. Modelo de Datos

### 3.1 PostgreSQL - Tabla Incidencias

```sql
CREATE TABLE "Incidencias" (
    "Id" SERIAL PRIMARY KEY,
    "GuidId" UUID NOT NULL UNIQUE,
    "Titulo" VARCHAR(255) NOT NULL,
    "Descripcion" TEXT,
    "Estado" VARCHAR(50) NOT NULL DEFAULT 'Abierta',
    "Prioridad" VARCHAR(50) NOT NULL DEFAULT 'Media',
    "FechaCreacion" TIMESTAMP WITH TIME ZONE NOT NULL,
    "UltimaActualizacion" TIMESTAMP WITH TIME ZONE NOT NULL
);

-- Índices para optimización
CREATE INDEX "idx_Incidencias_GuidId" ON "Incidencias"("GuidId");
CREATE INDEX "idx_Incidencias_Estado" ON "Incidencias"("Estado");
CREATE INDEX "idx_Incidencias_Prioridad" ON "Incidencias"("Prioridad");
CREATE INDEX "idx_Incidencias_FechaCreacion" ON "Incidencias"("FechaCreacion");
```

### 3.2 MongoDB - Colección IncidenciaLogs

```javascript
// Estructura de documento
{
  _id: ObjectId("65a7b8c9d0e1f2g3h4i5j6k7"),
  incidenciaId: 1,
  acción: "Creación",
  usuario: "Sistema",
  fecha: ISODate("2025-01-17T08:00:00Z"),
  datos: {
    titulo: "Error de login",
    descripcion: "Los usuarios no pueden acceder",
    estado: "Abierta",
    prioridad: "Crítica",
    fechaCreacion: ISODate("2025-01-17T08:00:00Z"),
    ultimaActualizacion: ISODate("2025-01-17T08:00:00Z")
  }
}
```

### 3.3 MongoDB - Colección IncidenciasDirect

```javascript
// Estructura de documento (copia directa de IncidenciaSql)
{
  _id: ObjectId("65a7b8c9d0e1f2g3h4i5j6k8"),
  guidId: UUID("550e8400-e29b-41d4-a716-446655440000"),
  titulo: "Error de login",
  descripcion: "Los usuarios no pueden acceder",
  estado: "Abierta",
  prioridad: "Crítica",
  fechaCreacion: ISODate("2025-01-17T08:00:00Z"),
  ultimaActualizacion: ISODate("2025-01-17T08:00:00Z")
}
```

---

## 4. Patrones de Diseño

### 4.1 MVC (Model-View-Controller)

```
┌─────────────────────────────────────────────────────────┐
│ MODEL                                                   │
│ ├── IncidenciaSql (EF Core entity)                     │
│ ├── IncidenciaMongo (BSON model)                       │
│ ├── IncidenciaDto (Data transfer object)              │
│ ├── IncidenciaLog (Audit trail)                       │
│ └── IncidenciaData (Snapshot)                         │
│                                                         │
├─────────────────────────────────────────────────────────┤
│ VIEW                                                    │
│ ├── React Components                                   │
│ │   ├── Dashboard.jsx                                 │
│ │   ├── IncidenciasPage                               │
│ │   ├── IncidenciaCard                                │
│ │   ├── IncidenciaForm                                │
│ │   └── FilterBar                                     │
│ └── CSS Styling                                        │
│                                                         │
├─────────────────────────────────────────────────────────┤
│ CONTROLLER                                              │
│ ├── IncidenciasController                             │
│ ├── IncidenciasMongoController                        │
│ ├── IncidenciasMongoDirectController                  │
│ ├── EstadisticasController                            │
│ └── DebugController                                   │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### 4.2 Repository Pattern (via EF Core)

```
API Controller
      │
      ▼
AppDbContext (Repository)
      │
      ├── DbSet<IncidenciaSql>
      └── SaveChangesAsync()
```

### 4.3 Service Layer

```
Controller → Service → Repository → Database
    │          │           │
    │      LogService    EF Core
    │      SyncService   MongoDB.Driver
    └─ MongoToSqlSyncService
```

---

## 5. Stack Tecnológico

### Backend
| Componente | Tecnología | Versión | Propósito |
|-----------|-----------|---------|----------|
| Framework | ASP.NET Core | 8.0 | API REST |
| ORM | Entity Framework Core | 8.0 | Acceso SQL |
| Driver SQL | Npgsql | 8.0 | PostgreSQL |
| Driver NoSQL | MongoDB.Driver | 3.6.0 | MongoDB |
| Documentación | Swashbuckle | 6.6.2 | Swagger UI |
| Testing | xUnit | Futuro | Unit tests |

### Frontend
| Componente | Tecnología | Versión | Propósito |
|-----------|-----------|---------|----------|
| Framework | React | 18+ | UI Components |
| Bundler | Vite | 4+ | Build & Dev |
| HTTP | Axios | - | API Calls |
| Styling | CSS3 | - | Component Styles |
| State | React Hooks | - | State Management |

### Bases de Datos
| DB | Tipo | Propósito | Puerto |
|----|------|----------|--------|
| PostgreSQL | Relacional | CRUD persistente | 5432 |
| MongoDB | Documental | Logs & Auditoría | 27017 |

---

## 6. Flujo de Sincronización Detallado

```
┌─────────────────┐
│  PostgreSQL     │
│  (FUENTE)       │
│                 │
│ Incidencias     │
│ - Id (PK)       │
│ - GuidId (UK)   │
│ - Titulo        │
│ - Estado        │
│ - Prioridad     │
│ - Timestamps    │
└────────┬────────┘
         │
         │ LogService.CrearLogAsync()
         │ (Después de cada operación)
         │
┌────────▼────────────────────────────┐
│ MongoDB - IncidenciaLogs             │
│ (HISTORIAL DE AUDITORÍA)             │
│                                      │
│ {                                    │
│   _id: ObjectId,                    │
│   incidenciaId: 1,                  │
│   acción: "Creación/Actualización", │
│   usuario: "Sistema",               │
│   fecha: ISODate,                   │
│   datos: { snapshot }               │
│ }                                    │
│                                      │
└────────┬────────────────────────────┘
         │
         │ SyncService.SincronizarAsync()
         │ (Manual: POST /api/incidencias/sync)
         │
         │ Lectura: todos los logs
         │ Procesamiento: por orden de fecha
         │ Escritura: cambios aplicados a SQL
         │
┌────────▼───────┐
│ PostgreSQL      │
│ (ACTUALIZADO)   │
└─────────────────┘
```

---

## 7. Manejo de Conflictos

### Escenario: Cambios Concurrentes

```
SQL (13:00)          MongoDB (13:05)
"Abierta"            "En Proceso"
UltimaAct: 13:00     UltimaAct: 13:05

        ↓ Sincronizar (13:10)

Comparar: 13:05 > 13:00 ✓
Aplicar MongoDB a SQL

SQL (13:10)
"En Proceso" ← Ganó MongoDB
UltimaAct: 13:05
```

### Regla de Resolución

```
if (mongo.UltimaActualizacion > sql.UltimaActualizacion) {
    // Aplicar cambios de MongoDB a SQL
    sql.UpdateFrom(mongo);
} else {
    // Mantener cambios de SQL
    // (MongoDB es más antigua)
}
```

---

## 8. Seguridad (Arquitectura)

### Autenticación (Futuro)
```
[Public] → API (No Auth)
[Protegido] → JWT Token → Validation → Controller
```

### Validación
```
Frontend: Validación local (UX)
     ↓
Backend: Validación de entrada (Seguridad)
```

### CORS
```
Frontend (localhost:5173) ←→ Backend (localhost:5268)
Configurado en Program.cs
```

---

## 9. Escalabilidad

### Mejoras Futuras

```
┌─ Caching
│  └─ Redis para datos frecuentes
│
├─ Logging
│  └─ Serilog + ELK Stack
│
├─ Monitoreo
│  └─ Prometheus + Grafana
│
├─ Búsqueda Avanzada
│  └─ Elasticsearch
│
├─ Comunicación Real-time
│  └─ SignalR
│
└─ Escalado Horizontal
   └─ Contenedorización (Docker)
      └─ Orquestación (Kubernetes)
```

---

## 10. Despliegue

### Arquitectura de Producción

```
┌──────────────┐
│   Nginx      │ ← Load Balancer
└──────┬───────┘
       │
   ┌───┴────┬────────┐
   │        │        │
┌──▼──┐  ┌─▼──┐  ┌──▼──┐
│API 1│  │API 2│  │API 3│  ← Instancias de API
└──┬──┘  └─┬──┘  └──┬──┘
   │       │       │
   └───┬───┴───┬───┘
       │       │
┌──────▼──┐ ┌─▼──────┐
│PostgreSQL└─MongoDB  ├─ Shared Databases
└─────────┘ └─────────┘
```

---

## 11. Evolución de la Arquitectura

```
v1.0 (Actual)
├─ Single API Instance
├─ SQL + Logs
├─ Manual Sync
└─ No Cache

v2.0 (Próxima)
├─ Multiple API Instances
├─ Redis Cache
├─ Auto Sync Scheduler
└─ SignalR Notifications

v3.0 (Futura)
├─ Kubernetes
├─ Message Queue
├─ Event Sourcing
└─ CQRS Pattern
```

---

**Última actualización:** Enero 2025  
**Diagrama version:** 1.0
