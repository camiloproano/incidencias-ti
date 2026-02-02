using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using IncidenciasTI.API.Models;

namespace IncidenciasTI.Models
{
    [BsonIgnoreExtraElements]
    public class IncidenciaLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public int IncidenciaId { get; set; }
        
        public string Acción { get; set; } = string.Empty;
        
        public string Usuario { get; set; } = string.Empty;
        
        public DateTime Fecha { get; set; }
        
        public IncidenciaData? Datos { get; set; }
    }
}
