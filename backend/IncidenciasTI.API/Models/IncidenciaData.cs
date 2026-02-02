using MongoDB.Bson.Serialization.Attributes;

namespace IncidenciasTI.API.Models
{
    [BsonIgnoreExtraElements]
    public class IncidenciaData
    {
        public required string Titulo { get; set; }
        
        public required string Descripcion { get; set; }
        
        public required string Estado { get; set; }
        
        public required string Prioridad { get; set; }
        
        public required DateTime FechaCreacion { get; set; }
        
        public required DateTime UltimaActualizacion { get; set; }
    }
}
