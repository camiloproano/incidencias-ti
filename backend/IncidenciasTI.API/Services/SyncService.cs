using IncidenciasTI.API.Data;
using IncidenciasTI.API.Models;
using IncidenciasTI.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace IncidenciasTI.Services
{
    public class SyncService
    {
        private readonly AppDbContext _context;
        private readonly LogService _logService;
        private readonly IMongoDatabase _mongoDatabase;

        public SyncService(AppDbContext context, LogService logService, IMongoDatabase mongoDatabase)
        {
            _context = context;
            _logService = logService;
            _mongoDatabase = mongoDatabase;
        }

        public async Task SincronizarAsync()
        {
            // ✅ NUEVA LÓGICA: Lee logs de auditoría para saber QUÉ cambió
            // Luego lee los datos COMPLETOS de PostgreSQL
            // Finalmente replica a MongoDB
            
            var logs = await _logService.ObtenerLogsAsync();
            var mongoCollection = _mongoDatabase.GetCollection<IncidenciaMongo>("IncidenciasDirect");
            int syncCount = 0;

            foreach (var log in logs.OrderBy(l => l.Fecha))
            {
                // Skip sync-generated logs to prevent infinite loops
                if (log.Acción.StartsWith("Sincronización-")) continue;

                Console.WriteLine($"[SYNC] Procesando log: ID={log.IncidenciaId}, Acción={log.Acción}");

                // ✅ CAMBIO: No leemos datos del log, leemos directamente de PostgreSQL
                var incidenciaEnSQL = await _context.Incidencias.FirstOrDefaultAsync(i => i.Id == log.IncidenciaId);

                switch (log.Acción)
                {
                    case "Creación":
                        if (incidenciaEnSQL != null)
                        {
                            // Verificar si ya existe en MongoDB
                            var existeEnMongo = await mongoCollection.Find(i => i.GuidId == incidenciaEnSQL.GuidId).FirstOrDefaultAsync();
                            
                            if (existeEnMongo == null)
                            {
                                var incidenciaMongo = new IncidenciaMongo
                                {
                                    GuidId = incidenciaEnSQL.GuidId,
                                    Titulo = incidenciaEnSQL.Titulo,
                                    Descripcion = incidenciaEnSQL.Descripcion,
                                    Estado = incidenciaEnSQL.Estado,
                                    Prioridad = incidenciaEnSQL.Prioridad,
                                    FechaCreacion = incidenciaEnSQL.FechaCreacion,
                                    UltimaActualizacion = incidenciaEnSQL.UltimaActualizacion
                                };
                                await mongoCollection.InsertOneAsync(incidenciaMongo);
                                Console.WriteLine($"[SYNC] ✅ Creación: Incidencia ID={log.IncidenciaId} ({incidenciaEnSQL.Titulo}) replicada a MongoDB");
                                syncCount++;
                            }
                            else
                            {
                                Console.WriteLine($"[SYNC] ⏭️ Creación: Incidencia ID={log.IncidenciaId} ya existe en MongoDB, omitiendo");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[SYNC] ⚠️ Creación: Incidencia ID={log.IncidenciaId} NO existe en PostgreSQL");
                        }
                        break;

                    case "Actualización":
                        if (incidenciaEnSQL != null)
                        {
                            var filter = Builders<IncidenciaMongo>.Filter.Eq(i => i.GuidId, incidenciaEnSQL.GuidId);
                            var update = Builders<IncidenciaMongo>.Update
                                .Set(i => i.Titulo, incidenciaEnSQL.Titulo)
                                .Set(i => i.Descripcion, incidenciaEnSQL.Descripcion)
                                .Set(i => i.Estado, incidenciaEnSQL.Estado)
                                .Set(i => i.Prioridad, incidenciaEnSQL.Prioridad)
                                .Set(i => i.UltimaActualizacion, incidenciaEnSQL.UltimaActualizacion);

                            var result = await mongoCollection.UpdateOneAsync(filter, update);
                            
                            if (result.MatchedCount > 0)
                            {
                                Console.WriteLine($"[SYNC] ✅ Actualización: Incidencia ID={log.IncidenciaId} actualizada en MongoDB");
                                syncCount++;
                            }
                            else
                            {
                                Console.WriteLine($"[SYNC] ⚠️ Actualización: Incidencia ID={log.IncidenciaId} NO existe en MongoDB para actualizar");
                            }
                        }
                        break;

                    case "Eliminación":
                        // Para eliminación, necesitamos buscar por ID del log (que ya no existe en SQL)
                        // Buscar en logs anteriores para obtener el GUID
                        var guidAnterior = await ObtenerGuidDelLogAnteriorAsync(log.IncidenciaId);
                        if (guidAnterior != Guid.Empty)
                        {
                            var filter = Builders<IncidenciaMongo>.Filter.Eq(i => i.GuidId, guidAnterior);
                            var result = await mongoCollection.DeleteOneAsync(filter);
                            
                            if (result.DeletedCount > 0)
                            {
                                Console.WriteLine($"[SYNC] ✅ Eliminación: Incidencia ID={log.IncidenciaId} eliminada de MongoDB");
                                syncCount++;
                            }
                        }
                        break;
                }
            }

            await _context.SaveChangesAsync();
            Console.WriteLine($"[SYNC] ✅ Sincronización completada: {syncCount} operaciones procesadas");
        }

        private async Task<Guid> ObtenerGuidDelLogAnteriorAsync(int incidenciaId)
        {
            // Buscar en SQL si aún existe (sino, buscar en MongoDB por ID)
            var incidencia = await _context.Incidencias.FirstOrDefaultAsync(i => i.Id == incidenciaId);
            if (incidencia != null)
                return incidencia.GuidId;

            return Guid.Empty;
        }
    }
}
