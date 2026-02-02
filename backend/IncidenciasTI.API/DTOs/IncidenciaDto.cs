using System;

namespace IncidenciasTI.API.DTOs
{
    public class IncidenciaDto
    {
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
