## Lo que debe tener:
SISTEMA DE INTEGRACIÓN Y TRANSFORMACIÓN DE DATOS
POSTGRESQL ↔ MONGODB
CASO DE USO: INCIDENCIAS TI

DESCRIPCIÓN GENERAL
Este proyecto implementa una aplicación distribuida para la gestión de incidencias de Tecnologías de la Información (TI), utilizando una arquitectura híbrida que combina una base de datos relacional (PostgreSQL) y una base de datos documental (MongoDB).

El sistema permite realizar operaciones CRUD en ambos motores, transformar datos entre estructuras relacionales y documentales, y sincronizar la información de manera controlada y verificable.

ARQUITECTURA DEL SISTEMA

Frontend (React)
→ se comunica mediante HTTP (API REST)

Backend (ASP.NET Core Web API)
→ PostgreSQL: almacena los datos estructurados principales
→ MongoDB: almacena datos documentales (historial, logs, transformaciones)

La aplicación demuestra el flujo de datos entre ambos motores de base de datos.

TECNOLOGÍAS UTILIZADAS

Backend

ASP.NET Core Web API

Entity Framework Core

PostgreSQL

MongoDB.Driver

Swagger (OpenAPI)

Frontend

React

Vite

Fetch o Axios

Herramientas

Visual Studio Code

pgAdmin

MongoDB Compass

ESTRUCTURA GENERAL DEL BACKEND

Controllers

Controladores REST (IncidenciasController)

Models

Entidades del dominio (Incidencia, IncidenciaLog)

DTOs

Objetos de transferencia de datos (CreateIncidenciaDto, IncidenciaDto)

Data

AppDbContext (PostgreSQL)

MongoDbService (MongoDB)

Helpers

Utilidades y lógica auxiliar (si aplica)

MODELO DE DATOS

PostgreSQL (modelo relacional)
Tabla: Incidencias
Campos principales:

Id

Titulo

Descripcion

Estado

Prioridad

FechaCreacion

MongoDB (modelo documental)
Colección: IncidenciasLogs
Campos principales:

Id (ObjectId)

IncidenciaId

Accion (CREADA, ACTUALIZADA, ELIMINADA)

Fecha

Datos (estructura JSON flexible)

TRANSFORMACIÓN DE DATOS

Relacional → Documental
Cada operación realizada sobre una incidencia en PostgreSQL genera un documento en MongoDB que registra la acción realizada y los datos involucrados.

Documental → Relacional
La información documental puede utilizarse para análisis histórico, auditoría o reconstrucción del estado (opcional).

Las reglas de transformación están definidas explícitamente en el backend.

SINCRONIZACIÓN

La sincronización es automática y ocurre cuando:

Se crea una incidencia

Se actualiza una incidencia

Se elimina una incidencia

Cada sincronización registra:

Fecha y hora

Tipo de acción

Datos relacionados

Manejo básico de conflictos:

Se considera válida la última operación registrada.

ENDPOINTS PRINCIPALES

GET /api/incidencias
Lista todas las incidencias

GET /api/incidencias/{id}
Obtiene una incidencia específica

POST /api/incidencias
Crea una nueva incidencia

PUT /api/incidencias/{id}
Actualiza una incidencia existente

DELETE /api/incidencias/{id}
Elimina una incidencia

EJECUCIÓN DEL PROYECTO

Backend

Ejecutar: dotnet run

Acceder a Swagger para pruebas de la API

Frontend

Ejecutar: npm install

Ejecutar: npm run dev

EVIDENCIAS DE EJECUCIÓN

Capturas de Swagger mostrando CRUD funcional

Capturas de pgAdmin con registros en PostgreSQL

Capturas de MongoDB Compass con documentos creados

Logs de sincronización entre motores

CRITERIOS DE PROYECTO COMPLETADO

CRUD completo en PostgreSQL

CRUD accesible vía API REST

Uso simultáneo de PostgreSQL y MongoDB

Transformación de datos relacional ↔ documental

Sincronización entre ambos motores

Aplicación funcional con caso de uso claro

Evidencias visuales de ejecución

CONCLUSIONES

El proyecto demuestra el uso práctico de arquitecturas híbridas modernas, integrando bases de datos relacionales y documentales para resolver un problema realista de gestión de incidencias TI.