# 📁 Estructura Completa del Proyecto - IncidenciasTI

```
incidencias-ti/
│
├── 📄 README.md                          # Documentación principal
├── 📄 SETUP.md                          # Guía de instalación (600+ líneas)
├── 📄 API.md                            # Documentación de endpoints
├── 📄 ARCHITECTURE.md                   # Diagramas y decisiones técnicas
├── 📄 CONTRIBUTING.md                   # Guía para contribuidores
├── 📄 CHANGELOG.md                      # Historial de cambios
├── 📄 TODO.md                           # Estado del proyecto (actualizado)
├── 📄 .env.example                      # Configuración de ejemplo
├── 📄 incidencias-ti.sln                # Solución Visual Studio
│
│
├── 📂 backend/
│   ├── 📂 Data/
│   │   ├── AppDbContext.cs              # EF Core DbContext
│   │   ├── SQL_INIT.sql                 # Script SQL con datos
│   │   ├── MONGO_SEED.json              # 8 documentos para IncidenciaLogs
│   │   └── MONGO_SEED_DIRECT.json       # 5 documentos para IncidenciasDirect
│   │
│   ├── 📂 IncidenciasTI.API/
│   │   │
│   │   ├── 📂 Controllers/              # 5 Controllers
│   │   │   ├── IncidenciasController.cs
│   │   │   ├── IncidenciasMongoController.cs
│   │   │   ├── IncidenciasMongoDirectController.cs
│   │   │   ├── EstadisticasController.cs      # ✨ NUEVO
│   │   │   └── DebugController.cs
│   │   │
│   │   ├── 📂 Models/                   # 4 Models
│   │   │   ├── IncidenciaSql.cs         # Entity para SQL
│   │   │   ├── IncidenciaMongo.cs       # BSON Model
│   │   │   ├── IncidenciaLog.cs         # Audit Log
│   │   │   └── IncidenciaData.cs        # Data Snapshot
│   │   │
│   │   ├── 📂 DTOs/                     # 3 Data Transfer Objects
│   │   │   ├── IncidenciaDto.cs
│   │   │   ├── CreateIncidenciaDto.cs
│   │   │   └── UpdateIncidenciaDto.cs
│   │   │
│   │   ├── 📂 Services/                 # 3 Services
│   │   │   ├── LogService.cs            # CRUD de logs
│   │   │   ├── SyncService.cs           # Logs → SQL
│   │   │   └── MongoToSqlSyncService.cs # Direct → SQL
│   │   │
│   │   ├── 📂 Configurations/
│   │   │   └── MongoDBSettings.cs       # MongoDB Config
│   │   │
│   │   ├── 📂 Migrations/               # 3 EF Core Migrations
│   │   │   ├── 20260201015951_InitialCreate.cs
│   │   │   ├── 20260201021104_FixDateTimeUtc.cs
│   │   │   └── 20260201021458_FixDateTimeUtc-2.cs
│   │   │
│   │   ├── 📂 Properties/
│   │   │   └── launchSettings.json
│   │   │
│   │   ├── 📂 bin/Debug/               # Compilado
│   │   │
│   │   ├── 📄 Program.cs               # DI & Configuration
│   │   ├── 📄 appsettings.json         # Config
│   │   ├── 📄 appsettings.Development.json
│   │   ├── 📄 IncidenciasTI.API.csproj # .NET Project
│   │   └── 📄 IncidenciasTI.API.http   # REST Client
│   │
│   └── 📂 Helpers/
│       └── (Helpers si se necesitan)
│
│
├── 📂 frontend/
│   └── 📂 incidencias-ti-ui/
│       │
│       ├── 📄 package.json             # npm dependencies
│       ├── 📄 vite.config.js           # Vite config
│       ├── 📄 eslint.config.js         # Linting
│       ├── 📄 index.html               # HTML principal
│       ├── 📄 .env                     # ✨ NUEVO - VITE_API_URL
│       │
│       ├── 📂 src/
│       │   │
│       │   ├── 📄 main.jsx             # Entry point
│       │   ├── 📄 App.jsx              # Root component (actualizado)
│       │   ├── 📄 index.css            # Global styles
│       │   │
│       │   ├── 📂 components/          # 8 Reusable Components
│       │   │   ├── Header.jsx
│       │   │   ├── Navigation.jsx      # ✨ NUEVO
│       │   │   ├── Dashboard.jsx       # ✨ NUEVO
│       │   │   ├── IncidenciaForm.jsx
│       │   │   ├── IncidenciaCard.jsx
│       │   │   ├── FilterBar.jsx
│       │   │   ├── Modal.jsx
│       │   │   └── Alert.jsx
│       │   │
│       │   ├── 📂 pages/
│       │   │   └── Incidencias.jsx     # Main page
│       │   │
│       │   ├── 📂 api/
│       │   │   └── incidenciasApi.js   # Axios client (actualizado)
│       │   │
│       │   ├── 📂 styles/             # 7 CSS Files
│       │   │   ├── global.css
│       │   │   ├── Dashboard.css       # ✨ NUEVO
│       │   │   ├── Navigation.css      # ✨ NUEVO
│       │   │   ├── Incidencias.css
│       │   │   ├── Header.css
│       │   │   ├── IncidenciaForm.css
│       │   │   ├── IncidenciaCard.css
│       │   │   ├── FilterBar.css
│       │   │   ├── Modal.css
│       │   │   └── Alert.css
│       │   │
│       │   └── 📂 assets/
│       │       └── (Icons, images)
│       │
│       ├── 📂 public/
│       │   └── (Public assets)
│       │
│       └── 📂 node_modules/
│           └── (npm dependencies)
│
└── 📂 .git/
    └── (Git history)
```

