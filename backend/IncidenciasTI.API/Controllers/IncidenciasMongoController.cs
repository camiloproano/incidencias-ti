using Microsoft.AspNetCore.Mvc;
using IncidenciasTI.API.Models;
using Incidencia = IncidenciasTI.API.Models.IncidenciaMongo;
using IncidenciasTI.API.DTOs;
using IncidenciasTI.Services;
using IncidenciasTI.Models;
using MongoDB.Driver;

namespace IncidenciasTI.API.Controllers
{
    [ApiController]
    [Route("api/mongo/incidencias")]
    public class IncidenciasMongoController : ControllerBase
    {
        private readonly IMongoCollection<IncidenciaLog> _logs;
        private readonly LogService _logService;
        private readonly SyncService _syncService;

        public IncidenciasMongoController(IMongoDatabase database, LogService logService, SyncService syncService)
        {
            _logs = database.GetCollection<IncidenciaLog>("IncidenciaLogs");
            _logService = logService;
            _syncService = syncService;
        }

        // GET: api/mongo/incidencias
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logs = await _logService.ObtenerLogsAsync();
            var incidencias = logs
                .Where(l => l.Datos != null && l.Acción != "Eliminación")
                .GroupBy(l => l.IncidenciaId)
                .Select(g => g.OrderByDescending(l => l.Fecha).First())
                .Select(l => new IncidenciaDto
                {
                    Id = l.IncidenciaId.ToString(),
                    Titulo = l.Datos!.Titulo,
                    Descripcion = l.Datos!.Descripcion,
                    Estado = l.Datos!.Estado,
                    Prioridad = l.Datos!.Prioridad,
                    FechaCreacion = l.Datos!.FechaCreacion,
                    UltimaActualizacion = l.Datos!.UltimaActualizacion
                })
                .ToList();

            return Ok(incidencias);
        }

        // GET: api/mongo/incidencias/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var logs = await _logService.ObtenerLogsAsync();
            var latestLog = logs
                .Where(l => l.IncidenciaId == id && l.Datos != null && l.Acción != "Eliminación")
                .OrderByDescending(l => l.Fecha)
                .FirstOrDefault();

            if (latestLog == null)
                return NotFound("Incidencia no encontrada en MongoDB");

            var incidenciaDto = new IncidenciaDto
            {
                Id = latestLog.IncidenciaId.ToString(),
                Titulo = latestLog.Datos!.Titulo,
                Descripcion = latestLog.Datos!.Descripcion,
                Estado = latestLog.Datos!.Estado,
                Prioridad = latestLog.Datos!.Prioridad,
                FechaCreacion = latestLog.Datos!.FechaCreacion,
                UltimaActualizacion = latestLog.Datos!.UltimaActualizacion
            };

