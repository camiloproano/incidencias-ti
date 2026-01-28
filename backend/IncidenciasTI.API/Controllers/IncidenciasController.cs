using Microsoft.AspNetCore.Mvc;

namespace IncidenciasTI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncidenciasController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API de Incidencias TI funcionando correctamente");
        }
    }
}
