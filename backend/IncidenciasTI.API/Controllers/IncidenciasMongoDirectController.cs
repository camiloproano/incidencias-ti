using Microsoft.AspNetCore.Mvc;
using IncidenciasTI.API.Models;
using IncidenciasTI.API.DTOs;
using MongoDB.Driver;
using System;
using IncidenciasTI.Services;
using IncidenciasTI.Models;
namespace IncidenciasTI.API.Controllers
{
    [ApiController]
    [Route("api/mongo/direct/incidencias")]
    public class IncidenciasMongoDirectController : ControllerBase
    {
        private readonly IMongoCollection<IncidenciaMongo> _incidencias;
        private readonly MongoToSqlSyncService _mongoToSqlSyncService;

        public IncidenciasMongoDirectController(IMongoDatabase database, MongoToSqlSyncService mongoToSqlSyncService)
        {
            _incidencias = database.GetCollection<IncidenciaMongo>("IncidenciasDirect");
            _mongoToSqlSyncService = mongoToSqlSyncService;
        }

        // GET: api/mongo/direct/incidencias
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var incidencias = await _incidencias.Find(i => true).ToListAsync();
            var incidenciasDto = incidencias.Select(i => new IncidenciaDto
            {
                Id = i.Id,
                GuidId = i.GuidId,
                Titulo = i.Titulo,
                Descripcion = i.Descripcion,
                Estado = i.Estado,
                Prioridad = i.Prioridad,
                FechaCreacion = i.FechaCreacion,
                UltimaActualizacion = i.UltimaActualizacion
            }).ToList();

            return Ok(incidenciasDto);
        }

        // GET: api/mongo/direct/incidencias/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var incidencia = await _incidencias.Find(i => i.GuidId == id).FirstOrDefaultAsync();
            if (incidencia == null)
                return NotFound("Incidencia no encontrada en MongoDB");

            var incidenciaDto = new IncidenciaDto
            {
                Id = incidencia.Id,
                GuidId = incidencia.GuidId,
                Titulo = incidencia.Titulo,
                Descripcion = incidencia.Descripcion,
                Estado = incidencia.Estado,
                Prioridad = incidencia.Prioridad,
                FechaCreacion = incidencia.FechaCreacion,
                UltimaActualizacion = incidencia.UltimaActualizacion
            };

            return Ok(incidenciaDto);
        }

        // POST: api/mongo/direct/incidencias
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateIncidenciaDto createDto)
        {
            try
            {
                // Validación explícita
                if (createDto == null)
                {
                    Console.WriteLine("[ERROR-POST] createDto es NULL");
                    return BadRequest(new { error = "El cuerpo de la solicitud es requerido" });
                }

                if (string.IsNullOrWhiteSpace(createDto.Titulo))
                    return BadRequest(new { error = "Titulo es obligatorio" });

                if (string.IsNullOrWhiteSpace(createDto.Descripcion))
                    return BadRequest(new { error = "Descripcion es obligatoria" });

                Console.WriteLine($"[MONGO-POST] Creando incidencia: {createDto.Titulo}");

                var nuevaIncidencia = new IncidenciaMongo
                {
                    GuidId = Guid.NewGuid(),
                    Titulo = createDto.Titulo.Trim(),
                    Descripcion = createDto.Descripcion.Trim(),
                    Estado = "Abierta",
                    Prioridad = string.IsNullOrWhiteSpace(createDto.Prioridad) ? "Media" : createDto.Prioridad,
                    FechaCreacion = DateTime.UtcNow,
                    UltimaActualizacion = DateTime.UtcNow
                };

                await _incidencias.InsertOneAsync(nuevaIncidencia);
                Console.WriteLine($"[MONGO-POST] ✅ Incidencia guardada en IncidenciasDirect con GuidId: {nuevaIncidencia.GuidId}");

                // ✅ AUDITORÍA: Registrar la operación en IncidenciaLogs
                try
                {
                    await _incidencias.Database.GetCollection<IncidenciaLog>("IncidenciaLogs").InsertOneAsync(new IncidenciaLog
                    {
                        Acción = "Creación",
                        Usuario = createDto.Usuario ?? "Desconocido",
                        Fecha = DateTime.UtcNow
                        // ⚠️ NOTA: Solo auditamos la operación, sin duplicar datos
                    });
                    Console.WriteLine($"[AUDIT] ✅ Operación registrada en IncidenciaLogs");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AUDIT] ⚠️ Error registrando auditoría: {ex.Message}");
                }

                var incidenciaDto = new IncidenciaDto
                {
                    Id = nuevaIncidencia.Id,
                    Titulo = nuevaIncidencia.Titulo,
                    Descripcion = nuevaIncidencia.Descripcion,
                    Estado = nuevaIncidencia.Estado,
                    Prioridad = nuevaIncidencia.Prioridad,
                    FechaCreacion = nuevaIncidencia.FechaCreacion,
                    UltimaActualizacion = nuevaIncidencia.UltimaActualizacion
                };

                return CreatedAtAction(nameof(GetById), new { id = nuevaIncidencia.GuidId }, incidenciaDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR-POST] Excepción: {ex.Message}");
                return StatusCode(500, new { error = "Error al crear incidencia", detalles = ex.Message });
            }
        }

        // PUT: api/mongo/direct/incidencias/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIncidenciaDto updateDto)
        {
            var incidencia = await _incidencias.Find(i => i.GuidId == id).FirstOrDefaultAsync();
            if (incidencia == null)
                return NotFound("Incidencia no encontrada en MongoDB");

            // Build update definition dynamically based on provided fields
            var update = Builders<IncidenciaMongo>.Update.Set(i => i.UltimaActualizacion, DateTime.UtcNow);

            if (!string.IsNullOrWhiteSpace(updateDto.Titulo))
                update = update.Set(i => i.Titulo, updateDto.Titulo);

            if (!string.IsNullOrWhiteSpace(updateDto.Descripcion))
                update = update.Set(i => i.Descripcion, updateDto.Descripcion);

            if (!string.IsNullOrWhiteSpace(updateDto.Prioridad))
                update = update.Set(i => i.Prioridad, updateDto.Prioridad);

            if (!string.IsNullOrEmpty(updateDto.Estado))
                update = update.Set(i => i.Estado, updateDto.Estado);

            await _incidencias.UpdateOneAsync(i => i.GuidId == id, update);

            return NoContent();
        }

        // DELETE: api/mongo/direct/incidencias/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _incidencias.DeleteOneAsync(i => i.GuidId == id);
            if (result.DeletedCount == 0)
                return NotFound("Incidencia no encontrada en MongoDB");

            return NoContent();
        }

        // POST: api/mongo/direct/incidencias/sync
        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {
            try
            {
                await _mongoToSqlSyncService.SincronizarAsync();
                return Ok("Sincronización de MongoDB directo a SQL completada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error durante la sincronización: {ex.Message}");
            }
        }
    }
}
