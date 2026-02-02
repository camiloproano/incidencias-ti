using System;

namespace IncidenciasTI.API.Models
{
    public class IncidenciaSql
    {
        public int Id { get; set; }              // PK PostgreSQL
        public Guid GuidId { get; set; }          // ID lógico compartido

        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string Estado { get; set; } = null!;
        public string Prioridad { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public DateTime UltimaActualizacion { get; set; }
    }
}