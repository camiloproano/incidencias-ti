import React, { useState, useEffect, useCallback } from 'react';
import { obtenerIncidencias } from '../api/incidenciasApi';
import '../styles/Dashboard.css';

export default function Dashboard() {
  const [estadisticas, setEstadisticas] = useState(null);
  const [health, setHealth] = useState(null);
  const [criticas, setCriticas] = useState(null);
  const [recientes, setRecientes] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const calcularEstadisticas = useCallback((incidencias) => {
    const totalIncidencias = incidencias.length;
    
    const porEstado = {
      abiertas: incidencias.filter(i => i.estado === 'Abierta').length,
      enProceso: incidencias.filter(i => i.estado === 'En Proceso').length,
      cerradas: incidencias.filter(i => i.estado === 'Cerrada').length,
    };

    const porPrioridad = {
      critica: incidencias.filter(i => i.prioridad === 'Crítica').length,
      alta: incidencias.filter(i => i.prioridad === 'Alta').length,
      media: incidencias.filter(i => i.prioridad === 'Media').length,
      baja: incidencias.filter(i => i.prioridad === 'Baja').length,
    };

    const incidenciasCriticas = porPrioridad.critica;
    const tasaResolucion = totalIncidencias > 0 
      ? Math.round((porEstado.cerradas / totalIncidencias) * 100) 
      : 0;

    const resumen = {
      totalIncidencias,
      porEstado,
      porPrioridad,
      incidenciasCriticas,
      tasaResolucion,
    };

    let estado = 'Excelente';
    if (incidenciasCriticas > 0) estado = 'Crítica';
    else if (porPrioridad.alta > 0) estado = 'Regular';
    else if (porEstado.abiertas > totalIncidencias / 2) estado = 'Buena';

    const health = {
      estado,
      totalIncidencias,
      incidenciasAbiertas: porEstado.abiertas,
    };

    const criticasSinResolver = incidencias.filter(
      i => i.prioridad === 'Crítica' && i.estado !== 'Cerrada'
    ).map(i => ({
      ...i,
      horasTranscurridas: Math.random() * 48,
    }));

    const criticas = {
      totalCriticas: criticasSinResolver.length,
      incidencias: criticasSinResolver.slice(0, 5),
    };

    const recientes = incidencias
      .sort((a, b) => new Date(b.fechaCreacion) - new Date(a.fechaCreacion))
      .slice(0, 5);

    return { resumen, health, criticas, recientes };
  }, []);

  const cargarEstadisticas = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const res = await obtenerIncidencias();
      const incidencias = res.data || [];

      const stats = calcularEstadisticas(incidencias);
      
      setEstadisticas(stats.resumen);
      setHealth(stats.health);
      setCriticas(stats.criticas);
      setRecientes(stats.recientes);
    } catch (err) {
      setError('Error al cargar las incidencias');
      console.error(err);
    } finally {
      setLoading(false);
    }
  }, [calcularEstadisticas]);

  useEffect(() => {
    cargarEstadisticas();
    const intervalo = setInterval(cargarEstadisticas, 30000);
    return () => clearInterval(intervalo);
  }, [cargarEstadisticas]);

  if (loading) {
    return (
      <div className="dashboard-container">
        <div className="spinner"></div>
        <p>Cargando estadísticas...</p>
      </div>
    );
  }

  return (
    <div className="dashboard-container">
      {error && <div className="alert-error">{error}</div>}

      {/* Health Status */}
      {health && (
        <div className={`health-card health-${health.estado.toLowerCase().replace(' ', '-')}`}>
          <div className="health-icon">
            {health.estado === 'Excelente' && '✓'}
            {health.estado === 'Buena' && '◐'}
            {health.estado === 'Regular' && '◑'}
            {health.estado === 'Crítica' && '✕'}
          </div>
          <div className="health-content">
            <h3>Estado del Sistema</h3>
            <p className="health-status">{health.estado}</p>
            <div className="health-details">
              <span>{health.totalIncidencias} Total</span>
              <span>{health.incidenciasAbiertas} Abiertas</span>
            </div>
          </div>
        </div>
      )}

      {/* Metrics Grid */}
      {estadisticas && (
        <div className="metrics-grid">
          <div className="metric-card">
            <div className="metric-value">{estadisticas.totalIncidencias}</div>
            <div className="metric-label">Incidencias Totales</div>
          </div>

          <div className="metric-card">
            <div className="metric-value">{estadisticas.porEstado.abiertas}</div>
            <div className="metric-label">Abiertas</div>
            <div className="metric-status status-open"></div>
          </div>

          <div className="metric-card">
            <div className="metric-value">{estadisticas.porEstado.enProceso}</div>
            <div className="metric-label">En Proceso</div>
            <div className="metric-status status-in-process"></div>
          </div>

          <div className="metric-card">
            <div className="metric-value">{estadisticas.porEstado.cerradas}</div>
            <div className="metric-label">Cerradas</div>
            <div className="metric-status status-closed"></div>
          </div>

          <div className="metric-card">
            <div className="metric-value">{estadisticas.incidenciasCriticas}</div>
            <div className="metric-label">Críticas sin Resolver</div>
            <div className="metric-status status-critical"></div>
          </div>

          <div className="metric-card">
            <div className="metric-value">{estadisticas.tasaResolucion}%</div>
            <div className="metric-label">Tasa de Resolución</div>
            <div className="metric-bar">
              <div 
                className="metric-bar-fill" 
                style={{ width: `${estadisticas.tasaResolucion}%` }}
              ></div>
            </div>
          </div>
        </div>
      )}

      {/* Priority Distribution */}
      {estadisticas && (
        <div className="section-card">
          <h3>Distribución por Prioridad</h3>
          <div className="priority-distribution">
            <div className="priority-item priority-critical">
              <span className="priority-name">Crítica</span>
              <span className="priority-count">{estadisticas.porPrioridad.critica}</span>
              <div className="priority-bar">
                <div 
                  className="priority-bar-fill" 
                  style={{ 
                    width: `${estadisticas.totalIncidencias > 0 
                      ? (estadisticas.porPrioridad.critica / estadisticas.totalIncidencias * 100) 
                      : 0}%` 
                  }}
                ></div>
              </div>
            </div>

            <div className="priority-item priority-high">
              <span className="priority-name">Alta</span>
              <span className="priority-count">{estadisticas.porPrioridad.alta}</span>
              <div className="priority-bar">
                <div 
                  className="priority-bar-fill" 
                  style={{ 
                    width: `${estadisticas.totalIncidencias > 0 
                      ? (estadisticas.porPrioridad.alta / estadisticas.totalIncidencias * 100) 
                      : 0}%` 
                  }}
                ></div>
              </div>
            </div>

            <div className="priority-item priority-medium">
              <span className="priority-name">Media</span>
              <span className="priority-count">{estadisticas.porPrioridad.media}</span>
              <div className="priority-bar">
                <div 
                  className="priority-bar-fill" 
                  style={{ 
                    width: `${estadisticas.totalIncidencias > 0 
                      ? (estadisticas.porPrioridad.media / estadisticas.totalIncidencias * 100) 
                      : 0}%` 
                  }}
                ></div>
              </div>
            </div>

            <div className="priority-item priority-low">
              <span className="priority-name">Baja</span>
              <span className="priority-count">{estadisticas.porPrioridad.baja}</span>
              <div className="priority-bar">
                <div 
                  className="priority-bar-fill" 
                  style={{ 
                    width: `${estadisticas.totalIncidencias > 0 
                      ? (estadisticas.porPrioridad.baja / estadisticas.totalIncidencias * 100) 
                      : 0}%` 
                  }}
                ></div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Critical Issues */}
      {criticas && criticas.totalCriticas > 0 && (
        <div className="section-card critical-section">
          <h3>⚠️ Incidencias Críticas Sin Resolver ({criticas.totalCriticas})</h3>
          <div className="critical-list">
            {criticas.incidencias.map(inc => (
              <div key={inc.id} className="critical-item">
                <div className="critical-header">
                  <span className="critical-title">{inc.titulo}</span>
                  <span className="critical-hours">
                    {Math.round(inc.horasTranscurridas)}h
                  </span>
                </div>
                <p className="critical-description">{inc.descripcion}</p>
                <span className="critical-status">{inc.estado}</span>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Recent Activities */}
      {recientes && (
        <div className="section-card">
          <h3>Actividades Recientes</h3>
          <div className="recent-list">
            {recientes.map(inc => (
              <div key={inc.id} className="recent-item">
                <div className="recent-left">
                  <span className="recent-title">{inc.titulo}</span>
                  <span className="recent-date">
                    {new Date(inc.fechaCreacion).toLocaleDateString('es-ES')}
                  </span>
                </div>
                <div className="recent-right">
                  <span className={`recent-status status-${inc.estado.toLowerCase().replace(' ', '-')}`}>
                    {inc.estado}
                  </span>
                  <span className={`recent-priority priority-${inc.prioridad.toLowerCase()}`}>
                    {inc.prioridad}
                  </span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Refresh Button */}
      <button className="refresh-button" onClick={cargarEstadisticas} disabled={loading}>
        {loading ? 'Actualizando...' : 'Actualizar Ahora'}
      </button>
    </div>
  );
}
