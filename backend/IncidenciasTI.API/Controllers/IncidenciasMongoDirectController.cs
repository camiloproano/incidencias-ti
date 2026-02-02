using Microsoft.AspNetCore.Mvc;
using IncidenciasTI.API.Models;
using IncidenciasTI.API.DTOs;
using MongoDB.Driver;
using System;
using IncidenciasTI.Services;
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
            if (string.IsNullOrWhiteSpace(createDto.Titulo) || string.IsNullOrWhiteSpace(createDto.Descripcion))
                return BadRequest("Titulo y Descripcion son obligatorios.");

            var nuevaIncidencia = new IncidenciaMongo
            {
                GuidId = Guid.NewGuid(), // ID seguro y único
                Titulo = createDto.Titulo,
                Descripcion = createDto.Descripcion,
                Estado = "Abierta", // unificado con PostgreSQL
                Prioridad = createDto.Prioridad,
                FechaCreacion = DateTime.UtcNow,
                UltimaActualizacion = DateTime.UtcNow
            };

            await _incidencias.InsertOneAsync(nuevaIncidencia);

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

        // PUT: api/mongo/direct/incidencias/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIncidenciaDto updateDto)
        {
            var incidencia = await _incidencias.Find(i => i.GuidId == id).FirstOrDefaultAsync();
            if (incidencia == null)
                return NotFound("Incidencia no encontrada en MongoDB");

            if (string.IsNullOrWhiteSpace(updateDto.Titulo) && string.IsNullOrWhiteSpace(updateDto.Descripcion))
                return BadRequest("Debe proporcionar al menos Titulo o Descripcion.");

            var update = Builders<IncidenciaMongo>.Update
                .Set(i => i.Titulo, updateDto.Titulo ?? incidencia.Titulo)
                .Set(i => i.Descripcion, updateDto.Descripcion ?? incidencia.Descripcion)
                .Set(i => i.Prioridad, updateDto.Prioridad ?? incidencia.Prioridad)
                .Set(i => i.UltimaActualizacion, DateTime.UtcNow);

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
