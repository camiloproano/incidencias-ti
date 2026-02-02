using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using IncidenciasTI.Models;
using IncidenciasTI.Services;

namespace IncidenciasTI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : ControllerBase
    {
        private readonly IMongoDatabase _database;
        private readonly LogService _logService;

        public DebugController(IMongoDatabase database, LogService logService)
        {
            _database = database;
            _logService = logService;
        }

        [HttpGet("mongo/logs")]
        public async Task<IActionResult> GetMongoLogsDebug()
        {
            try
            {
                var dbName = _database.DatabaseNamespace.DatabaseName;
                var collection = _database.GetCollection<IncidenciaLog>("IncidenciaLogs");
                var count = await collection.CountDocumentsAsync(_ => true);
                var recent = await collection.Find(_ => true).SortByDescending(l => l.Fecha).Limit(20).ToListAsync();

                return Ok(new
                {
                    Database = dbName,
                    CollectionName = "IncidenciaLogs",
                    CollectionCount = count,
                    RecentLogs = recent
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("logs-via-service")]
        public async Task<IActionResult> GetLogsViaService()
        {
            try
            {
                var logs = await _logService.ObtenerLogsAsync();
                return Ok(new
                {
                    TotalCount = logs.Count,
                    RecentLogs = logs.OrderByDescending(l => l.Fecha).Take(20).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("database-info")]
        public async Task<IActionResult> GetDatabaseInfo()
        {
            try
            {
                var dbName = _database.DatabaseNamespace.DatabaseName;
                var collections = await _database.ListCollectionNamesAsync();
                var collectionList = await collections.ToListAsync();

                var info = new Dictionary<string, object>
                {
                    { "DatabaseName", dbName },
                    { "Collections", collectionList },
                    { "CollectionCounts", new Dictionary<string, long>() }
                };

                foreach (var collName in collectionList)
                {
                    var coll = _database.GetCollection<dynamic>(collName);
                    var count = await coll.CountDocumentsAsync(_ => true);
                    ((Dictionary<string, long>)info["CollectionCounts"])[collName] = count;
                }

                return Ok(info);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
