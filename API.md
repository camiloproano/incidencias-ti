# 📚 Documentación API REST - IncidenciasTI

## Información General

**Base URL:** `http://localhost:5268/api`  
**Formato de Respuesta:** JSON  
**Autenticación:** No requerida (versión de demostración)

---

## Tabla de Contenidos

1. [Incidencias (SQL)](#incidencias-sql)
2. [Incidencias (MongoDB via Logs)](#incidencias-mongodb-via-logs)
3. [Incidencias (MongoDB Directo)](#incidencias-mongodb-directo)
4. [Estadísticas](#estadísticas)
5. [Sincronización](#sincronización)
6. [Códigos de Error](#códigos-de-error)
7. [Ejemplos cURL](#ejemplos-curl)

---

## Incidencias (SQL)

### Obtener todas las Incidencias

```http
GET /api/incidencias
```

**Parámetros Query:**
- `pageSize` (opcional): Tamaño de página. Defecto: 10
- `pageNumber` (opcional): Número de página. Defecto: 1

**Respuesta Exitosa (200 OK):**
```json
[
  {
    "id": 1,
    "guidId": "550e8400-e29b-41d4-a716-446655440000",
    "titulo": "Error de login",
    "descripcion": "Los usuarios no pueden acceder al sistema",
    "estado": "Abierta",
    "prioridad": "Crítica",
    "fechaCreacion": "2025-01-15T10:30:00Z",
    "ultimaActualizacion": "2025-01-16T14:20:00Z"
  },
  {
    "id": 2,
    "guidId": "550e8400-e29b-41d4-a716-446655440001",
    "titulo": "Rendimiento lento",
    "descripcion": "Las consultas tardan más de 30 segundos",
    "estado": "En Proceso",
    "prioridad": "Alta",
    "fechaCreacion": "2025-01-14T09:15:00Z",
    "ultimaActualizacion": "2025-01-16T11:45:00Z"
  }
]
```

---

### Obtener Incidencia por ID

```http
GET /api/incidencias/{id}
```

**Parámetros:**
- `id` (requerido, path): ID de la incidencia. Tipo: entero.

**Ejemplo:**
```http
GET /api/incidencias/1
```

**Respuesta Exitosa (200 OK):**
```json
{
  "id": 1,
  "guidId": "550e8400-e29b-41d4-a716-446655440000",
  "titulo": "Error de login",
  "descripcion": "Los usuarios no pueden acceder al sistema",
  "estado": "Abierta",
  "prioridad": "Crítica",
  "fechaCreacion": "2025-01-15T10:30:00Z",
  "ultimaActualizacion": "2025-01-16T14:20:00Z"
}
```

**Respuesta Error (404 Not Found):**
```json
{
  "error": "Incidencia con ID 999 no encontrada"
}
```

---

### Crear Nueva Incidencia

```http
POST /api/incidencias
Content-Type: application/json
```

**Body (requerido):**
```json
{
  "titulo": "Servidor caído",
  "descripcion": "El servidor web no responde",
  "estado": "Abierta",
  "prioridad": "Crítica"
}
```

**Valores Válidos:**
- `estado`: "Abierta", "En Proceso", "Cerrada"
- `prioridad`: "Crítica", "Alta", "Media", "Baja"

**Respuesta Exitosa (201 Created):**
```json
{
  "id": 10,
  "guidId": "550e8400-e29b-41d4-a716-446655440010",
  "titulo": "Servidor caído",
  "descripcion": "El servidor web no responde",
  "estado": "Abierta",
  "prioridad": "Crítica",
  "fechaCreacion": "2025-01-17T08:00:00Z",
  "ultimaActualizacion": "2025-01-17T08:00:00Z"
}
```

**Respuesta Error (400 Bad Request):**
```json
{
  "error": "El campo 'titulo' es requerido"
}
```

---

### Actualizar Incidencia

```http
PUT /api/incidencias/{id}
Content-Type: application/json
```

**Parámetros:**
- `id` (requerido, path): ID de la incidencia a actualizar

**Body (requerido):**
```json
{
  "titulo": "Servidor caído - URGENTE",
  "descripcion": "El servidor web principal no responde desde hace 30 minutos",
  "estado": "En Proceso",
  "prioridad": "Crítica"
}
```

**Respuesta Exitosa (200 OK):**
```json
{
  "id": 1,
  "guidId": "550e8400-e29b-41d4-a716-446655440000",
  "titulo": "Servidor caído - URGENTE",
  "descripcion": "El servidor web principal no responde desde hace 30 minutos",
  "estado": "En Proceso",
  "prioridad": "Crítica",
  "fechaCreacion": "2025-01-15T10:30:00Z",
  "ultimaActualizacion": "2025-01-17T08:15:00Z"
}
```

---

### Eliminar Incidencia

```http
DELETE /api/incidencias/{id}
```

**Parámetros:**
- `id` (requerido, path): ID de la incidencia a eliminar

**Ejemplo:**
```http
DELETE /api/incidencias/5
```

**Respuesta Exitosa (204 No Content):**
```
(sin body)
```

**Respuesta Error (404 Not Found):**
```json
{
  "error": "Incidencia con ID 999 no encontrada"
}
```

---

## Incidencias (MongoDB via Logs)

### Obtener todas vía Logs

```http
GET /api/mongo/incidencias
```

Obtiene todas las incidencias basándose en el historial de logs en MongoDB.

**Respuesta Exitosa (200 OK):**
```json
[
  {
    "id": 1,
    "titulo": "Error de permisos",
    "descripcion": "El usuario no tiene permisos de administrador",
    "estado": "Abierta",
    "prioridad": "Media"
  }
]
```

---

### Crear Incidencia (con Log)

```http
POST /api/mongo/incidencias
Content-Type: application/json
```

**Body:**
```json
{
  "titulo": "Base de datos lenta",
  "descripcion": "Las consultas SQL tardan más de lo normal",
  "prioridad": "Alta"
}
```

**Nota:** Automáticamente crea un log de auditoría en MongoDB.

**Respuesta Exitosa (201 Created):**
```json
{
  "id": 15,
  "titulo": "Base de datos lenta",
  "descripcion": "Las consultas SQL tardan más de lo normal",
  "estado": "Abierta",
  "prioridad": "Alta"
}
```

---

### Actualizar Incidencia (con Log)

```http
PUT /api/mongo/incidencias/{id}
Content-Type: application/json
```

**Body:**
```json
{
  "titulo": "Base de datos lenta - INVESTIGANDO",
  "estado": "En Proceso"
}
```

**Respuesta Exitosa (200 OK):**
```json
{
  "id": 15,
  "titulo": "Base de datos lenta - INVESTIGANDO",
  "descripcion": "Las consultas SQL tardan más de lo normal",
  "estado": "En Proceso",
  "prioridad": "Alta"
}
```

---

### Eliminar Incidencia (con Log)

```http
DELETE /api/mongo/incidencias/{id}
```

**Respuesta Exitosa (204 No Content):**
```
(sin body)
```

---

## Incidencias (MongoDB Directo)

### Obtener todas (Colección Directa)

```http
GET /api/mongo/direct/incidencias
```

Obtiene directamente de la colección IncidenciasDirect en MongoDB.

---

### Crear (Directo en MongoDB)

```http
POST /api/mongo/direct/incidencias
Content-Type: application/json
```

**Body:**
```json
{
  "titulo": "Problema de red",
  "descripcion": "Pérdida de conectividad",
  "prioridad": "Alta"
}
```

---

### Actualizar (Directo en MongoDB)

```http
PUT /api/mongo/direct/incidencias/{id}
Content-Type: application/json
```

---

### Eliminar (Directo en MongoDB)

```http
DELETE /api/mongo/direct/incidencias/{id}
```

---

## Estadísticas

### Resumen General

```http
GET /api/estadisticas/resumen
```

Obtiene un resumen completo del estado del sistema de incidencias.

**Respuesta Exitosa (200 OK):**
```json
{
  "totalIncidencias": 45,
  "porEstado": {
    "abiertas": 12,
    "enProceso": 8,
    "cerradas": 25
  },
  "porPrioridad": {
    "critica": 3,
    "alta": 8,
    "media": 18,
    "baja": 16
  },
  "tasaResolucion": 55.56,
  "incidenciasCriticas": 1,
  "tiempoPromedio": 4.5
}
```

**Campos:**
- `totalIncidencias`: Número total de incidencias
- `porEstado`: Desglose por estado actual
- `porPrioridad`: Desglose por nivel de prioridad
- `tasaResolucion`: Porcentaje de incidencias cerradas
- `incidenciasCriticas`: Críticas sin resolver
- `tiempoPromedio`: Horas promedio para resolver

---

### Incidencias Críticas

```http
GET /api/estadisticas/criticas
```

**Respuesta Exitosa (200 OK):**
```json
{
  "totalCriticas": 2,
  "incidencias": [
    {
      "id": 1,
      "titulo": "Error de login",
      "descripcion": "Los usuarios no pueden acceder",
      "estado": "Abierta",
      "horasTranscurridas": 6.5
    },
    {
      "id": 5,
      "titulo": "Servidor caído",
      "descripcion": "No responde a peticiones",
      "estado": "En Proceso",
      "horasTranscurridas": 2.25
    }
  ]
}
```

---

### Actividades Recientes

```http
GET /api/estadisticas/recientes?cantidad=5
```

**Parámetros Query:**
- `cantidad` (opcional): Número de registros a retornar. Defecto: 5

**Respuesta Exitosa (200 OK):**
```json
[
  {
    "id": 10,
    "titulo": "Nuevo sistema",
    "estado": "Abierta",
    "prioridad": "Baja",
    "fechaCreacion": "2025-01-17T08:00:00Z",
    "ultimaActualizacion": "2025-01-17T08:00:00Z"
  },
  {
    "id": 9,
    "titulo": "Update de software",
    "estado": "En Proceso",
    "prioridad": "Media",
    "fechaCreacion": "2025-01-16T15:30:00Z",
    "ultimaActualizacion": "2025-01-16T16:45:00Z"
  }
]
```

---

### Distribución Temporal

```http
GET /api/estadisticas/distribucion-temporal?dias=30
```

**Parámetros Query:**
- `dias` (opcional): Número de días a analizar. Defecto: 30

**Respuesta Exitosa (200 OK):**
```json
{
  "periodo": "Últimos 30 días",
  "datos": [
    {
      "fecha": "2025-01-17",
      "cantidad": 3,
      "criticas": 0,
      "resueltas": 1
    },
    {
      "fecha": "2025-01-16",
      "cantidad": 5,
      "criticas": 1,
      "resueltas": 2
    }
  ]
}
```

---

### Ranking de Estados

```http
GET /api/estadisticas/ranking-estados
```

**Respuesta Exitosa (200 OK):**
```json
[
  {
    "estado": "Cerrada",
    "cantidad": 25,
    "porcentaje": 55.56,
    "promedioPrioridad": 2.1
  },
  {
    "estado": "Abierta",
    "cantidad": 12,
    "porcentaje": 26.67,
    "promedioPrioridad": 3.2
  },
  {
    "estado": "En Proceso",
    "cantidad": 8,
    "porcentaje": 17.78,
    "promedioPrioridad": 2.9
  }
]
```

---

### Health Check

```http
GET /api/estadisticas/health
```

**Respuesta Exitosa (200 OK):**
```json
{
  "estado": "Excelente",
  "totalIncidencias": 45,
  "incidenciasAbiertas": 12,
  "incidenciasCerradas": 33,
  "timestamp": "2025-01-17T12:00:00Z"
}
```

**Estados Posibles:**
- `Excelente`: 0 incidencias abiertas
- `Buena`: 1-3 incidencias abiertas
- `Regular`: 4-10 incidencias abiertas
- `Crítica`: Más de 10 incidencias abiertas

---

## Sincronización

### Sincronizar MongoDB a PostgreSQL (via Logs)

```http
POST /api/incidencias/sync
```

Aplica todos los cambios registrados en MongoDB logs a la base de datos PostgreSQL.

**Respuesta Exitosa (200 OK):**
```json
{
  "mensaje": "Sincronización completada",
  "operacionesRealizadas": 5
}
```

---

### Sincronizar MongoDB Directo a PostgreSQL

```http
POST /api/mongo/direct/incidencias/sync
```

Sincroniza la colección IncidenciasDirect con PostgreSQL.

**Respuesta Exitosa (200 OK):**
```json
{
  "mensaje": "Sincronización directa completada",
  "documentosProcesados": 3
}
```

---

## Códigos de Error

| Código | Descripción | Causa |
|--------|-------------|-------|
| `200 OK` | Solicitud exitosa | La operación se completó correctamente |
| `201 Created` | Recurso creado | La incidencia fue creada exitosamente |
| `204 No Content` | Operación completada | Eliminación exitosa (sin contenido de respuesta) |
| `400 Bad Request` | Solicitud inválida | Datos incompletos o formato incorrecto |
| `404 Not Found` | Recurso no encontrado | La incidencia no existe |
| `500 Internal Server Error` | Error del servidor | Error inesperado en la API |
| `503 Service Unavailable` | Servicio no disponible | BD no conectada o servicio caído |

---

## Ejemplos cURL

### Obtener todas las incidencias
```bash
curl -X GET "http://localhost:5268/api/incidencias" \
  -H "Content-Type: application/json"
```

### Crear una incidencia
```bash
curl -X POST "http://localhost:5268/api/incidencias" \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Error crítico",
    "descripcion": "La aplicación se congela",
    "estado": "Abierta",
    "prioridad": "Crítica"
  }'
```

### Actualizar una incidencia
```bash
curl -X PUT "http://localhost:5268/api/incidencias/1" \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Error crítico - RESUELTO",
    "estado": "Cerrada"
  }'
```

### Eliminar una incidencia
```bash
curl -X DELETE "http://localhost:5268/api/incidencias/5" \
  -H "Content-Type: application/json"
```

### Obtener resumen de estadísticas
```bash
curl -X GET "http://localhost:5268/api/estadisticas/resumen" \
  -H "Content-Type: application/json"
```

### Sincronizar desde MongoDB
```bash
curl -X POST "http://localhost:5268/api/incidencias/sync" \
  -H "Content-Type: application/json"
```

---

## Notas Importantes

1. **Sincronización**: El sistema mantiene datos sincronizados entre PostgreSQL y MongoDB
2. **UltimaActualizacion**: Se usa para resolver conflictos (el registro más reciente gana)
3. **GuidId**: Cada incidencia tiene un identificador único para sincronización
4. **Logs**: Todos los cambios en MongoDB se registran para auditoría

---

## Swagger UI

Para una documentación interactiva, visita:
```
http://localhost:5268/swagger
```

Desde allí puedes probar todos los endpoints directamente.

---

**Última actualización:** Enero 2025  
**Versión API:** 1.0
