using Microsoft.AspNetCore.Mvc;
using IncidenciasTI.API.Models;

namespace IncidenciasTI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncidenciasController : ControllerBase
    {
        // Simula base de datos (temporal)
        private static List<Incidencia> incidencias = new()
        {
            new Incidencia { Id = 1, Titulo = "Error de red", Descripcion = "No hay conexión", Prioridad = "Alta" },
            new Incidencia { Id = 2, Titulo = "PC lenta", Descripcion = "Equipo tarda en iniciar", Prioridad = "Media" }
        };

        // GET: api/incidencias
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(incidencias);
        }

        // GET: api/incidencias/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var incidencia = incidencias.FirstOrDefault(i => i.Id == id);
            if (incidencia == null)
                return NotFound("Incidencia no encontrada");

            return Ok(incidencia);
        }

        // POST: api/incidencias
        [HttpPost]
        public IActionResult Create([FromBody] Incidencia nuevaIncidencia)
        {
            if (nuevaIncidencia == null)
                return BadRequest();

            nuevaIncidencia.Id = incidencias.Any()
                ? incidencias.Max(i => i.Id) + 1
                : 1;

            nuevaIncidencia.FechaCreacion = DateTime.Now;

            incidencias.Add(nuevaIncidencia);
            return Ok(nuevaIncidencia);
        }


        // PUT: api/incidencias/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Incidencia incidenciaActualizada)
        {
            var incidencia = incidencias.FirstOrDefault(i => i.Id == id);
            if (incidencia == null)
                return NotFound("Incidencia no encontrada");

            incidencia.Titulo = incidenciaActualizada.Titulo;
            incidencia.Descripcion = incidenciaActualizada.Descripcion;
            incidencia.Estado = incidenciaActualizada.Estado;
            incidencia.Prioridad = incidenciaActualizada.Prioridad;

            return Ok(incidencia);
        }

        // DELETE: api/incidencias/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var incidencia = incidencias.FirstOrDefault(i => i.Id == id);
            if (incidencia == null)
                return NotFound("Incidencia no encontrada");

            incidencias.Remove(incidencia);
            return Ok("Incidencia eliminada correctamente");
        }
    }
}