---

## 📊 Resumen de Archivos

### 📋 Documentación (7 archivos)
| Archivo | Líneas | Propósito |
|---------|--------|----------|
| README.md | 500+ | Descripción general |
| SETUP.md | 600+ | Guía de instalación |
| API.md | 400+ | Documentación de endpoints |
| ARCHITECTURE.md | 500+ | Diagramas técnicos |
| CONTRIBUTING.md | 400+ | Guía de contribución |
| CHANGELOG.md | 350+ | Historial de cambios |
| TODO.md | 200+ | Estado del proyecto |

**Total documentación: 3000+ líneas**

---

### 🔧 Backend (C#)

#### Controllers (5 archivos)
- **IncidenciasController.cs**: CRUD + Sync (SQL)
- **IncidenciasMongoController.cs**: CRUD via Logs
- **IncidenciasMongoDirectController.cs**: CRUD Directo
- **EstadisticasController.cs**: 6 endpoints de reportes ✨
- **DebugController.cs**: Troubleshooting endpoints

#### Services (3 archivos)
- **LogService.cs**: CRUD en MongoDB logs
- **SyncService.cs**: Sincronización logs → SQL
- **MongoToSqlSyncService.cs**: Sincronización directo → SQL

#### Models (4 archivos)
- **IncidenciaSql.cs**: Entity para EF Core (PostgreSQL)
- **IncidenciaMongo.cs**: BSON Model (MongoDB)
- **IncidenciaLog.cs**: Registro de auditoría
- **IncidenciaData.cs**: Snapshot de datos

#### DTOs (3 archivos)
- **IncidenciaDto.cs**: Para GET requests
- **CreateIncidenciaDto.cs**: Para POST
- **UpdateIncidenciaDto.cs**: Para PUT

#### Configuración
- **Program.cs**: Dependency Injection & Startup
- **AppDbContext.cs**: EF Core DbContext
- **MongoDBSettings.cs**: MongoDB Configuration
- **appsettings.json**: Configuración general
- **appsettings.Development.json**: Dev config

#### Migraciones (EF Core)
- 3 migrations aplicadas a PostgreSQL
- Schema completo con índices

**Total backend C#: ~2000 líneas**

---

### 🎨 Frontend (React + Vite)

#### Componentes (8 archivos)
- **Header.jsx**: Título con gradient
- **Navigation.jsx**: Navegación sticky ✨
- **Dashboard.jsx**: Panel de control ✨
- **IncidenciasPage.jsx**: Página principal
- **IncidenciaForm.jsx**: Formulario CRUD
- **IncidenciaCard.jsx**: Tarjeta con edición
- **FilterBar.jsx**: Filtros interactivos
- **Alert.jsx**: Notificaciones toast
- **Modal.jsx**: Modal reusable

#### Estilos (9 archivos CSS)
- **global.css**: Variables y base styles
- **Dashboard.css**: Estilos del dashboard ✨
- **Navigation.css**: Navegación ✨
- **Incidencias.css**: Grid y layout
- **Header.css**: Estilos del header
- **IncidenciaForm.css**: Formulario
- **IncidenciaCard.css**: Tarjetas
- **FilterBar.css**: Filtros
- **Modal.css** / **Alert.css**: UI elements

#### API Integration
- **incidenciasApi.js**: Cliente Axios
- **.env**: Configuración de variables de entorno

**Total frontend: ~2500 líneas (JS + CSS)**

---

### 💾 Bases de Datos

#### PostgreSQL
- **SQL_INIT.sql**: DDL + 5 registros de ejemplo
- **Tabla**: "Incidencias" (8 columnas)
- **Índices**: 5 índices para optimización
- **Migraciones**: 3 aplicadas con EF Core

#### MongoDB
- **MONGO_SEED.json**: 8 documentos IncidenciaLogs
- **MONGO_SEED_DIRECT.json**: 5 documentos IncidenciasDirect
- **Colecciones**: 2
- **Documentos totales**: 13

---

## 📈 Estadísticas Totales

### Código
```
Backend (C#):     ~2000 líneas
Frontend (JS/CSS): ~2500 líneas
SQL/NoSQL:        ~200 líneas
─────────────────────────────
TOTAL:            ~4700 líneas
```

### Documentación
```
README:           500+ líneas
SETUP:            600+ líneas
API:              400+ líneas
ARCHITECTURE:     500+ líneas
CONTRIBUTING:     400+ líneas
CHANGELOG:        350+ líneas
────────────────────────────
TOTAL:            3000+ líneas
```

