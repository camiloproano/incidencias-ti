namespace IncidenciasTI.API.Models
{
    public class Incidencia
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = "Abierta";
        public string Prioridad { get; set; } = "Media";
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
