using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IncidenciasTI.API.Data;
using IncidenciasTI.API.Models;
using Incidencia = IncidenciasTI.API.Models.IncidenciaSql;
using IncidenciasTI.API.DTOs;
using IncidenciasTI.Services;
using IncidenciasTI.Models;
namespace IncidenciasTI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncidenciasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;
        private readonly SyncService _syncService;

        
        public IncidenciasController(AppDbContext context, LogService logService, SyncService syncService)
        {
            _context = context;
            _logService = logService;
            _syncService = syncService;
        }
        // GET: api/incidencias
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                await _logService.CrearLogAsync(new IncidenciaLog
                {
                    Acción = "Consulta masiva",
                    Usuario = "Desconocido",
                    Fecha = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando log en MongoDB: {ex.Message}");
            }
            var incidencias = await _context.Incidencias.ToListAsync();
            var incidenciasDto = incidencias.Select(i => new IncidenciaDto
            {
                Id = i.Id.ToString(),
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

        // GET: api/incidencias/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var incidencia = await _context.Incidencias.FindAsync(id);
            if (incidencia == null)
                return NotFound("Incidencia no encontrada");

            try
            {
                await _logService.CrearLogAsync(new IncidenciaLog
                {
                    IncidenciaId = incidencia.Id,
                    Acción = "Consulta por ID",
                    Usuario = "Desconocido",
                    Fecha = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando log en MongoDB: {ex.Message}");
            }
            var incidenciaDto = new IncidenciaDto
            {
                Id = incidencia.Id.ToString(),
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

        // POST: api/incidencias
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateIncidenciaDto createDto)
        {
            if (createDto == null)
            {
                // Intentamos leer el cuerpo bruto para diagnóstico (útil en dev/testing)
                try
                {
                    Request.EnableBuffering();
                    using var reader = new System.IO.StreamReader(Request.Body, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
                    var rawBody = await reader.ReadToEndAsync();
                    Request.Body.Position = 0;
                    Console.WriteLine($"[DEBUG] POST /api/incidencias - model binding failed. Raw body:\n{rawBody}");
                    return BadRequest(new {
                        error = "Invalid or malformed JSON in request body.",
                        hint = "Ensure Content-Type: application/json and that string values do not contain unescaped raw newlines (\n).",
                        rawBody
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Reading raw request body failed: {ex.Message}");
                    return BadRequest(new { error = "Invalid request body and raw body could not be read." });
                }
            }

            var nuevaIncidencia = new Incidencia
            {
                GuidId = Guid.NewGuid(), // ✅ IMPORTANTE: Generar GUID único para sincronización
                Titulo = createDto.Titulo,
                Descripcion = createDto.Descripcion,
                Prioridad = createDto.Prioridad,
                FechaCreacion = DateTime.UtcNow,
                UltimaActualizacion = DateTime.UtcNow,
                Estado = "Abierta" // Estado por defecto
            };

            await _context.Incidencias.AddAsync(nuevaIncidencia);
            await _context.SaveChangesAsync();
            
            // ✅ AUDITORÍA: Registrar la operación (NO replicar datos completos)
            try
            {
                Console.WriteLine($"[AUDIT] POST SQL: Creación de incidencia ID={nuevaIncidencia.Id}, GUID={nuevaIncidencia.GuidId}");
                await _logService.CrearLogAsync(new IncidenciaLog
                {
                    IncidenciaId = nuevaIncidencia.Id,
                    Acción = "Creación",
                    Usuario = createDto.Usuario ?? "Desconocido",
                    Fecha = DateTime.UtcNow
                    // ⚠️ NOTA: NO guardamos Datos aquí. Solo registramos QUE se creó, no replicamos el contenido.
                    // La sincronización manual (POST /sync) es la que replica datos cuando se necesita.
                });
                Console.WriteLine($"[AUDIT] ✅ Log registrado para auditoría");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AUDIT] ⚠️ Error registrando log en MongoDB: {ex.Message}");
                // No fallar la creación si MongoDB está down - la incidencia ya está en SQL
            }
                        
            var incidenciaDto = new IncidenciaDto
            {
                Id = nuevaIncidencia.Id.ToString(),
                GuidId = nuevaIncidencia.GuidId,
                Titulo = nuevaIncidencia.Titulo,
                Descripcion = nuevaIncidencia.Descripcion,
                Estado = nuevaIncidencia.Estado,
                Prioridad = nuevaIncidencia.Prioridad,
                FechaCreacion = nuevaIncidencia.FechaCreacion,
                UltimaActualizacion = nuevaIncidencia.UltimaActualizacion
            };

            return CreatedAtAction(nameof(GetById), new { id = nuevaIncidencia.Id }, incidenciaDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateIncidenciaDto updateDto)
        {
            var incidencia = await _context.Incidencias.FindAsync(id);
            if (incidencia == null)
                return NotFound("Incidencia no encontrada");

            incidencia.Titulo = updateDto.Titulo;
            incidencia.Descripcion = updateDto.Descripcion;
            incidencia.Prioridad = updateDto.Prioridad;
            incidencia.UltimaActualizacion = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(updateDto.Estado))
                incidencia.Estado = updateDto.Estado; // solo si se envía

            await _context.SaveChangesAsync();

            try
            {
                await _logService.CrearLogAsync(new IncidenciaLog
                {
                    IncidenciaId = incidencia.Id,
                    Acción = "Actualización",
                    Usuario = updateDto.Usuario ?? "Desconocido",
                    Fecha = DateTime.UtcNow,
                    Datos = new IncidenciaData
                    {
                        Titulo = incidencia.Titulo,
                        Descripcion = incidencia.Descripcion,
                        Estado = incidencia.Estado,
                        Prioridad = incidencia.Prioridad,
                        FechaCreacion = incidencia.FechaCreacion,
                        UltimaActualizacion = incidencia.UltimaActualizacion
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando log en MongoDB: {ex.Message}");
            }

            return NoContent();
        }
        // DELETE: api/incidencias/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var incidencia = await _context.Incidencias.FindAsync(id);
            if (incidencia == null)
                return NotFound("Incidencia no encontrada");

            _context.Incidencias.Remove(incidencia);
            await _context.SaveChangesAsync();
            try
            {
                await _logService.CrearLogAsync(new IncidenciaLog
                {
                    IncidenciaId = incidencia.Id,
                    Acción = "Eliminación",
                    Usuario = "Sistema",
                    Fecha = DateTime.UtcNow,
                    Datos = new IncidenciaData
                    {
                        Titulo = incidencia.Titulo,
                        Descripcion = incidencia.Descripcion,
                        Estado = incidencia.Estado,
                        Prioridad = incidencia.Prioridad,
                        FechaCreacion = incidencia.FechaCreacion,
                        UltimaActualizacion = incidencia.UltimaActualizacion
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error guardando log en MongoDB: {ex.Message}");
            }
            return NoContent();
        }

        // POST: api/incidencias/sync
        [HttpPost("sync")]
        public async Task<IActionResult> Sync()
        {
            try
            {
                await _syncService.SincronizarAsync();
                return Ok("Sincronización completada exitosamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error durante la sincronización: {ex.Message}");
            }
        }

        // DEBUG: GET all logs from MongoDB
        [HttpGet("debug/logs")]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var logs = await _logService.ObtenerLogsAsync();
                return Ok(new 
                { 
                    totalLogs = logs.Count,
                    logs = logs.Select(l => new 
                    {
                        id = l.Id,
                        incidenciaId = l.IncidenciaId,
                        acción = l.Acción,
                        usuario = l.Usuario,
                        fecha = l.Fecha,
                        datosNull = l.Datos == null
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener logs: {ex.Message}");
            }
        }
    }
}
