namespace IncidenciasTI.API.DTOs
{
    public class UpdateIncidenciaDto
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Prioridad { get; set; } = "Media";
        public string Usuario { get; set; } = "Desconocido"; // para MongoDB
        public string? Estado { get; set; } 
    }
}