            return Ok(incidenciaDto);
        }

        // POST: api/mongo/incidencias
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateIncidenciaDto createDto)
        {
            int maxId = 0;
            var nuevaIncidencia = new Incidencia
            {
                GuidId = Guid.NewGuid(),
                Titulo = "",
                Descripcion = "",
                Estado = "",
                Prioridad = "",
                FechaCreacion = DateTime.UtcNow,
                UltimaActualizacion = DateTime.UtcNow
            };

            try
            {
                // Generate a new ID (in a real scenario, this might be handled differently)
                maxId = (await _logService.ObtenerLogsAsync())
                    .Select(l => l.IncidenciaId)
                    .DefaultIfEmpty(0)
                    .Max() + 1;

                nuevaIncidencia = new Incidencia
                {
                    GuidId = Guid.NewGuid(),
                    Titulo = createDto.Titulo,
                    Descripcion = createDto.Descripcion,
                    Estado = "Pendiente", // Default state for new incidences
                    Prioridad = createDto.Prioridad,
                    FechaCreacion = DateTime.UtcNow,
                    UltimaActualizacion = DateTime.UtcNow
                };

                Console.WriteLine($"[DEBUG-Mongo] Intentando crear log para incidencia ID={maxId}");
                await _logService.CrearLogAsync(new IncidenciaLog
                {
                    IncidenciaId = maxId,
                    Acción = "Creación",
                    Usuario = createDto.Usuario ?? "Desconocido",
                    Fecha = DateTime.UtcNow,
                    Datos = new IncidenciaData
                    {
                        Titulo = nuevaIncidencia.Titulo,
                        Descripcion = nuevaIncidencia.Descripcion,
                        Estado = nuevaIncidencia.Estado,
                        Prioridad = nuevaIncidencia.Prioridad,
                        FechaCreacion = nuevaIncidencia.FechaCreacion,
                        UltimaActualizacion = nuevaIncidencia.UltimaActualizacion
                    }
                });
                Console.WriteLine($"[DEBUG-Mongo] Log creado exitosamente para incidencia ID={maxId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR-Mongo] Error en Create: {ex.Message}");
                Console.WriteLine($"[ERROR-Mongo] StackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error: {ex.Message}");
            }

            var incidenciaDto = new IncidenciaDto
            {
                Id = maxId.ToString(),
                Titulo = nuevaIncidencia.Titulo,
                Descripcion = nuevaIncidencia.Descripcion,
                Estado = nuevaIncidencia.Estado,
                Prioridad = nuevaIncidencia.Prioridad,
                FechaCreacion = nuevaIncidencia.FechaCreacion,
                UltimaActualizacion = nuevaIncidencia.UltimaActualizacion
            };

            return CreatedAtAction(nameof(GetById), new { id = nuevaIncidencia.Id }, incidenciaDto);
        }

        // PUT: api/mongo/incidencias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateIncidenciaDto updateDto)
        {
            try
            {
                var logs = await _logService.ObtenerLogsAsync();
                var latestLog = logs
                    .Where(l => l.IncidenciaId == id && l.Datos != null && l.Acción != "Eliminación")
                    .OrderByDescending(l => l.Fecha)
                    .FirstOrDefault();

                if (latestLog == null)
                    return NotFound("Incidencia no encontrada en MongoDB");

                var updatedData = new IncidenciaData
                {
                    Titulo = updateDto.Titulo ?? latestLog.Datos!.Titulo,
                    Descripcion = updateDto.Descripcion ?? latestLog.Datos!.Descripcion,
                    Estado = updateDto.Estado ?? latestLog.Datos!.Estado,
                    Prioridad = updateDto.Prioridad ?? latestLog.Datos!.Prioridad,
                    FechaCreacion = latestLog.Datos!.FechaCreacion,
                    UltimaActualizacion = DateTime.UtcNow
                };

                Console.WriteLine($"[DEBUG-Mongo] Intentando crear log de actualización para incidencia ID={id}");
                await _logService.CrearLogAsync(new IncidenciaLog
                {
                    IncidenciaId = id,
                    Acción = "Actualización",
                    Usuario = updateDto.Usuario ?? "Desconocido",
                    Fecha = DateTime.UtcNow,
                    Datos = updatedData
                });
                Console.WriteLine($"[DEBUG-Mongo] Log de actualización creado exitosamente para incidencia ID={id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR-Mongo] Error en Update: {ex.Message}");
                Console.WriteLine($"[ERROR-Mongo] StackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error: {ex.Message}");
            }

            return NoContent();
        }

        // DELETE: api/mongo/incidencias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var logs = await _logService.ObtenerLogsAsync();
            var exists = logs.Any(l => l.IncidenciaId == id && l.Acción != "Eliminación");

            if (!exists)
                return NotFound("Incidencia no encontrada en MongoDB");

            await _logService.CrearLogAsync(new IncidenciaLog
            {
                IncidenciaId = id,
                Acción = "Eliminación",
                Usuario = "Sistema",
                Fecha = DateTime.UtcNow,
                Datos = null // No data needed for deletion
            });

            return NoContent();
        }

        // POST: api/mongo/incidencias/sync
        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {
            try
            {
                await _syncService.SincronizarAsync();
                return Ok("Sincronización de logs a SQL completada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error durante la sincronización: {ex.Message}");
            }
        }
    }
}
