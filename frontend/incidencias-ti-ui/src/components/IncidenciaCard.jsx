import { useState } from "react";

export default function IncidenciaCard({ incidencia, onDelete, onUpdate }) {
  const [editando, setEditando] = useState(false);
  const [titulo, setTitulo] = useState(incidencia.titulo);
  const [prioridad, setPrioridad] = useState(incidencia.prioridad);
  const [estado, setEstado] = useState(incidencia.estado);

  const guardar = () => {
    onUpdate(incidencia.id, {
      ...incidencia,
      titulo,
      prioridad,
      estado
    });
    setEditando(false);
  };

  return (
    <div className="card">
      {editando ? (
        <>
          <input
            className="input"
            value={titulo}
            onChange={e => setTitulo(e.target.value)}
          />

          <select value={prioridad} onChange={e => setPrioridad(e.target.value)}>
            <option>Alta</option>
            <option>Media</option>
            <option>Baja</option>
          </select>

          <select value={estado} onChange={e => setEstado(e.target.value)}>
            <option>Abierta</option>
            <option>Cerrada</option>
          </select>

          <button className="btn-primary" onClick={guardar}>Guardar</button>
        </>
      ) : (
        <>
          <h3>{incidencia.titulo}</h3>

          <div className="badges">
            <span className={`badge prioridad ${incidencia.prioridad.toLowerCase()}`}>
              {incidencia.prioridad}
            </span>
            <span className={`badge estado ${incidencia.estado.toLowerCase()}`}>
              {incidencia.estado}
            </span>
          </div>

          <div className="actions">
            <button className="btn-secondary" onClick={() => setEditando(true)}>
              Editar
            </button>
            <button className="btn-danger" onClick={() => onDelete(incidencia.id)}>
              Eliminar
            </button>
          </div>
        </>
      )}
    </div>
  );
}
