using IncidenciasTI.API.Data;
using IncidenciasTI.API.Models;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;

namespace IncidenciasTI.Services
{
    public class MongoToSqlSyncService
    {
        private readonly AppDbContext _context;
        private readonly IMongoCollection<IncidenciaMongo> _incidenciasMongo;

        public MongoToSqlSyncService(AppDbContext context, IMongoDatabase database)
        {
            _context = context;
            _incidenciasMongo = database.GetCollection<IncidenciaMongo>("IncidenciasDirect");
        }

        public async Task SincronizarAsync()
        {
            // Obtener todos los documentos de MongoDB
            var mongoIncidencias = await _incidenciasMongo.Find(_ => true).ToListAsync();

            foreach (var mongoDoc in mongoIncidencias)
            {
                // Buscar en SQL por GuidId
                var sqlIncidencia = await _context.Incidencias
                    .FirstOrDefaultAsync(i => i.GuidId == mongoDoc.GuidId);

                if (sqlIncidencia == null)
                {
                    // No existe en SQL - crear nuevo registro
                    var nuevaIncidencia = new IncidenciaSql
                    {
                        GuidId = mongoDoc.GuidId,
                        Titulo = mongoDoc.Titulo,
                        Descripcion = mongoDoc.Descripcion,
                        Estado = mongoDoc.Estado,
                        Prioridad = mongoDoc.Prioridad,
                        FechaCreacion = mongoDoc.FechaCreacion,
                        UltimaActualizacion = mongoDoc.UltimaActualizacion
                    };
                    await _context.Incidencias.AddAsync(nuevaIncidencia);
                }
                else
                {
                    // Existe en SQL - comparar UltimaActualizacion para resolver conflictos
                    if (mongoDoc.UltimaActualizacion > sqlIncidencia.UltimaActualizacion)
                    {
                        // MongoDB es más reciente - actualizar SQL
                        sqlIncidencia.Titulo = mongoDoc.Titulo;
                        sqlIncidencia.Descripcion = mongoDoc.Descripcion;
                        sqlIncidencia.Estado = mongoDoc.Estado;
                        sqlIncidencia.Prioridad = mongoDoc.Prioridad;
                        sqlIncidencia.UltimaActualizacion = mongoDoc.UltimaActualizacion;
                        // FechaCreacion no se actualiza
                    }
                    // Si SQL es más reciente, no hacer nada (mantener SQL como fuente de verdad)
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
