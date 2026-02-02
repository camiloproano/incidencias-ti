using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IncidenciasTI.API.Models
{
    [BsonIgnoreExtraElements]
    public class IncidenciaMongo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public Guid GuidId { get; set; }

        public string Titulo { get; set; } = null!;
        
        public string Descripcion { get; set; } = null!;
        
        public string Estado { get; set; } = null!;
        
        public string Prioridad { get; set; } = null!;
        
        public DateTime FechaCreacion { get; set; }
        
        public DateTime UltimaActualizacion { get; set; } 
    }
}
