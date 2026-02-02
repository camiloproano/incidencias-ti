using IncidenciasTI.API.Data;
using IncidenciasTI.API.Models;
using IncidenciasTI.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidenciasTI.Services
{
    public class SyncService
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;

        public SyncService(AppDbContext context, LogService logService)
        {
            _context = context;
            _logService = logService;
        }

        public async Task SincronizarAsync()
        {
            var logs = await _logService.ObtenerLogsAsync();

            foreach (var log in logs.OrderBy(l => l.Fecha))
            {
                if (log.Datos == null) continue; // Skip logs without data

                var existingIncidencia = await _context.Incidencias.FindAsync(log.IncidenciaId);

                switch (log.Acción)
                {
                    case "Creación":
                        if (existingIncidencia == null)
                        {
                            var nuevaIncidencia = new IncidenciaSql
                            {
                                Id = log.IncidenciaId,
                                Titulo = log.Datos.Titulo,
                                Descripcion = log.Datos.Descripcion,
                                Estado = log.Datos.Estado,
                                Prioridad = log.Datos.Prioridad,
                                FechaCreacion = log.Datos.FechaCreacion,
                                UltimaActualizacion = log.Datos.UltimaActualizacion
                            };
                            await _context.Incidencias.AddAsync(nuevaIncidencia);
                            // Also create a log entry documenting the sync-created record
                            try
                            {
                                await _logService.CrearLogAsync(new IncidenciaLog
                                {
                                    IncidenciaId = nuevaIncidencia.Id,
                                    Acción = "Sincronización-Creación",
                                    Usuario = "SyncService",
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
                            }
                            catch {
                                // swallow to avoid breaking sync on logging failure
                            }
                        }
                        break;

                    case "Actualización":
                        if (existingIncidencia != null)
                        {
                            existingIncidencia.Titulo = log.Datos.Titulo;
                            existingIncidencia.Descripcion = log.Datos.Descripcion;
                            existingIncidencia.Estado = log.Datos.Estado;
                            existingIncidencia.Prioridad = log.Datos.Prioridad;
                            existingIncidencia.UltimaActualizacion = log.Datos.UltimaActualizacion;
                            // FechaCreacion should not be updated
                            // Also create a log entry documenting the sync update
                            try
                            {
                                await _logService.CrearLogAsync(new IncidenciaLog
                                {
                                    IncidenciaId = existingIncidencia.Id,
                                    Acción = "Sincronización-Actualización",
                                    Usuario = "SyncService",
                                    Fecha = DateTime.UtcNow,
                                    Datos = new IncidenciaData
                                    {
                                        Titulo = existingIncidencia.Titulo,
                                        Descripcion = existingIncidencia.Descripcion,
                                        Estado = existingIncidencia.Estado,
                                        Prioridad = existingIncidencia.Prioridad,
                                        FechaCreacion = existingIncidencia.FechaCreacion,
                                        UltimaActualizacion = existingIncidencia.UltimaActualizacion
                                    }
                                });
                            }
                            catch {
                                // swallow to avoid breaking sync on logging failure
                            }
                        }
                        break;

                    case "Eliminación":
                        if (existingIncidencia != null)
                        {
                            _context.Incidencias.Remove(existingIncidencia);
                            // Also create a log entry documenting the sync deletion
                            try
                            {
                                await _logService.CrearLogAsync(new IncidenciaLog
                                {
                                    IncidenciaId = existingIncidencia.Id,
                                    Acción = "Sincronización-Eliminación",
                                    Usuario = "SyncService",
                                    Fecha = DateTime.UtcNow,
                                    Datos = null
                                });
                            }
                            catch {
                                // swallow to avoid breaking sync on logging failure
                            }
                        }
                        break;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