### Componentes
```
Controllers:      5
Services:         3
Models:           4
DTOs:             3
React Components: 9
CSS Files:        9
────────────────
TOTAL:            33 archivos principales
```

---

## 🎯 Características Implementadas

### ✅ Core CRUD (27 endpoints)
- SQL CRUD (5 endpoints)
- MongoDB Logs CRUD (5 endpoints)
- MongoDB Direct CRUD (5 endpoints)
- Estadísticas (6 endpoints)
- Sincronización (2 endpoints)
- Debug (4 endpoints)

### ✅ Frontend Features
- Listar incidencias (grid responsive)
- Crear incidencia (formulario)
- Editar incidencia (inline mode)
- Eliminar incidencia (con confirmación)
- Filtrar por prioridad/estado
- Dashboard con 6 tipos de reportes
- Navegación entre secciones
- Alertas y notificaciones

### ✅ Sincronización
- Logs → SQL (manual)
- Directo → SQL (manual)
- Resolución de conflictos
- Auditoría completa

### ✅ Documentación
- README profesional
- Setup paso a paso
- API documentada
- Arquitectura diagramada
- Contributing guidelines
- Changelog detallado

---

## 🚀 Cómo Usar Este Proyecto

### 1️⃣ Clonar/Descargar
```bash
git clone https://github.com/tu-usuario/incidencias-ti.git
cd incidencias-ti
```

### 2️⃣ Leer Documentación
- Empezar con: `README.md` (2 minutos)
- Setup: `SETUP.md` (15 minutos)
- Endpoints: `API.md` (opcional)
- Arquitectura: `ARCHITECTURE.md` (opcional)

### 3️⃣ Instalar y Ejecutar
```bash
# Backend
cd backend/IncidenciasTI.API
dotnet run

# Frontend (en otra terminal)
cd frontend/incidencias-ti-ui
npm install
npm run dev
```

### 4️⃣ Acceder
- Frontend: http://localhost:5173
- Swagger: http://localhost:5268/swagger

---

## 📁 Convenciones de Proyecto

### Carpetas
```
/backend    → Código C# (ASP.NET Core)
/frontend   → Código JavaScript (React)
/docs       → Documentación adicional (si existe)
```

### Archivos
```
*.cs        → C# source files
*.jsx       → React components
*.js        → Utility functions
*.css       → Stylesheets
*.md        → Documentation
*.json      → Configuration & Data
*.sql       → Database scripts
```

### Nomenclatura
```
Controllers: PascalCase (IncidenciasController)
Services:    PascalCase (LogService)
Models:      PascalCase (IncidenciaSql)
Components:  PascalCase (IncidenciaCard)
Functions:   camelCase (getIncidencia)
Variables:   camelCase (incidenciaCount)
```

---

## 🔐 Archivos Importantes

### No Incluir en Git
```
.env                 (Contraseñas/secretos)
node_modules/        (Instalación local)
bin/                 (Compilado)
obj/                 (Build artifacts)
.vs/                 (Visual Studio cache)
```

### Incluir en Git
```
*.cs, *.jsx, *.js   (Source code)
*.md                (Documentation)
.env.example        (Template)
package.json        (Dependencies)
*.csproj            (Project files)
appsettings.json    (Settings)
```

---

## 📞 Puntos de Entrada

### Backend
- **Program.cs**: Configuración y startup
- **Controllers/**: Endpoints de API
- **Services/**: Lógica de negocio
- **Models/**: Entidades de datos

### Frontend
- **main.jsx**: Entry point
- **App.jsx**: Root component
- **components/**: Componentes reusables
- **pages/**: Páginas principales

---

## ✨ Elementos Destacados

### Nuevos en Fase 2
- ✨ EstadisticasController con 6 endpoints
- ✨ Dashboard.jsx con reportes
- ✨ Navigation.jsx para cambiar vistas
- ✨ Dashboard.css con estilos profesionales
- ✨ Navigation.css con sticky navigation
- ✨ .env para configuración de API URL
- ✨ SETUP.md con guía de instalación
- ✨ ARCHITECTURE.md con diagramas
- ✨ CONTRIBUTING.md para colaboradores
- ✨ CHANGELOG.md con historial

---

## 🎓 Cumplimiento de Requisitos

| Requisito | Archivo/Ubicación | Status |
|-----------|------------------|--------|
| Descripción | README.md | ✅ |
| Modelo Datos | ARCHITECTURE.md | ✅ |
| Arquitectura | ARCHITECTURE.md | ✅ |
| Reglas Transformación | README.md, ARCHITECTURE.md | ✅ |
| Implementación | Controllers/, Services/ | ✅ |
| Sincronización | SyncService.cs | ✅ |
| Frontend | frontend/incidencias-ti-ui | ✅ |
| Documentación | *.md files | ✅ |
| Conclusiones | README.md, ARCHITECTURE.md | ✅ |

---

**Última actualización:** 17 de Enero de 2025  
**Versión:** 1.0.0  
**Estado:** ✅ Completado y Listo
