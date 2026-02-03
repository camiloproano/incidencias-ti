import { useState } from "react";
import "../styles/IncidenciaCard.css";

export default function IncidenciaCard({ incidencia, onDelete, onUpdate }) {
  const [editando, setEditando] = useState(false);
  const [titulo, setTitulo] = useState(incidencia.titulo);
  const [descripcion, setDescripcion] = useState(incidencia.descripcion || "");
  const [prioridad, setPrioridad] = useState(incidencia.prioridad);
  const [estado, setEstado] = useState(incidencia.estado);

  const guardar = () => {
    if (!titulo.trim()) {
      alert("El título es obligatorio");
      return;
    }
    onUpdate(incidencia.id, {
      titulo,
      descripcion,
      prioridad,
      estado
    });
    setEditando(false);
  };

  const getPriorityColor = (prio) => {
    const colors = {
      "Baja": "#28a745",
      "Media": "#ffc107",
      "Alta": "#dc3545",
      "Crítica": "#721c24"
    };
    return colors[prio] || "#6c757d";
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString("es-ES", {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit"
    });
  };

  return (
    <div className="incidencia-card">
      {editando ? (
        <div className="card-edit-mode">
          <div className="form-group">
            <label>Título</label>
            <input
              type="text"
              value={titulo}
              onChange={(e) => setTitulo(e.target.value)}
              maxLength={100}
            />
          </div>

          <div className="form-group">
            <label>Descripción</label>
            <textarea
              value={descripcion}
              onChange={(e) => setDescripcion(e.target.value)}
              rows={2}
              maxLength={500}
            />
          </div>

          <div className="form-row">
            <div className="form-group">
              <label>Prioridad</label>
              <select value={prioridad} onChange={(e) => setPrioridad(e.target.value)}>
                <option value="Baja">🟢 Baja</option>
                <option value="Media">🟡 Media</option>
                <option value="Alta">🔴 Alta</option>
                <option value="Crítica">⚠️ Crítica</option>
              </select>
            </div>

            <div className="form-group">
              <label>Estado</label>
              <select value={estado} onChange={(e) => setEstado(e.target.value)}>
                <option value="Abierta">Abierta</option>
                <option value="En Proceso">En Proceso</option>
                <option value="Cerrada">Cerrada</option>
              </select>
            </div>
          </div>

          <div className="card-actions">
            <button className="btn-primary" onClick={guardar}>✓ Guardar</button>
            <button className="btn-secondary" onClick={() => setEditando(false)}>✕ Cancelar</button>
          </div>
        </div>
      ) : (
        <>
          <div className="card-header">
            <h3>{titulo}</h3>
            <div className="badges">
              <span
                className="badge-prioridad"
                style={{ backgroundColor: getPriorityColor(prioridad) }}
              >
                {prioridad}
              </span>
              <span className={`badge-estado ${estado === "Abierta" ? "abierta" : "cerrada"}`}>
                {estado}
              </span>
            </div>
          </div>

          <div className="card-body">
            {descripcion && <p className="descripcion">{descripcion}</p>}
            <div className="card-info">
              <div className="info-item">
                <span className="label">Creada:</span>
                <span className="value">{formatDate(incidencia.fechaCreacion)}</span>
              </div>
              <div className="info-item">
                <span className="label">Actualizada:</span>
                <span className="value">{formatDate(incidencia.ultimaActualizacion)}</span>
              </div>
            </div>
          </div>

          <div className="card-footer">
            <button className="btn-secondary" onClick={() => setEditando(true)}>✏️ Editar</button>
            <button className="btn-danger" onClick={() => {
              if (confirm("¿Estás seguro de que deseas eliminar esta incidencia?")) {
                onDelete(incidencia.id);
              }
            }}>🗑️ Eliminar</button>
          </div>
        </>
      )}
    </div>
  );
}
