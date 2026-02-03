using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IncidenciasTI.API.Data;

namespace IncidenciasTI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadisticasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EstadisticasController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene un resumen general de incidencias
        /// </summary>
        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen()
        {
            try
            {
                var incidencias = await _context.Incidencias.ToListAsync();

                var resumen = new
                {
                    totalIncidencias = incidencias.Count,
                    porEstado = new
                    {
                        abiertas = incidencias.Count(i => i.Estado == "Abierta"),
                        enProceso = incidencias.Count(i => i.Estado == "En Proceso"),
                        cerradas = incidencias.Count(i => i.Estado == "Cerrada")
                    },
                    porPrioridad = new
                    {
                        critica = incidencias.Count(i => i.Prioridad == "Crítica"),
                        alta = incidencias.Count(i => i.Prioridad == "Alta"),
                        media = incidencias.Count(i => i.Prioridad == "Media"),
                        baja = incidencias.Count(i => i.Prioridad == "Baja")
                    },
                    tasaResolucion = incidencias.Count > 0 
                        ? Math.Round((double)incidencias.Count(i => i.Estado == "Cerrada") / incidencias.Count * 100, 2)
                        : 0,
                    incidenciasCriticas = incidencias.Count(i => i.Prioridad == "Crítica" && i.Estado != "Cerrada"),
                    tiempoPromedio = CalcularTiempoPromedio(incidencias)
                };

                return Ok(resumen);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene las incidencias más recientes
        /// </summary>
        [HttpGet("recientes")]
        public async Task<IActionResult> GetRecientes([FromQuery] int cantidad = 5)
        {
            try
            {
                var recientes = await _context.Incidencias
                    .OrderByDescending(i => i.FechaCreacion)
                    .Take(cantidad)
                    .Select(i => new
                    {
                        i.Id,
                        i.Titulo,
                        i.Estado,
                        i.Prioridad,
                        i.FechaCreacion,
                        i.UltimaActualizacion
                    })
                    .ToListAsync();

                return Ok(recientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene incidencias críticas sin resolver
        /// </summary>
        [HttpGet("criticas")]
        public async Task<IActionResult> GetCriticas()
        {
            try
            {
                var criticas = await _context.Incidencias
                    .Where(i => i.Prioridad == "Crítica" && i.Estado != "Cerrada")
                    .OrderByDescending(i => i.FechaCreacion)
                    .Select(i => new
                    {
                        i.Id,
                        i.Titulo,
                        i.Descripcion,
                        i.Estado,
                        i.FechaCreacion,
                        horasTranscurridas = (DateTime.UtcNow - i.FechaCreacion).TotalHours
                    })
                    .ToListAsync();

                return Ok(new
                {
                    totalCriticas = criticas.Count,
                    incidencias = criticas
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene distribución de incidencias por día
        /// </summary>
        [HttpGet("distribucion-temporal")]
        public async Task<IActionResult> GetDistribucionTemporal([FromQuery] int dias = 30)
        {
            try
            {
                var fechaInicio = DateTime.UtcNow.AddDays(-dias);
                
                var distribucion = await _context.Incidencias
                    .Where(i => i.FechaCreacion >= fechaInicio)
                    .GroupBy(i => i.FechaCreacion.Date)
                    .OrderBy(g => g.Key)
                    .Select(g => new
                    {
                        fecha = g.Key.ToString("yyyy-MM-dd"),
                        cantidad = g.Count(),
                        criticas = g.Count(i => i.Prioridad == "Crítica"),
                        resueltas = g.Count(i => i.Estado == "Cerrada")
                    })
                    .ToListAsync();

                return Ok(new
                {
                    periodo = $"Últimos {dias} días",
                    datos = distribucion
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene ranking de estados
        /// </summary>
        [HttpGet("ranking-estados")]
        public async Task<IActionResult> GetRankingEstados()
        {
            try
            {
                var incidencias = await _context.Incidencias.ToListAsync();
                
                var ranking = incidencias
                    .GroupBy(i => i.Estado)
                    .Select(g => new
                    {
                        estado = g.Key,
                        cantidad = g.Count(),
                        porcentaje = Math.Round((double)g.Count() / incidencias.Count * 100, 2),
                        promedioPrioridad = g.Average(i => GetPrioridadNumerica(i.Prioridad))
                    })
                    .OrderByDescending(x => x.cantidad)
                    .ToList();

                return Ok(ranking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene health check del sistema
        /// </summary>
        [HttpGet("health")]
        public async Task<IActionResult> GetHealth()
        {
            try
            {
                var totalIncidencias = await _context.Incidencias.CountAsync();
                var incidenciasAbiertas = await _context.Incidencias
                    .CountAsync(i => i.Estado != "Cerrada");
                
                var salud = incidenciasAbiertas == 0 ? "Excelente" :
                           incidenciasAbiertas <= 3 ? "Buena" :
                           incidenciasAbiertas <= 10 ? "Regular" : "Crítica";

                return Ok(new
                {
                    estado = salud,
                    totalIncidencias = totalIncidencias,
                    incidenciasAbiertas = incidenciasAbiertas,
                    incidenciasCerradas = totalIncidencias - incidenciasAbiertas,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // Métodos auxiliares privados
        private double CalcularTiempoPromedio(List<Models.IncidenciaSql> incidencias)
        {
            var cerradas = incidencias.Where(i => i.Estado == "Cerrada").ToList();
            if (!cerradas.Any()) return 0;

            var tiemposPromedio = cerradas.Average(i => 
                (i.UltimaActualizacion - i.FechaCreacion).TotalHours);

            return Math.Round(tiemposPromedio, 2);
        }

        private int GetPrioridadNumerica(string prioridad)
        {
            return prioridad switch
            {
                "Crítica" => 4,
                "Alta" => 3,
                "Media" => 2,
                "Baja" => 1,
                _ => 0
            };
        }
    }
}
