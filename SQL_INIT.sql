-- ============================================
-- Script de Inicialización: PostgreSQL
-- Sistema de Gestión de Incidencias TI
-- ============================================
-- Ejecutar con: psql -U postgres -d incidencias_ti -f SQL_INIT.sql

-- Crear tabla Incidencias
CREATE TABLE IF NOT EXISTS "Incidencias" (
    "Id" SERIAL PRIMARY KEY,
    "GuidId" UUID NOT NULL UNIQUE,
    "Titulo" VARCHAR(200) NOT NULL,
    "Descripcion" TEXT NOT NULL,
    "Estado" VARCHAR(50) NOT NULL,
    "Prioridad" VARCHAR(50) NOT NULL,
    "FechaCreacion" TIMESTAMP NOT NULL,
    "UltimaActualizacion" TIMESTAMP NOT NULL
);

-- Crear índices para optimizar búsquedas
CREATE INDEX IF NOT EXISTS idx_incidencias_guidid ON "Incidencias"("GuidId");
CREATE INDEX IF NOT EXISTS idx_incidencias_estado ON "Incidencias"("Estado");
CREATE INDEX IF NOT EXISTS idx_incidencias_prioridad ON "Incidencias"("Prioridad");
CREATE INDEX IF NOT EXISTS idx_incidencias_fechacreacion ON "Incidencias"("FechaCreacion");

-- Insertar datos de prueba
INSERT INTO "Incidencias" ("GuidId", "Titulo", "Descripcion", "Estado", "Prioridad", "FechaCreacion", "UltimaActualizacion")
VALUES 
    (
        '550e8400-e29b-41d4-a716-446655440001',
        'Servidor SQL caído',
        'El servidor principal de base de datos no responde desde las 10:30. Se debe reiniciar o restaurar desde backup.',
        'Abierta',
        'Crítica',
        '2026-02-01 10:30:00',
        '2026-02-01 10:30:00'
    ),
    (
        '550e8400-e29b-41d4-a716-446655440002',
        'Red corporativa lenta',
        'Se experimenta congestión en la red. Ancho de banda utilizado al 85%. Investigar origen del tráfico anómalo.',
        'En Proceso',
        'Alta',
        '2026-02-01 11:00:00',
        '2026-02-02 08:15:00'
    ),
    (
        '550e8400-e29b-41d4-a716-446655440003',
        'Actualizaciones de seguridad pendientes',
        'Se deben aplicar los parches de seguridad mensuales en todos los servidores Windows.',
        'Abierta',
        'Media',
        '2026-02-02 09:00:00',
        '2026-02-02 09:00:00'
    ),
    (
        '550e8400-e29b-41d4-a716-446655440004',
        'Disco C: lleno en workstation',
        'Usuario reporta que el disco C tiene 98% de utilización. Limpiar archivos temporales.',
        'Cerrada',
        'Baja',
        '2026-02-01 14:30:00',
        '2026-02-01 16:45:00'
    ),
    (
        '550e8400-e29b-41d4-a716-446655440005',
        'Fallo en respaldo diario',
        'El script de backup no completó correctamente. Base de datos de producción sin respaldo desde ayer.',
        'En Proceso',
        'Crítica',
        '2026-02-02 06:00:00',
        '2026-02-02 10:00:00'
    );

-- Mostrar datos insertados
SELECT COUNT(*) as "Total Incidencias" FROM "Incidencias";
SELECT * FROM "Incidencias" ORDER BY "FechaCreacion" DESC;

-- ============================================
-- Notas de Uso:
-- ============================================
-- • Para conectar: psql -U postgres -d incidencias_ti
-- • Para ver tabla: \dt "Incidencias"
-- • Para limpiar: DELETE FROM "Incidencias"; (luego RESET SEQUENCE)
-- • Para resetear secuencia: SELECT setval('"Incidencias_Id_seq"', 1);
-- ============================================
