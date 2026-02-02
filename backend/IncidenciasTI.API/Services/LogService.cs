using IncidenciasTI.Models;
using MongoDB.Driver;

namespace IncidenciasTI.Services
{
    public class LogService
    {
        private readonly IMongoCollection<IncidenciaLog> _logs;

        public LogService(IMongoDatabase database)
        {
            _logs = database.GetCollection<IncidenciaLog>("IncidenciaLogs");
        }

        public async Task CrearLogAsync(IncidenciaLog log)
        {
            await _logs.InsertOneAsync(log);
        }

        public async Task<List<IncidenciaLog>> ObtenerLogsAsync()
        {
            return await _logs.Find(_ => true).ToListAsync();
        }
    }
}
