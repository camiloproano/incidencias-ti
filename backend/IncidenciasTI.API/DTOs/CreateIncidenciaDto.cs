namespace IncidenciasTI.API.DTOs
{
    public class CreateIncidenciaDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Prioridad { get; set; } = "Media";

        // Nuevo campo para registrar quién crea la incidencia
        public string Usuario { get; set; } = "Desconocido";
    }
}
