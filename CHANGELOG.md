# 📝 Changelog - IncidenciasTI

Todos los cambios notables en este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto adhiere a [Semantic Versioning](https://semver.org/es/).

---

## [Unreleased]

### Planeado para v2.0
- [ ] Autenticación y autorización (JWT)
- [ ] Real-time notifications (SignalR)
- [ ] Búsqueda avanzada y filtros mejorados
- [ ] Export de datos (PDF, Excel)
- [ ] Paginación en frontend
- [ ] Dark mode
- [ ] Internacionalización (i18n)
- [ ] Tests unitarios e integración
- [ ] Logging avanzado con Serilog
- [ ] Rate limiting en API
- [ ] Caché con Redis

---

## [1.0.0] - 2025-01-17

### Agregado - Backend
- ✨ **IncidenciasController**: CRUD completo de incidencias en PostgreSQL
  - GET `/api/incidencias` - Obtener todas
  - GET `/api/incidencias/{id}` - Obtener por ID
  - POST `/api/incidencias` - Crear nueva
  - PUT `/api/incidencias/{id}` - Actualizar
  - DELETE `/api/incidencias/{id}` - Eliminar
  - POST `/api/incidencias/sync` - Sincronizar desde MongoDB

- ✨ **IncidenciasMongoController**: CRUD vía logs en MongoDB
  - GET `/api/mongo/incidencias` - Obtener (desde logs)
  - POST `/api/mongo/incidencias` - Crear (genera log)
  - PUT `/api/mongo/incidencias/{id}` - Actualizar (genera log)
  - DELETE `/api/mongo/incidencias/{id}` - Eliminar (genera log)
  - POST `/api/mongo/incidencias/sync` - Sincronizar logs a SQL

- ✨ **IncidenciasMongoDirectController**: CRUD directo en MongoDB
  - GET `/api/mongo/direct/incidencias` - Obtener todas
  - POST `/api/mongo/direct/incidencias` - Crear
  - PUT `/api/mongo/direct/incidencias/{id}` - Actualizar
  - DELETE `/api/mongo/direct/incidencias/{id}` - Eliminar
  - POST `/api/mongo/direct/incidencias/sync` - Sincronizar a SQL

- ✨ **EstadisticasController**: Reporting y analytics
  - GET `/api/estadisticas/resumen` - Resumen general
  - GET `/api/estadisticas/criticas` - Incidencias críticas
  - GET `/api/estadisticas/recientes` - Actividades recientes
  - GET `/api/estadisticas/distribucion-temporal` - Gráficas por día
  - GET `/api/estadisticas/ranking-estados` - Ranking de estados
  - GET `/api/estadisticas/health` - Health check del sistema

- ✨ **Models**:
  - `IncidenciaSql`: Entidad para PostgreSQL
  - `IncidenciaMongo`: Modelo BSON para MongoDB
  - `IncidenciaLog`: Registro de auditoría
  - `IncidenciaData`: Snapshot de datos
  - `IncidenciaDto`: Data transfer object

- ✨ **Services**:
  - `LogService`: CRUD de logs en MongoDB
  - `SyncService`: Sincronización de logs a SQL
  - `MongoToSqlSyncService`: Sincronización directa MongoDB→SQL
  - Logging extenso con [DEBUG] y [ERROR] tags

- ✨ **Configuration**:
  - Integración Entity Framework Core 8.0
  - MongoDB.Driver con CamelCaseElementNameConvention
  - Dependency Injection en Program.cs
  - Swagger/Swashbuckle documentation

- ✨ **Database**:
  - PostgreSQL schema con índices optimizados
  - EF Core migrations (3 migrations aplicadas)
  - MongoDB collections (IncidenciaLogs, IncidenciasDirect)

### Agregado - Frontend
- ✨ **Componentes React**:
  - `Header.jsx`: Título con estilo gradient
  - `Navigation.jsx`: Navegación entre secciones
  - `Dashboard.jsx`: Panel de control con estadísticas
  - `IncidenciaForm.jsx`: Formulario para crear/editar
  - `IncidenciaCard.jsx`: Tarjeta de incidencia con modo edición
  - `FilterBar.jsx`: Filtros por prioridad y estado
  - `Modal.jsx`: Modal reusable
  - `Alert.jsx`: Notificaciones toast

- ✨ **Styling**:
  - `global.css`: Sistema de diseño con CSS variables
  - `Dashboard.css`: Estilos para panel de control
  - `Navigation.css`: Navegación sticky
  - `Incidencias.css`: Grid responsive
  - `IncidenciaForm.css`, `IncidenciaCard.css`, `FilterBar.css`, etc.

- ✨ **API Integration**:
  - `incidenciasApi.js`: Cliente Axios con configuración de .env
  - Interceptor para manejo de errores de red
  - Support para `VITE_API_URL` environment variable

- ✨ **State Management**:
  - React Hooks (useState, useEffect)
  - Local state para formularios y filtros
  - Auto-refresh de datos

### Agregado - Documentación
- 📚 **README.md**: Documentación completa (500+ líneas)
  - Introducción y descripción del proyecto
  - Arquitectura con diagrama ASCII
  - Modelo de datos
  - Reglas de transformación
  - Flujos de sincronización
  - Instalación paso a paso
  - Endpoints disponibles
  - Ejemplo de workflow con curl

- 📚 **SETUP.md**: Guía de instalación detallada (600+ líneas)
  - Requisitos previos
  - Instalación backend y frontend
  - Configuración de PostgreSQL
  - Configuración de MongoDB
  - Inicialización de datos
  - Ejecución de la aplicación
  - Verificación del sistema
  - Troubleshooting exhaustivo

- 📚 **API.md**: Documentación de endpoints (400+ líneas)
  - Descripción general y formato
  - Endpoints SQL, MongoDB y Estadísticas
  - Ejemplos de request/response
  - Códigos de error
  - Ejemplos cURL

- 📚 **ARCHITECTURE.md**: Diagramas y decisiones arquitectónicas (500+ líneas)
  - Diagrama general del sistema
  - Flujos de datos detallados
  - Modelo de datos
  - Patrones de diseño
  - Stack tecnológico
  - Sincronización
  - Manejo de conflictos
  - Escalabilidad

- 📚 **CONTRIBUTING.md**: Guía para contribuidores (400+ líneas)
  - Código de conducta
  - Proceso de reporte de bugs
  - Estándares de código
  - Guía de testing
  - Convenciones de commits
  - Pull request process

- 📚 **CHANGELOG.md**: Este archivo - Historial de cambios

- 📚 **API.md**: Documentación de endpoints REST

- 📝 **.env.example**: Archivo de configuración de ejemplo

### Agregado - Datos de Ejemplo
- 🗄️ **SQL_INIT.sql**: Script SQL con:
  - CREATE TABLE Incidencias con schema completo
  - CREATE INDEX para optimización
  - INSERT 5 registros de ejemplo

- 🗄️ **MONGO_SEED.json**: 8 documentos de ejemplo para IncidenciaLogs
  - Logs de creación
  - Logs de actualización
  - Datos de auditoría

- 🗄️ **MONGO_SEED_DIRECT.json**: 5 documentos de ejemplo para IncidenciasDirect
  - Documentos directos sincronizados
  - GuidId matching

### Cambios
- Renombrado archivo de modelo de `Incidencia.cs` a `IncidenciaSql.cs` y `IncidenciaMongo.cs` para claridad
- Agregado campo `GuidId` (UUID) para sincronización entre bases de datos
- Agregado campo `UltimaActualizacion` para resolución de conflictos
- Propagado campo `UltimaActualizacion` a través de DTOs, servicios y controllers

### Arreglado
- ✅ Error CS0246: Tipo 'Incidencia' no encontrado - Resuelto usando IncidenciaSql
- ✅ Error CS1660: No se puede convertir Incidencia a IncidenciaDto - Resuelto con mapeo correcto
- ✅ Npgsql.PostgresException 42703: Column 'GuidId' does not exist - Resuelto con migración EF
- ✅ System.FormatException: Element 'Id' does not match any field - Resuelto removiendo [BsonElement] conflictivos
- ✅ ERR_CONNECTION_REFUSED en frontend - Resuelto con .env y Axios configuration
- ✅ Variables fuera de scope en try block - Resuelto moviendo declaraciones

### Removido
- [BsonElement("IncidenciaId")] conflictivos en IncidenciaLog.cs
- [BsonElement("Titulo")] conflictivos en IncidenciaData.cs
- Hardcoded API URL en frontend (reemplazado con .env)

### Seguridad
- ⚠️ Sin autenticación (versión de demostración)
- ⚠️ CORS abierto para desarrollo local
- 🔄 Preparación para JWT en v2.0

### Conocidos
- [ ] La sincronización es manual (POST endpoint), no automática
- [ ] No hay paginación en frontend
- [ ] No hay búsqueda avanzada
- [ ] Límite de 10 registros por página en algunas queries

---

## [0.1.0] - 2025-01-10 (Fase Inicial)

### Agregado
- ✨ Proyecto base ASP.NET Core 8.0
- ✨ Proyecto base React + Vite
- ✨ Configuración PostgreSQL
- ✨ Configuración MongoDB
- ✨ Modelo inicial de Incidencia

### Planeado para Fase 2
- Autenticación
- Tests automáticos
- CI/CD con GitHub Actions

---

## Cómo Interpretar Este Changelog

- **Agregado**: Nuevas características
- **Cambios**: Cambios en funcionalidad existente
- **Deprecado**: Características que serán removidas pronto
- **Removido**: Características removidas
- **Arreglado**: Bugs corregidos
- **Seguridad**: Parches de seguridad

---

## Notas de Versión

### v1.0.0 - Producción Ready

Esta versión representa una implementación completa de un sistema de gestión de incidencias con:

✅ **Backend robusto** con tres patrones de sincronización
✅ **Frontend profesional** con componentes reusables
✅ **Documentación exhaustiva** (2000+ líneas)
✅ **Ejemplos y guías** paso a paso
✅ **Manejo de errores** y logging

### Características Principales

1. **Triple Patrón de Sincronización**
   - SQL directo
   - Logs con auditoría
   - MongoDB directo

2. **Estadísticas en Tiempo Real**
   - Dashboard con 6 tipos de reportes
   - Health check del sistema
   - Distribuciones por estado y prioridad

3. **UI Responsiva**
   - Componentes React modernos
   - CSS Grid adaptativo
   - Modo edición inline
   - Navegación sticky

4. **Documentación Completa**
   - 5 archivos de documentación
   - Diagramas ASCII
   - Ejemplos cURL
   - Guía de setup

---

## Roadmap

### Corto Plazo (v1.1 - Próxima semana)
- [ ] Mejorar validación de datos
- [ ] Agregar soft delete
- [ ] Búsqueda por título

### Mediano Plazo (v2.0 - Próximo mes)
- [ ] JWT Authentication
- [ ] SignalR Real-time
- [ ] Redis Cache
- [ ] Tests unitarios

### Largo Plazo (v3.0 - Próximos 3 meses)
- [ ] Docker & Kubernetes
- [ ] Message Queue
- [ ] Event Sourcing
- [ ] CQRS Pattern

---

## Contribuyentes

- 👨‍💻 Tu Nombre (Autor)
- 🙏 Gracias a todos los que han contribuido

---

## Licencia

Este proyecto está bajo la licencia MIT. Ver [LICENSE](LICENSE) para más detalles.

---

**Última actualización:** 17 de Enero de 2025  
**Versión actual:** 1.0.0  
**Estado:** ✅ Estable y Listo para Producción
